using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Basic.Policies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace Basic.Controllers
{
    public class FirmController : Controller
    {

        private readonly IAuthorizationService authorizationService;

        public FirmController(IAuthorizationService authorizationService)
        {
            this.authorizationService = authorizationService;
        }

        // Authenticate as a Boss
        public async Task<IActionResult> Index()
        {
            //// Hierarchy Policy
            var firmClaims = new List<Claim>()
            {
                new Claim(FirmHierarchyPolicy.FirmHierarchy, "Boss")
            };

            var firmIdentity = new ClaimsIdentity(firmClaims, "Firm Identity");
            var userPrincipal = new ClaimsPrincipal(new[] { firmIdentity });

            await HttpContext.SignInAsync(userPrincipal);

            return View();
        }

        // Only the CEO can access this
        [FirmHierarchy("CEO")]
        public IActionResult CEO()
        {
            return View();
        }

        // Only the Boss can access this
        [FirmHierarchy("Boss")]
        public IActionResult Boss()
        {
            return View();
        }
    }
}
