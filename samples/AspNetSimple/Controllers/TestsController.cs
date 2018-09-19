using System;
using System.Threading.Tasks;
using AspNetSimple.Models;
using Microsoft.AspNetCore.Mvc;
using SampleModels;

namespace AspNetSimple.Controllers
{
    public partial class TestsController : DebugControllerBase
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
        [NonAction]
        public IActionResult NonActionWithParams(int id) => throw new NotImplementedException();
        [R4MvcExclude]
        public IActionResult R4MvcExcluded() => throw new NotImplementedException();
        [R4MvcExclude]
        public IActionResult R4MvcExcludedWithParams(int id) => throw new NotImplementedException();

        public virtual IActionResult Parameters(int id, string name) => throw new NotImplementedException();
        public virtual IActionResult ParametersWithDefault(int id = 5, string name = "test") => throw new NotImplementedException();

        public virtual Product ApiCall() => throw new NotImplementedException();
        public virtual Product ApiCallWithParams(int id) => throw new NotImplementedException();

        public virtual ActionResult<Product> ApiCallTyped() => throw new NotImplementedException();
        public virtual ActionResult<Product> ApiCallTypedWithParams(int id) => throw new NotImplementedException();

        public virtual IActionResult LocalViewModel(ErrorViewModel model) => throw new NotImplementedException();
        public virtual IActionResult ExternalViewModel(TestViewModel model) => throw new NotImplementedException();
    }
}
