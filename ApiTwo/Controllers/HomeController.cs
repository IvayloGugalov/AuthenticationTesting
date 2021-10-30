using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace ApiTwo.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory clientFactory;

        public HomeController(IHttpClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        [Route("/home")]
        public async Task<IActionResult> Index()
        {
            // retrieve access token
            var serverClient = this.clientFactory.CreateClient();

            // url can be set in startup
            // discover endpoints from metadata
            var discoveryDocument = await serverClient.GetDiscoveryDocumentAsync("http://localhost:46293");

            if (discoveryDocument.IsError) return BadRequest(discoveryDocument.Error);

            // Based on:
            // IdentityServer.ConfigurationConst => new Client { .. }
            var tokenResponse = await serverClient.RequestClientCredentialsTokenAsync(
                new ClientCredentialsTokenRequest
                {
                    Address = discoveryDocument.TokenEndpoint,
                    ClientId = "client_id",
                    ClientSecret = "client_secret",
                    Scope = "ApiOne"
                });

            if (tokenResponse.IsError) return BadRequest(tokenResponse.Error);

            // retrieve data

            var apiClient = this.clientFactory.CreateClient();

            apiClient.SetBearerToken(tokenResponse.AccessToken);

            // ApiOne Url
            var response = await apiClient.GetAsync("http://localhost:53530/secret");

            if (response.StatusCode == HttpStatusCode.Unauthorized) return Unauthorized();

            var content = await response.Content.ReadAsStringAsync();

            return Ok(new
            {
                access_token = tokenResponse.AccessToken,
                message = content,
            });
        }
    }
}
