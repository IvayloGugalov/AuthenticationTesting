using System.Collections.Generic;
using System.Security.Claims;

namespace Basic.Authenticators
{
    public static class HomeControllerAuthenticator
    {
        public static ClaimsPrincipal GenerateClaims()
        {
            // Ways to recognize the user
            var ivoClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "Ivo"),
                new Claim(ClaimTypes.Email, "Ivo@mail.bg"),
                new Claim("favourite food", "tiramisu"),
                // Policy
                new Claim(ClaimTypes.Country, "Bulgaria"),
                // Role
                new Claim(ClaimTypes.Role, "Admin")
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

            return userPrincipal;
        }
    }
}
