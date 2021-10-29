using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace Basic.Transformer
{
    public class ClaimsTransformation : IClaimsTransformation
    {
        private readonly IEnumerable<Claim> claims;

        public ClaimsTransformation(IEnumerable<Claim> claims)
        {
            this.claims = claims;
        }

        /// <summary>
        /// Add Claims when they are required
        /// Hard to pass the new Claims
        /// </summary>
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var notRegisteredClaims = this.claims.Where(x => !principal.Claims.Contains(x))
                .ToArray();

            if (notRegisteredClaims.Any())
            {
                var identity = principal.Identity as ClaimsIdentity;
                identity?.AddClaims(notRegisteredClaims);
            }

            return Task.FromResult(principal);
        }
    }
}
