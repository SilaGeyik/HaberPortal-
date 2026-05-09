using Microsoft.AspNetCore.Mvc;
namespace uyg.UI.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Login() => View();
        public IActionResult Register() => View();
    }
}