using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace R4MvcHostApp.Areas.Admin.Controllers
{
    public partial class PagesController : AdminControllerBase
    {
        public virtual IActionResult Index()
        {
            return View();
        }

        [NonAction]
        public IActionResult ExcludedNonAction()
        {
            return View();
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
        }
    }
}
