using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAuthorizationService authorizationService;
        private readonly HttpClient httpClient;

        public HomeController(IAuthorizationService authorizationService, IHttpClientFactory httpClientFactory)
        {
            this.authorizationService = authorizationService;
            this.httpClient = httpClientFactory.CreateClient();
        }


        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [Route("secret")]
        public async Task<IActionResult> Secret()
        {
            // Used for accessing other APIs. If the token is not valid, the API will not execute anything, based on implementation
            var token = await HttpContext.GetTokenAsync("access_token");

            this.httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            // Server URL, go to secret page
            var serverResponse = await this.httpClient.GetAsync("http://localhost:14744/secret/index");
            
            // API URL, go to secret page
            var apiResponse = await this.httpClient.GetAsync("http://localhost:21060/secret/index");

            return View();
        }
    }
}
