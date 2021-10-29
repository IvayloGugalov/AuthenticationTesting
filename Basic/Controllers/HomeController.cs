using System.Threading.Tasks;
using Basic.Authenticators;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Basic.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAuthorizationService authorizationService;

        public HomeController(IAuthorizationService authorizationService)
        {
            this.authorizationService = authorizationService;
        }

        public async Task<IActionResult> DoWork()
        {
            // Pass policy name or AuthorizationPolicy object
            // Do this inside razor page also
            var result = await this.authorizationService.AuthorizeAsync(User, "Claim.UserCountry");

            if (!result.Succeeded) return Unauthorized();

            return View(nameof(Index));
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [Route("secret")]
        public IActionResult Secret()
        {
            return View();
        }

        [Authorize(Policy = "Claim.UserCountry")]
        public IActionResult SecretWithPolicy()
        {
            return RedirectToAction(nameof(Secret));
        }

        [Authorize(Roles = "Admin")]
        public IActionResult SecretRole()
        {
            return RedirectToAction(nameof(Secret));
        }

        public IActionResult Authenticate()
        {
            var userPrincipal = HomeControllerAuthenticator.GenerateClaims();

            // Register all the claims and sign in
            HttpContext.SignInAsync(userPrincipal);

            return RedirectToAction(nameof(Index));
        }
    }
}
