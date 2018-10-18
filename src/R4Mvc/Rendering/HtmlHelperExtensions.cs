using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#if CORE2
using Microsoft.AspNetCore.Mvc.Infrastructure;
#endif

namespace Microsoft.AspNetCore.Mvc.Rendering
{
    public static class HtmlHelperExtensions
    {
        #region ActionLink
        public static IHtmlContent ActionLink(this IHtmlHelper htmlHelper, string linkText, IActionResult result, object htmlAttributes = null, string protocol = null, string hostName = null, string fragment = null)
        {
            return htmlHelper.ActionLink(linkText, result.GetR4ActionResult(), htmlAttributes, protocol, hostName, fragment);
        }

        public static IHtmlContent ActionLink(this IHtmlHelper htmlHelper, string linkText, IActionResult result, IDictionary<string, object> htmlAttributes, string protocol = null, string hostName = null, string fragment = null)
        {
            return htmlHelper.ActionLink(linkText, result.GetR4ActionResult(), htmlAttributes, protocol, hostName, fragment);
        }

        public static IHtmlContent ActionLink<TAction>(this IHtmlHelper htmlHelper, string linkText, Task<TAction> result, object htmlAttributes = null, string protocol = null, string hostName = null, string fragment = null)
            where TAction : IActionResult
        {
            return htmlHelper.ActionLink(linkText, result.Result, htmlAttributes, protocol, hostName, fragment);
        }

        public static IHtmlContent ActionLink<TAction>(this IHtmlHelper htmlHelper, string linkText, Task<TAction> result, IDictionary<string, object> htmlAttributes, string protocol = null, string hostName = null, string fragment = null)
            where TAction : IActionResult
        {
            return htmlHelper.ActionLink(linkText, result.Result, htmlAttributes, protocol, hostName, fragment);
        }

        public static IHtmlContent ActionLink(this IHtmlHelper htmlHelper, string linkText, Task result, object htmlAttributes = null, string protocol = null, string hostName = null, string fragment = null)
        {
            return htmlHelper.ActionLink(linkText, result.GetActionResult(), htmlAttributes, protocol, hostName, fragment);
        }

        public static IHtmlContent ActionLink(this IHtmlHelper htmlHelper, string linkText, Task result, IDictionary<string, object> htmlAttributes, string protocol = null, string hostName = null, string fragment = null)
        {
            return htmlHelper.ActionLink(linkText, result.GetActionResult(), htmlAttributes, protocol, hostName, fragment);
        }

        public static IHtmlContent ActionLink(this IHtmlHelper htmlHelper, string linkText, IR4ActionResult result, object htmlAttributes = null, string protocol = null, string hostName = null, string fragment = null)
        {
            return htmlHelper.RouteLink(linkText, null, result, htmlAttributes, protocol, hostName, fragment);
        }

        public static IHtmlContent ActionLink(this IHtmlHelper htmlHelper, string linkText, IR4ActionResult result, IDictionary<string, object> htmlAttributes, string protocol = null, string hostName = null, string fragment = null)
        {
            return htmlHelper.RouteLink(linkText, null, result, htmlAttributes, protocol, hostName, fragment);
        }

#if CORE2
        public static IHtmlContent ActionLink(this IHtmlHelper htmlHelper, string linkText, IConvertToActionResult result, object htmlAttributes = null, string protocol = null, string hostName = null, string fragment = null)
        {
            return htmlHelper.ActionLink(linkText, result.Convert(), htmlAttributes, protocol, hostName, fragment);
        }

        public static IHtmlContent ActionLink(this IHtmlHelper htmlHelper, string linkText, IConvertToActionResult result, IDictionary<string, object> htmlAttributes, string protocol = null, string hostName = null, string fragment = null)
        {
            return htmlHelper.ActionLink(linkText, result.Convert(), htmlAttributes, protocol, hostName, fragment);
        }
#endif
        #endregion

        #region RouteLink
        public static IHtmlContent RouteLink(this IHtmlHelper htmlHelper, string linkText, IActionResult result, object htmlAttributes)
        {
            return htmlHelper.RouteLink(linkText, null, result, htmlAttributes, null, null, null);
        }

        public static IHtmlContent RouteLink(this IHtmlHelper htmlHelper, string linkText, string routeName, IActionResult result, object htmlAttributes, string protocol = null, string hostName = null, string fragment = null)
        {
            return htmlHelper.RouteLink(linkText, routeName, result.GetR4ActionResult(), htmlAttributes, protocol, hostName, fragment);
        }

        public static IHtmlContent RouteLink(this IHtmlHelper htmlHelper, string linkText, IActionResult result, IDictionary<string, object> htmlAttributes)
        {
            return htmlHelper.RouteLink(linkText, null, result, htmlAttributes, null, null, null);
        }

        public static IHtmlContent RouteLink(this IHtmlHelper htmlHelper, string linkText, string routeName, IActionResult result, IDictionary<string, object> htmlAttributes, string protocol = null, string hostName = null, string fragment = null)
        {
            return htmlHelper.RouteLink(linkText, routeName, result.GetR4ActionResult(), htmlAttributes, protocol, hostName, fragment);
        }

        public static IHtmlContent RouteLink<TAction>(this IHtmlHelper htmlHelper, string linkText, Task<TAction> result, object htmlAttributes)
            where TAction : IActionResult
        {
            return htmlHelper.RouteLink(linkText, result.Result, htmlAttributes);
        }

        public static IHtmlContent RouteLink<TAction>(this IHtmlHelper htmlHelper, string linkText, string routeName, Task<TAction> result, object htmlAttributes, string protocol = null, string hostName = null, string fragment = null)
            where TAction : IActionResult
        {
            return htmlHelper.RouteLink(linkText, routeName, result.Result, htmlAttributes, protocol, hostName, fragment);
        }

        public static IHtmlContent RouteLink<TAction>(this IHtmlHelper htmlHelper, string linkText, Task<TAction> result, IDictionary<string, object> htmlAttributes)
            where TAction : IActionResult
        {
            return htmlHelper.RouteLink(linkText, result.Result, htmlAttributes);
        }

        public static IHtmlContent RouteLink<TAction>(this IHtmlHelper htmlHelper, string linkText, string routeName, Task<TAction> result, IDictionary<string, object> htmlAttributes, string protocol = null, string hostName = null, string fragment = null)
            where TAction : IActionResult
        {
            return htmlHelper.RouteLink(linkText, routeName, result.Result, htmlAttributes, protocol, hostName, fragment);
        }

        public static IHtmlContent RouteLink(this IHtmlHelper htmlHelper, string linkText, Task result, object htmlAttributes)
        {
            return htmlHelper.RouteLink(linkText, result.GetActionResult(), htmlAttributes);
        }

        public static IHtmlContent RouteLink(this IHtmlHelper htmlHelper, string linkText, string routeName, Task result, object htmlAttributes, string protocol = null, string hostName = null, string fragment = null)
        {
            return htmlHelper.RouteLink(linkText, routeName, result.GetActionResult(), htmlAttributes, protocol, hostName, fragment);
        }

        public static IHtmlContent RouteLink(this IHtmlHelper htmlHelper, string linkText, Task result, IDictionary<string, object> htmlAttributes)
        {
            return htmlHelper.RouteLink(linkText, result.GetActionResult(), htmlAttributes);
        }

        public static IHtmlContent RouteLink(this IHtmlHelper htmlHelper, string linkText, string routeName, Task result, IDictionary<string, object> htmlAttributes, string protocol = null, string hostName = null, string fragment = null)
        {
            return htmlHelper.RouteLink(linkText, routeName, result.GetActionResult(), htmlAttributes, protocol, hostName, fragment);
        }

        public static IHtmlContent RouteLink(this IHtmlHelper htmlHelper, string linkText, string routeName, IR4ActionResult result, object htmlAttributes, string protocol = null, string hostName = null, string fragment = null)
        {
            return htmlHelper.RouteLink(linkText, routeName, result, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes), protocol, hostName, fragment);
        }

        public static IHtmlContent RouteLink(this IHtmlHelper htmlHelper, string linkText, string routeName, IR4ActionResult result, IDictionary<string, object> htmlAttributes, string protocol = null, string hostName = null, string fragment = null)
        {
            return htmlHelper.RouteLink(linkText, routeName, protocol ?? result.Protocol, hostName, fragment, result.RouteValueDictionary, htmlAttributes);
        }

#if CORE2
        public static IHtmlContent RouteLink(this IHtmlHelper htmlHelper, string linkText, IConvertToActionResult result, object htmlAttributes)
        {
            return htmlHelper.RouteLink(linkText, result.Convert(), htmlAttributes);
        }

        public static IHtmlContent RouteLink(this IHtmlHelper htmlHelper, string linkText, string routeName, IConvertToActionResult result, object htmlAttributes, string protocol = null, string hostName = null, string fragment = null)
        {
            return htmlHelper.RouteLink(linkText, routeName, result.Convert(), htmlAttributes, protocol, hostName, fragment);
        }

        public static IHtmlContent RouteLink(this IHtmlHelper htmlHelper, string linkText, IConvertToActionResult result, IDictionary<string, object> htmlAttributes)
        {
            return htmlHelper.RouteLink(linkText, result.Convert(), htmlAttributes);
        }

        public static IHtmlContent RouteLink(this IHtmlHelper htmlHelper, string linkText, string routeName, IConvertToActionResult result, IDictionary<string, object> htmlAttributes, string protocol = null, string hostName = null, string fragment = null)
        {
            return htmlHelper.RouteLink(linkText, routeName, result.Convert(), htmlAttributes, protocol, hostName, fragment);
        }
#endif
        #endregion

        #region AutoNamedRouteLink
        public static IHtmlContent AutoNamedRouteLink(this IHtmlHelper htmlHelper, string linkText, IActionResult result, object htmlAttributes = null, string protocol = null, string hostName = null, string fragment = null)
        {
            return htmlHelper.AutoNamedRouteLink(linkText, result, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes), protocol, hostName, fragment);
        }

        public static IHtmlContent AutoNamedRouteLink(this IHtmlHelper htmlHelper, string linkText, IActionResult result, IDictionary<string, object> htmlAttributes, string protocol = null, string hostName = null, string fragment = null)
        {
            string routeName = autoRouteNameFromActionResult(result);
            return htmlHelper.RouteLink(linkText, routeName, protocol ?? result.GetR4ActionResult().Protocol, hostName, fragment, result.GetRouteValueDictionary(), htmlAttributes);
        }

        public static IHtmlContent AutoNamedRouteLink<TAction>(this IHtmlHelper htmlHelper, string linkText, Task<TAction> result, object htmlAttributes = null, string protocol = null, string hostName = null, string fragment = null)
            where TAction : IActionResult
        {
            return htmlHelper.AutoNamedRouteLink(linkText, result.Result, htmlAttributes, protocol, hostName, fragment);
        }

        public static IHtmlContent AutoNamedRouteLink<TAction>(this IHtmlHelper htmlHelper, string linkText, Task<TAction> result, IDictionary<string, object> htmlAttributes, string protocol = null, string hostName = null, string fragment = null)
            where TAction : IActionResult
        {
            return htmlHelper.AutoNamedRouteLink(linkText, result.Result, htmlAttributes, protocol, hostName, fragment);
        }

        public static IHtmlContent AutoNamedRouteLink(this IHtmlHelper htmlHelper, string linkText, Task result, object htmlAttributes = null, string protocol = null, string hostName = null, string fragment = null)
        {
            return htmlHelper.AutoNamedRouteLink(linkText, result.GetActionResult(), htmlAttributes, protocol, hostName, fragment);
        }

        public static IHtmlContent AutoNamedRouteLink(this IHtmlHelper htmlHelper, string linkText, Task result, IDictionary<string, object> htmlAttributes, string protocol = null, string hostName = null, string fragment = null)
        {
            return htmlHelper.AutoNamedRouteLink(linkText, result.GetActionResult(), htmlAttributes, protocol, hostName, fragment);
        }

#if CORE2
        public static IHtmlContent AutoNamedRouteLink(this IHtmlHelper htmlHelper, string linkText, IConvertToActionResult result, object htmlAttributes = null, string protocol = null, string hostName = null, string fragment = null)
        {
            return htmlHelper.AutoNamedRouteLink(linkText, result.Convert(), htmlAttributes, protocol, hostName, fragment);
        }

        public static IHtmlContent AutoNamedRouteLink(this IHtmlHelper htmlHelper, string linkText, IConvertToActionResult result, IDictionary<string, object> htmlAttributes, string protocol = null, string hostName = null, string fragment = null)
        {
            return htmlHelper.AutoNamedRouteLink(linkText, result.Convert(), htmlAttributes, protocol, hostName, fragment);
        }
#endif
        #endregion

        #region BeginForm
        public static MvcForm BeginForm(this IHtmlHelper htmlHelper, IActionResult result, FormMethod formMethod, object htmlAttributes)
        {
            return BeginForm(htmlHelper, result, formMethod, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcForm BeginForm(this IHtmlHelper htmlHelper, IActionResult result, FormMethod formMethod = FormMethod.Post, IDictionary<string, object> htmlAttributes = null)
        {
            return BeginForm(htmlHelper, result.GetR4ActionResult(), formMethod, htmlAttributes);
        }

        public static MvcForm BeginForm<TAction>(this IHtmlHelper htmlHelper, Task<TAction> result, FormMethod formMethod, object htmlAttributes)
            where TAction : IActionResult
        {
            return BeginForm(htmlHelper, result.Result, formMethod, htmlAttributes);
        }

        public static MvcForm BeginForm<TAction>(this IHtmlHelper htmlHelper, Task<TAction> result, FormMethod formMethod = FormMethod.Post, IDictionary<string, object> htmlAttributes = null)
            where TAction : IActionResult
        {
            return BeginForm(htmlHelper, result.Result, formMethod, htmlAttributes);
        }

        public static MvcForm BeginForm(this IHtmlHelper htmlHelper, Task result, FormMethod formMethod, object htmlAttributes)
        {
            return BeginForm(htmlHelper, result.GetActionResult(), formMethod, htmlAttributes);
        }

        public static MvcForm BeginForm(this IHtmlHelper htmlHelper, Task result, FormMethod formMethod = FormMethod.Post, IDictionary<string, object> htmlAttributes = null)
        {
            return BeginForm(htmlHelper, result.GetActionResult(), formMethod, htmlAttributes);
        }

        public static MvcForm BeginForm(this IHtmlHelper htmlHelper, IR4ActionResult result, FormMethod formMethod = FormMethod.Post, IDictionary<string, object> htmlAttributes = null)
        {
            return htmlHelper.BeginRouteForm(null, result, formMethod, htmlAttributes);
        }

#if CORE2
        public static MvcForm BeginForm(this IHtmlHelper htmlHelper, IConvertToActionResult result, FormMethod formMethod, object htmlAttributes)
        {
            return BeginForm(htmlHelper, result.Convert(), formMethod, htmlAttributes);
        }

        public static MvcForm BeginForm(this IHtmlHelper htmlHelper, IConvertToActionResult result, FormMethod formMethod = FormMethod.Post, IDictionary<string, object> htmlAttributes = null)
        {
            return BeginForm(htmlHelper, result.Convert(), formMethod, htmlAttributes);
        }
#endif
        #endregion

        #region BeginRouteForm
        public static MvcForm BeginRouteForm(this IHtmlHelper htmlHelper, IActionResult result)
        {
            return htmlHelper.BeginRouteForm(null, result, FormMethod.Post, null);
        }

        public static MvcForm BeginRouteForm(this IHtmlHelper htmlHelper, string routeName, IActionResult result, FormMethod formMethod, object htmlAttributes)
        {
            return htmlHelper.BeginRouteForm(routeName, result, formMethod, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcForm BeginRouteForm(this IHtmlHelper htmlHelper, string routeName, IActionResult result, FormMethod formMethod = FormMethod.Post, IDictionary<string, object> htmlAttributes = null)
        {
            return htmlHelper.BeginRouteForm(routeName, result.GetR4ActionResult(), formMethod, htmlAttributes);
        }

        public static MvcForm BeginRouteForm<TAction>(this IHtmlHelper htmlHelper, Task<TAction> result)
            where TAction : IActionResult
        {
            return htmlHelper.BeginRouteForm(result.Result);
        }

        public static MvcForm BeginRouteForm<TAction>(this IHtmlHelper htmlHelper, string routeName, Task<TAction> result, FormMethod formMethod, object htmlAttributes)
            where TAction : IActionResult
        {
            return htmlHelper.BeginRouteForm(routeName, result.Result, formMethod, htmlAttributes);
        }

        public static MvcForm BeginRouteForm<TAction>(this IHtmlHelper htmlHelper, string routeName, Task<TAction> result, FormMethod formMethod = FormMethod.Post, IDictionary<string, object> htmlAttributes = null)
            where TAction : IActionResult
        {
            return htmlHelper.BeginRouteForm(routeName, result.Result, formMethod, htmlAttributes);
        }

        public static MvcForm BeginRouteForm(this IHtmlHelper htmlHelper, Task result)
        {
            return htmlHelper.BeginRouteForm(result.GetActionResult());
        }

        public static MvcForm BeginRouteForm(this IHtmlHelper htmlHelper, string routeName, Task result, FormMethod formMethod, object htmlAttributes)
        {
            return htmlHelper.BeginRouteForm(routeName, result.GetActionResult(), formMethod, htmlAttributes);
        }

        public static MvcForm BeginRouteForm(this IHtmlHelper htmlHelper, string routeName, Task result, FormMethod formMethod = FormMethod.Post, IDictionary<string, object> htmlAttributes = null)
        {
            return htmlHelper.BeginRouteForm(routeName, result.GetActionResult(), formMethod, htmlAttributes);
        }

        public static MvcForm BeginRouteForm(this IHtmlHelper htmlHelper, string routeName, IR4ActionResult result, FormMethod formMethod = FormMethod.Post, IDictionary<string, object> htmlAttributes = null)
        {
            return htmlHelper.BeginRouteForm(routeName, result.RouteValueDictionary, formMethod, null, htmlAttributes);
        }

#if CORE2
        public static MvcForm BeginRouteForm(this IHtmlHelper htmlHelper, IConvertToActionResult result)
        {
            return htmlHelper.BeginRouteForm(result.Convert());
        }

        public static MvcForm BeginRouteForm(this IHtmlHelper htmlHelper, string routeName, IConvertToActionResult result, FormMethod formMethod, object htmlAttributes)
        {
            return htmlHelper.BeginRouteForm(routeName, result.Convert(), formMethod, htmlAttributes);
        }

        public static MvcForm BeginRouteForm(this IHtmlHelper htmlHelper, string routeName, IConvertToActionResult result, FormMethod formMethod = FormMethod.Post, IDictionary<string, object> htmlAttributes = null)
        {
            return htmlHelper.BeginRouteForm(routeName, result.Convert(), formMethod, htmlAttributes);
        }
#endif
        #endregion

        private static string autoRouteNameFromActionResult(IActionResult result)
        {
            if (result is IR4MvcActionResult r4mvcRes)
            {
                string actionName = r4mvcRes.Action;
                string ctrlName = r4mvcRes.Controller;
                // get area from route values
                r4mvcRes.RouteValueDictionary.TryGetValue("area", out object areaName);
                r4mvcRes.RouteValueDictionary.Remove("area");
                // compose route name
                string routeName = ComposeAutoRouteName(areaName as string, ctrlName, actionName);
                return routeName;
            }
            else if (result is IR4PageActionResult r4pageRes)
            {
                string pageName = r4pageRes.PageName;
                // compose route name
                string routeName = ComposeAutoRouteName(pageName);
                return routeName;
            }
            else
            {
                // fall back to defaults
                return null;
            }
        }

        public static string ComposeAutoRouteName(string areaName, string controllerName, string actionName)
        {
            if (controllerName == null)
                throw new ArgumentNullException(nameof(controllerName), "Controller name cannot be null");
            if (actionName == null)
                throw new ArgumentNullException(nameof(actionName), "Action name cannot be null");

            if (string.IsNullOrWhiteSpace(areaName))
                areaName = "__AUTONAMEDROUTE_DEFAULT__";

            return string.Join("_", areaName, controllerName, actionName).ToLowerInvariant();
        }

        public static string ComposeAutoRouteName(string pageName)
        {
            if (pageName == null)
                throw new ArgumentNullException(nameof(pageName), "Page name cannot be null");

            return pageName.Replace('/', '_').ToLowerInvariant();
        }
    }
}
