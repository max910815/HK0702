using Microsoft.AspNetCore.Mvc;

namespace HK_Product.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult AboutView()
        {
            return View();
        }
    }
}
