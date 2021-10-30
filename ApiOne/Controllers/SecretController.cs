using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.AspNetCore.Authorization;

namespace ApiOne.Controllers
{
    public class SecretController : Controller
    {
        [Route("/secret")]
        [Authorize]
        [HttpGet]
        public string Index()
        {
            return "secret message from API ONE";
        }
    }
}
