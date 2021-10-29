﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.IdentityModel.Tokens;

namespace Server.Controllers
{
    public class OAuth : Controller
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
            string client_id)
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

            var token = new JwtSecurityToken(
                issuer: Constants.Issuer,
                audience: Constants.Audience,
                claims: ivoClaims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddDays(10),
                signingCredentials: signingCredentials);

            var access_token = new JwtSecurityTokenHandler().WriteToken(token);

            var responseObject = new
            {
                access_token,
                token_type = "Bearer",
                raw_claim = "oauthTutorial"
            };

            var jsonResponse = JsonSerializer.Serialize(responseObject);
            var responseBytes = Encoding.UTF8.GetBytes(jsonResponse);

            await Response.Body.WriteAsync(responseBytes, 0, responseBytes.Length);


            return Redirect(redirect_uri);
        }
    }
}
