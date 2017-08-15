using Microsoft.AspNetCore.Mvc;

namespace R4MvcHostApp.Controllers
{
    public partial class HomeController
    {
        public virtual IActionResult ExtensionTest()
        {
            return Content("Success!");
        }
    }
}
