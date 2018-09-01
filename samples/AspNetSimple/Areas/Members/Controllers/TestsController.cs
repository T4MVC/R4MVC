using Microsoft.AspNetCore.Mvc;

namespace AspNetSimple.Areas.Members.Controllers
{
    [MembersArea]
    public partial class TestsController : Controller
    {
        public virtual IActionResult Index()
        {
            return View();
        }
    }
}
