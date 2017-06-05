using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Mvc.Rendering
{
    public static class HtmlHelperExtensions
    {
        #region ActionLink
        public static IHtmlContent ActionLink(this IHtmlHelper htmlHelper, string linkText, IActionResult result, object htmlAttributes = null, string protocol = null, string hostName = null, string fragment = null)
        {
            return htmlHelper.RouteLink(linkText, null, protocol ?? result.GetR4MvcResult().Protocol, hostName, fragment, result.GetRouteValueDictionary(), HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static IHtmlContent ActionLink(this IHtmlHelper htmlHelper, string linkText, IActionResult result, IDictionary<string, object> htmlAttributes, string protocol = null, string hostName = null, string fragment = null)
        {
            return htmlHelper.RouteLink(linkText, null, protocol ?? result.GetR4MvcResult().Protocol, hostName, fragment, result.GetRouteValueDictionary(), htmlAttributes);
        }

        public static IHtmlContent ActionLink(this IHtmlHelper htmlHelper, string linkText, Task<IActionResult> result, object htmlAttributes = null, string protocol = null, string hostName = null, string fragment = null)
        {
            return htmlHelper.ActionLink(linkText, result.Result, htmlAttributes, protocol, hostName, fragment);
        }

        public static IHtmlContent ActionLink(this IHtmlHelper htmlHelper, string linkText, Task<IActionResult> result, IDictionary<string, object> htmlAttributes, string protocol = null, string hostName = null, string fragment = null)
        {
            return htmlHelper.ActionLink(linkText, result.Result, htmlAttributes, protocol, hostName, fragment);
        }
        #endregion

        #region RouteLink
        public static IHtmlContent RouteLink(this IHtmlHelper htmlHelper, string linkText, IActionResult result, object htmlAttributes)
        {
            return htmlHelper.RouteLink(linkText, null, result, htmlAttributes, null, null, null);
        }

        public static IHtmlContent RouteLink(this IHtmlHelper htmlHelper, string linkText, string routeName, IActionResult result, object htmlAttributes, string protocol = null, string hostName = null, string fragment = null)
        {
            return htmlHelper.RouteLink(linkText, routeName, result, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes), protocol, hostName, fragment);
        }

        public static IHtmlContent RouteLink(this IHtmlHelper htmlHelper, string linkText, IActionResult result, IDictionary<string, object> htmlAttributes)
        {
            return htmlHelper.RouteLink(linkText, null, result, htmlAttributes, null, null, null);
        }

        public static IHtmlContent RouteLink(this IHtmlHelper htmlHelper, string linkText, string routeName, IActionResult result, IDictionary<string, object> htmlAttributes, string protocol = null, string hostName = null, string fragment = null)
        {
            return htmlHelper.RouteLink(linkText, routeName, protocol ?? result.GetR4MvcResult().Protocol, hostName, fragment, result.GetRouteValueDictionary(), htmlAttributes);
        }

        public static IHtmlContent RouteLink(this IHtmlHelper htmlHelper, string linkText, Task<IActionResult> result, object htmlAttributes)
        {
            return htmlHelper.RouteLink(linkText, result.Result, htmlAttributes);
        }

        public static IHtmlContent RouteLink(this IHtmlHelper htmlHelper, string linkText, string routeName, Task<IActionResult> result, object htmlAttributes, string protocol = null, string hostName = null, string fragment = null)
        {
            return htmlHelper.RouteLink(linkText, routeName, result.Result, htmlAttributes, protocol, hostName, fragment);
        }

        public static IHtmlContent RouteLink(this IHtmlHelper htmlHelper, string linkText, Task<IActionResult> result, IDictionary<string, object> htmlAttributes)
        {
            return htmlHelper.RouteLink(linkText, result.Result, htmlAttributes);
        }

        public static IHtmlContent RouteLink(this IHtmlHelper htmlHelper, string linkText, string routeName, Task<IActionResult> result, IDictionary<string, object> htmlAttributes, string protocol = null, string hostName = null, string fragment = null)
        {
            return htmlHelper.RouteLink(linkText, routeName, result.Result, htmlAttributes, protocol, hostName, fragment);
        }
        #endregion

        #region AutoNamedRouteLink
        public static IHtmlContent AutoNamedRouteLink(this IHtmlHelper htmlHelper, string linkText, IActionResult result, object htmlAttributes = null, string protocol = null, string hostName = null, string fragment = null)
        {
            return htmlHelper.AutoNamedRouteLink(linkText, result, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes), protocol, hostName, fragment);
        }

        public static IHtmlContent AutoNamedRouteLink(this IHtmlHelper htmlHelper, string linkText, IActionResult result, IDictionary<string, object> htmlAttributes, string protocol = null, string hostName = null, string fragment = null)
        {
            string routeName = autoRouteNameFromActionResult(result);
            return htmlHelper.RouteLink(linkText, routeName, protocol ?? result.GetR4MvcResult().Protocol, hostName, fragment, result.GetRouteValueDictionary(), htmlAttributes);
        }

        public static IHtmlContent AutoNamedRouteLink(this IHtmlHelper htmlHelper, string linkText, Task<IActionResult> result, object htmlAttributes = null, string protocol = null, string hostName = null, string fragment = null)
        {
            return htmlHelper.AutoNamedRouteLink(linkText, result.Result, htmlAttributes, protocol, hostName, fragment);
        }

        public static IHtmlContent AutoNamedRouteLink(this IHtmlHelper htmlHelper, string linkText, Task<IActionResult> result, IDictionary<string, object> htmlAttributes, string protocol = null, string hostName = null, string fragment = null)
        {
            return htmlHelper.AutoNamedRouteLink(linkText, result.Result, htmlAttributes, protocol, hostName, fragment);
        }
        #endregion

        #region BeginForm
        public static MvcForm BeginForm(this IHtmlHelper htmlHelper, IActionResult result, FormMethod formMethod, object htmlAttributes)
        {
            return BeginForm(htmlHelper, result, formMethod, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcForm BeginForm(this IHtmlHelper htmlHelper, IActionResult result, FormMethod formMethod = FormMethod.Post, IDictionary<string, object> htmlAttributes = null)
        {
            var callInfo = result.GetR4MvcResult();
            return htmlHelper.BeginForm(callInfo.Action, callInfo.Controller, callInfo.RouteValueDictionary, formMethod, null, htmlAttributes);
        }

        public static MvcForm BeginForm(this IHtmlHelper htmlHelper, Task<IActionResult> result, FormMethod formMethod, object htmlAttributes)
        {
            return BeginForm(htmlHelper, result.Result, formMethod, htmlAttributes);
        }

        public static MvcForm BeginForm(this IHtmlHelper htmlHelper, Task<IActionResult> result, FormMethod formMethod = FormMethod.Post, IDictionary<string, object> htmlAttributes = null)
        {
            return BeginForm(htmlHelper, result.Result, formMethod, htmlAttributes);
        }
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
            return htmlHelper.BeginRouteForm(routeName, result.GetRouteValueDictionary(), formMethod, null, htmlAttributes);
        }

        public static MvcForm BeginRouteForm(this IHtmlHelper htmlHelper, Task<IActionResult> result)
        {
            return htmlHelper.BeginRouteForm(result.Result);
        }

        public static MvcForm BeginRouteForm(this IHtmlHelper htmlHelper, string routeName, Task<IActionResult> result, FormMethod formMethod, object htmlAttributes)
        {
            return htmlHelper.BeginRouteForm(routeName, result.Result, formMethod, htmlAttributes);
        }

        public static MvcForm BeginRouteForm(this IHtmlHelper htmlHelper, string routeName, Task<IActionResult> result, FormMethod formMethod = FormMethod.Post, IDictionary<string, object> htmlAttributes = null)
        {
            return htmlHelper.BeginRouteForm(routeName, result.Result, formMethod, htmlAttributes);
        }
        #endregion

        private static string autoRouteNameFromActionResult(IActionResult result)
        {
            var t4mvcRes = result.GetR4MvcResult();
            string actionName = t4mvcRes.Action;
            string ctrlName = t4mvcRes.Controller;
            // get area from route values
            object areaName = "";
            t4mvcRes.RouteValueDictionary.TryGetValue("area", out areaName);
            t4mvcRes.RouteValueDictionary.Remove("area");
            // compose route name
            string routeName = ComposeAutoRouteName(areaName as string, ctrlName, actionName);
            return routeName;
        }

        public static string ComposeAutoRouteName(string areaName, string controllerName, string actionName)
        {
            if (controllerName == null)
                throw new ArgumentNullException("controllerName", "Controller name cannot be null");
            if (actionName == null)
                throw new ArgumentNullException("actionName", "Action name cannot be null");

            if (string.IsNullOrWhiteSpace(areaName))
                areaName = "__AUTONAMEDROUTE_DEFAULT__";

            return string.Join("_", areaName, controllerName, actionName).ToLowerInvariant();
        }
    }
}
