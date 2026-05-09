using Microsoft.AspNetCore.Mvc;
namespace uyg.UI.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();
        public IActionResult NewsDetail() => View();
    }
}