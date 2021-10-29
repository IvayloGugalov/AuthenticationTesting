using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Basic.Policies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Basic.Controllers
{
    public class OperationsController : Controller
    {
        private readonly IAuthorizationService authorizationService;
        public OperationsController(IAuthorizationService authorizationService)
        {
            this.authorizationService = authorizationService;
        }

        /// <summary>
        /// Sign in as the Cookie monster
        /// </summary>
        public IActionResult Authenticate()
        {
            var cookieMonsterClaims = new List<Claim>()
            {
                new Claim(CookieJarPolicy.MonsterClaim, CookieJarPolicy.CookieMonsterValue)
            };

            var cookieMonsterIdentity = new ClaimsIdentity(cookieMonsterClaims, "Cookie Monster Identity");
            var userPrincipal = new ClaimsPrincipal(new[] { cookieMonsterIdentity });

            HttpContext.SignInAsync(userPrincipal);

            return Ok();
        }

        public async Task<IActionResult> Open()
        {
            var requirement = new OperationAuthorizationRequirement()
            {
                Name = CookieJarOperations.NotAllowed
            };

            // Check which claims the user has. Here authorization will fail as we have the Cookie Monster claim which is not allowed on this Endpoint.
            var result = await this.authorizationService.AuthorizeAsync(User, null, requirement);

            if (!result.Succeeded) return Unauthorized();

            return Ok();
        }
    }

    public static class CookieJarOperations
    {
        public static string Open => "Open";
        public static string NotAllowed => "NotAllowed";
    }
}
