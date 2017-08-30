using Microsoft.AspNetCore.Mvc;

namespace R4MvcHostApp.Controllers
{
    public abstract class AboutControllerBase : Controller
    {
        public IActionResult About()
        {
            return View();
        }
    }
}
