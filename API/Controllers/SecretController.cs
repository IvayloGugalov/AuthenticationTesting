using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class SecretController : Controller
    {
        // Authorize will always go through the JwtRequirementHandler
        [Authorize]
        public string Index()
        {
            return "secret message API";
        }
    }
}
