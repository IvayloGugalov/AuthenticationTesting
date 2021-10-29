using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Home
{
    public class HomeController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        public HomeController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
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

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await this.userManager.FindByNameAsync(username);

            if (user == null) return NotFound();

            var result = await this.signInManager.PasswordSignInAsync(user, password, false, false);

            if (!result.Succeeded) return NotFound();

            return RedirectToAction(nameof(Index));
        }
        
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string username, string password)
        {
            var user = new IdentityUser
            {
                UserName = username,
                Email = ""
            };

            var createResult = await this.userManager.CreateAsync(user, password);

            if (createResult.Succeeded)
            {
                var signInResult = await this.userManager.FindByNameAsync(username);

                if (signInResult == null) return NotFound();

                var result = await this.signInManager.PasswordSignInAsync(signInResult, password, false, false);

                return RedirectToAction(nameof(Index));
            }

            return NotFound();
        }

        public async Task<IActionResult> LogOut()
        {
            await this.signInManager.SignOutAsync();

            return Ok();
        }

    }
}
