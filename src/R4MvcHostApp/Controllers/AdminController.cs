using Microsoft.AspNetCore.Mvc;

namespace R4MvcHostApp.Controllers
{
    // Testing area/controller name clash avoidance
    public partial class AdminController : Controller
    {
        public virtual IActionResult Index()
        {
            return View();
        }
    }
}