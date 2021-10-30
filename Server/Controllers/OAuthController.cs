using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.IdentityModel.Tokens;

namespace Server.Controllers
{
    public class OAuthController : Controller
    {
        [HttpGet]
        public IActionResult Login(
            string response_type, // authorization flow type
            string client_id, // client Id
            string redirect_uri, // redirect Uri
            string scope, // requested information = email..
            string state /* randomly generated string for confirmation that we are returning to the same client */ )
        {
            var query = new QueryBuilder
            {
                {"redirect_uri", redirect_uri},
                {"state", state}
            };

            return View(model: query.ToString());
        }

        [HttpPost]
        public IActionResult Login(
            string username,
            string redirect_uri,
            string state)
        {
            const string code = "blahblah";

            var query = new QueryBuilder
            {
                {"code", code},
                {"state", state}
            };

            return Redirect($"{redirect_uri}{query.ToString()}");
        }

        public async Task<IActionResult> Token(
            string grant_type,
            string code,
            string redirect_uri,
            string client_id,
            string refresh_token)
        {
            // mechanism to validate the "code"

            var ivoClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "Ivo"),
                new Claim(ClaimTypes.Email, "Ivo@mail.bg"),
                new Claim(JwtRegisteredClaimNames.Sub, "_id")
            };

            var secretBytes = Encoding.UTF8.GetBytes(Constants.SecretValue);
            var key = new SymmetricSecurityKey(secretBytes);
            const string algorithm = SecurityAlgorithms.HmacSha256;

            var signingCredentials = new SigningCredentials(key, algorithm);

            var expires = grant_type == "refresh_token"
                ? DateTime.Now.AddMinutes(5)
                : DateTime.Now.AddMilliseconds(1);

            var token = new JwtSecurityToken(
                issuer: Constants.Issuer,
                audience: Constants.Audience,
                claims: ivoClaims,
                notBefore: DateTime.Now,
                expires: expires,
                signingCredentials: signingCredentials);

            var access_token = new JwtSecurityTokenHandler().WriteToken(token);

            var responseObject = new
            {
                access_token,
                token_type = "Bearer",
                raw_claim = "oauthTutorial",
                refresh_token = "SomeRefreshTokenHere"
            };

            var jsonResponse = JsonSerializer.Serialize(responseObject);
            var responseBytes = Encoding.UTF8.GetBytes(jsonResponse);

            await Response.Body.WriteAsync(responseBytes, 0, responseBytes.Length);

            return Redirect(redirect_uri);
        }

        [Authorize]
        public IActionResult Validate()
        {
            if (!HttpContext.Request.Query.TryGetValue("access_token", out var accessToken)) return BadRequest();

            return Ok(new { accessToken });
        }
    }
}
