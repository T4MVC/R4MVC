using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace R4MvcHostApp.Areas.Admin.Controllers
{
    [AdminArea]
    public abstract class AdminControllerBase : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
        }
    }
}
