using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAuthorizationService authorizationService;
        private readonly IHttpClientFactory httpClientFactory;

        public HomeController(IAuthorizationService authorizationService, IHttpClientFactory httpClientFactory)
        {
            this.authorizationService = authorizationService;
            this.httpClientFactory = httpClientFactory;
        }


        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [Route("secret")]
        public async Task<IActionResult> Secret()
        {

            // Server URL, go to secret page
            // This will return Unauthorized as the access token has expired
            // Try to refresh the access token
            var serverResponse = await this.TryRefreshingAccessToken(() => this.SecuredGetRequest("http://localhost:14744/secret/index"));

            // API URL, go to secret page
            // Token was refreshed and we have access
            var apiResponse = await this.TryRefreshingAccessToken(() =>
                this.SecuredGetRequest("http://localhost:21060/secret/index"));

            return View();
        }

        public async Task<HttpResponseMessage> TryRefreshingAccessToken(Func<Task<HttpResponseMessage>> securedRequest)
        {
            var response = await securedRequest();

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await RefreshAccessToken();
                response = await securedRequest();
            }

            return response;
        }

        private async Task RefreshAccessToken()
        {
            const string refresh_token = "refresh_token";
            const string access_token = "access_token";
            const string grant_type = "grant_type";
            const string basicCredentials = "username:password";

            var refreshToken = await HttpContext.GetTokenAsync(refresh_token);

            var refreshTokenClient = this.httpClientFactory.CreateClient();

            var data = new Dictionary<string, string>
            {
                {grant_type, refresh_token},
                {refresh_token, refreshToken},
            };

            var encodedCredentials = Encoding.UTF8.GetBytes(basicCredentials);
            var base64 = Convert.ToBase64String(encodedCredentials);

            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:14744/oauth/token")
            {
                Content = new FormUrlEncodedContent(data)
            };

            request.Headers.Add("Authorization", $"Basic {base64}");
            var response = await refreshTokenClient.SendAsync(request);

            var responseJson = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();

            var newAccessToken = responseJson!.GetValueOrDefault(access_token);
            var newRefreshToken = responseJson!.GetValueOrDefault(refresh_token);

            var authInfo = await HttpContext.AuthenticateAsync(Constants.CookieAuthScheme);
            authInfo.Properties.UpdateTokenValue(access_token, newAccessToken);
            authInfo.Properties.UpdateTokenValue(refresh_token, newRefreshToken);

            await HttpContext.SignInAsync(Constants.CookieAuthScheme, authInfo.Principal, authInfo.Properties);
        }

        /// <summary>
        /// Used for accessing other APIs. If the access token is not valid, the API should not execute anything, based on implementation
        /// </summary>
        private async Task<HttpResponseMessage> SecuredGetRequest(string url)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var client = this.httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            return await client.GetAsync(url);
        }
    }
}
