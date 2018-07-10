using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AspNetSimple.Controllers
{
    public partial class TestsController : Controller
    {
        public virtual IActionResult Index() => throw new NotImplementedException();
        public virtual ActionResult ActionResult() => throw new NotImplementedException();
        public virtual JsonResult JsonResult() => throw new NotImplementedException();
        public virtual FileResult FileResult() => throw new NotImplementedException();
        public virtual RedirectResult RedirectResult() => throw new NotImplementedException();
        public virtual RedirectToActionResult RedirectToActionResult() => throw new NotImplementedException();
        public virtual RedirectToRouteResult RedirectToRouteResult() => throw new NotImplementedException();

        public virtual Task<IActionResult> TaskIndex() => throw new NotImplementedException();
        public virtual Task<ActionResult> TaskActionResult() => throw new NotImplementedException();
        public virtual Task<JsonResult> TaskJsonResult() => throw new NotImplementedException();
        public virtual Task<FileResult> TaskFileResult() => throw new NotImplementedException();
        public virtual Task<RedirectResult> TaskRedirectResult() => throw new NotImplementedException();
        public virtual Task<RedirectToActionResult> TaskRedirectToActionResult() => throw new NotImplementedException();
        public virtual Task<RedirectToRouteResult> TaskRedirectToRouteResult() => throw new NotImplementedException();

        [RequireHttps]
        public virtual IActionResult RequiresHttps() => throw new NotImplementedException();
        [NonAction]
        public IActionResult NonAction() => throw new NotImplementedException();

        public virtual IActionResult Parameters(int id, string name) => throw new NotImplementedException();
        public virtual IActionResult ParametersWithDefault(int id = 5, string name = "test") => throw new NotImplementedException();

        public virtual object ApiCall() => throw new NotImplementedException();
        public virtual object ApiCallWithParams(int id) => throw new NotImplementedException();
    }
}
