using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers
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

        [Authorize]
        [Route("secret")]
        public async Task<IActionResult> Secret()
        {
            // Used for accessing other APIs. If the token is not valid, the API will not execute anything, based on implementation
            var token = await HttpContext.GetTokenAsync("access_token");

            return View();
        }
    }
}
