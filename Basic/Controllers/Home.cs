using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Basic.Controllers
{
    public class Home : Controller
    {
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

        public IActionResult Authenticate()
        {
            // Ways to recognize the user
            var ivoClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "Ivo"),
                new Claim(ClaimTypes.Email, "Ivo@mail.bg"),
                new Claim("favourite food", "tiramisu")
            };

            var licenseClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email, "Ivo@mail.bg"),
                new Claim("License", "B2")
            };

            // Identity based on the claims
            var ivoIdentity = new ClaimsIdentity(ivoClaims, "Ivo Identity");
            var licenseIdentity = new ClaimsIdentity(licenseClaims, "License Identity");

            var userPrincipal = new ClaimsPrincipal(new[] { ivoIdentity, licenseIdentity });

            HttpContext.SignInAsync(userPrincipal);

            return RedirectToAction(nameof(Index));
        }
    }
}
