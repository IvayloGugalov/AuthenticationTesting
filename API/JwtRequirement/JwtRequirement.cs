using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace API.JwtRequirement
{
    public class JwtRequirement : IAuthorizationRequirement
    {
    }

    public class JwtRequirementHandler : AuthorizationHandler<JwtRequirement>
    {
        private readonly HttpClient client;
        // can be made to pass the access token through the url
        private readonly HttpContext httpContext;

        public JwtRequirementHandler(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            this.client = httpClientFactory.CreateClient();
            this.httpContext = httpContextAccessor.HttpContext;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, JwtRequirement requirement)
        {
            if (this.httpContext.Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                // Get the actual token without the Bearer in front (Bearer df6sd768gc....)
                var accessToken = authHeader.ToString().Split(' ')[1];

                // OAuthController.Validate()
                // Validate with the server
                var response = await this.client.GetAsync($"http://localhost:14744/oauth/validate?access_token={accessToken}");

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    context.Succeed(requirement);
                }
            }
        }
    }
}
