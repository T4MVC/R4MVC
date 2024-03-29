// <auto-generated />
// This file was generated by R4Mvc.
// Don't change it directly as your change would get overwritten.  Instead, make changes
// to the r4mvc.json file (i.e. the settings file), save it and run the generator tool again.

// Make sure the compiler doesn't complain about missing Xml comments and CLS compliance
// 0108: suppress "Foo hides inherited member Foo.Use the new keyword if hiding was intended." when a controller and its abstract parent are both processed
#pragma warning disable 1591, 3008, 3009, 0108
using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using R4Mvc;
using AspNetSimple;

namespace AspNetSimple.Pages.Categories
{
    public partial class DetailsModel : IR4ActionResult
    {
        [GeneratedCode("R4Mvc", "1.0"), DebuggerNonUserCode]
        public DetailsModel()
        {
        }

        [GeneratedCode("R4Mvc", "1.0"), DebuggerNonUserCode]
        protected DetailsModel(Dummy d)
        {
        }

        [GeneratedCode("R4Mvc", "1.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToAction(IActionResult result)
        {
            var callInfo = result.GetR4ActionResult();
            return RedirectToRoute(callInfo.RouteValueDictionary);
        }

        [GeneratedCode("R4Mvc", "1.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToAction(Task<IActionResult> taskResult)
        {
            return RedirectToAction(taskResult.Result);
        }

        [GeneratedCode("R4Mvc", "1.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToActionPermanent(IActionResult result)
        {
            var callInfo = result.GetR4ActionResult();
            return RedirectToRoutePermanent(callInfo.RouteValueDictionary);
        }

        [GeneratedCode("R4Mvc", "1.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToActionPermanent(Task<IActionResult> taskResult)
        {
            return RedirectToActionPermanent(taskResult.Result);
        }

        [GeneratedCode("R4Mvc", "1.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToPage(IActionResult result)
        {
            var callInfo = result.GetR4ActionResult();
            return RedirectToRoute(callInfo.RouteValueDictionary);
        }

        [GeneratedCode("R4Mvc", "1.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToPage(Task<IActionResult> taskResult)
        {
            return RedirectToPage(taskResult.Result);
        }

        [GeneratedCode("R4Mvc", "1.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToPagePermanent(IActionResult result)
        {
            var callInfo = result.GetR4ActionResult();
            return RedirectToRoutePermanent(callInfo.RouteValueDictionary);
        }

        [GeneratedCode("R4Mvc", "1.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToPagePermanent(Task<IActionResult> taskResult)
        {
            return RedirectToPagePermanent(taskResult.Result);
        }

        [GeneratedCode("R4Mvc", "1.0"), DebuggerNonUserCode]
        string IR4ActionResult.Protocol => null;
        [GeneratedCode("R4Mvc", "1.0")]
        RouteValueDictionary m_RouteValueDictionary = new RouteValueDictionary{{"Page", "/Categories/Details"}};
        [GeneratedCode("R4Mvc", "1.0"), DebuggerNonUserCode]
        RouteValueDictionary IR4ActionResult.RouteValueDictionary => m_RouteValueDictionary;
        [NonHandler]
        [GeneratedCode("R4Mvc", "1.0"), DebuggerNonUserCode]
        public virtual IActionResult OnGet()
        {
            return new R4Mvc_Microsoft_AspNetCore_Mvc_RazorPages_ActionResult(Name, null);
        }

        [GeneratedCode("R4Mvc", "1.0")]
        public readonly string Name = "/Categories/Details";
        [GeneratedCode("R4Mvc", "1.0")]
        public const string NameConst = "/Categories/Details";
        [GeneratedCode("R4Mvc", "1.0")]
        static readonly HandlerNamesClass s_HandlerNames = new HandlerNamesClass();
        [GeneratedCode("R4Mvc", "1.0"), DebuggerNonUserCode]
        public HandlerNamesClass HandlerNames => s_HandlerNames;
        [GeneratedCode("R4Mvc", "1.0"), DebuggerNonUserCode]
        public class HandlerNamesClass
        {
        }

        [GeneratedCode("R4Mvc", "1.0"), DebuggerNonUserCode]
        public class HandlerNameConstants
        {
        }

        [GeneratedCode("R4Mvc", "1.0")]
        static readonly HandlerParamsClass_OnGet s_OnGetParams = new HandlerParamsClass_OnGet();
        [GeneratedCode("R4Mvc", "1.0"), DebuggerNonUserCode]
        public HandlerParamsClass_OnGet OnGetParams => s_OnGetParams;
        [GeneratedCode("R4Mvc", "1.0"), DebuggerNonUserCode]
        public class HandlerParamsClass_OnGet
        {
            public readonly string id = "id";
        }
    }

    [GeneratedCode("R4Mvc", "1.0"), DebuggerNonUserCode]
    public partial class R4MVC_DetailsModel : AspNetSimple.Pages.Categories.DetailsModel
    {
        public R4MVC_DetailsModel(): base(Dummy.Instance)
        {
        }
    }
}
#pragma warning restore 1591, 3008, 3009, 0108
