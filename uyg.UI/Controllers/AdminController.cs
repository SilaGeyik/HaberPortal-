using Microsoft.AspNetCore.Mvc;
namespace Uyg.UI.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Dashboard() => View(); 
        public IActionResult Categories() => View(); 
        public IActionResult News() => View();
        public IActionResult Comments() => View(); 
    }
}