using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Server.Controllers
{
    public class Home : Controller
    {
        private readonly IAuthorizationService authorizationService;

        public Home(IAuthorizationService authorizationService)
        {
            this.authorizationService = authorizationService;
        }

        public IActionResult Index()
        {
            return View();
        }

        // pass token in header as Bearer
        // or inside Request.Query /secret?access_token={token value}
        // see startup - config.Events = new JwtBearerEvents
        [Authorize]
        [Route("secret")]
        public IActionResult Secret()
        {
            return View();
        }

        // get token
        [Route("auth")]
        public IActionResult Authenticate()
        {
            // Ways to recognize the user
            var ivoClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "Ivo"),
                new Claim(ClaimTypes.Email, "Ivo@mail.bg"),
                new Claim(JwtRegisteredClaimNames.Sub, "_id")
            };

            var secretBytes = Encoding.UTF8.GetBytes(Constants.SecretValue);
            var key = new SymmetricSecurityKey(secretBytes);
            const string algorithm = SecurityAlgorithms.HmacSha256;

            var signingCredentials = new SigningCredentials(key, algorithm);

            var token = new JwtSecurityToken(
                issuer: Constants.Issuer,
                audience: Constants.Audience,
                claims: ivoClaims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddDays(10),
                signingCredentials: signingCredentials);

            var tokenAsJson = new JwtSecurityTokenHandler().WriteToken(token);


            return Ok(new { access_token = tokenAsJson } );
        }
    }
}
