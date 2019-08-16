using System.Threading.Tasks;
#if CORE2
using Microsoft.AspNetCore.Mvc.Infrastructure;
#endif

namespace Microsoft.AspNetCore.Mvc.Rendering
{
    public static class UrlHelperExtensions
    {
        public static string Action(this IUrlHelper urlHelper, IActionResult result, string protocol = null, string hostName = null, string fragment = null)
        {
            return urlHelper.Action(result.GetR4ActionResult(), protocol, hostName, fragment);
        }

        public static string Action<TAction>(this IUrlHelper urlHelper, Task<TAction> taskResult, string protocol = null, string hostName = null, string fragment = null)
            where TAction : IActionResult
        {
            return urlHelper.Action(taskResult.Result, protocol, hostName, fragment);
        }

        public static string Action(this IUrlHelper urlHelper, Task taskResult, string protocol = null, string hostName = null, string fragment = null)
        {
            return urlHelper.Action(taskResult.GetActionResult(), protocol, hostName, fragment);
        }

        public static string Action(this IUrlHelper urlHelper, IR4ActionResult result, string protocol = null, string hostName = null, string fragment = null)
        {
            return urlHelper.RouteUrl(null, result, protocol, hostName, fragment);
        }

#if CORE2
        public static string Action(this IUrlHelper urlHelper, IConvertToActionResult result, string protocol = null, string hostName = null, string fragment = null)
        {
            return urlHelper.Action(result.Convert(), protocol, hostName, fragment);
        }
#endif

        public static string ActionAbsolute(this IUrlHelper urlHelper, IActionResult result)
        {
            return urlHelper.ActionAbsolute(result.GetR4ActionResult());
        }

        public static string ActionAbsolute<TAction>(this IUrlHelper urlHelper, Task<TAction> taskResult)
            where TAction : IActionResult
        {
            return urlHelper.ActionAbsolute(taskResult.Result);
        }

        public static string ActionAbsolute(this IUrlHelper urlHelper, Task taskResult)
        {
            return urlHelper.ActionAbsolute(taskResult.GetActionResult());
        }

        public static string ActionAbsolute(this IUrlHelper urlHelper, IR4ActionResult result)
        {
            var request = urlHelper.ActionContext.HttpContext.Request;
            return $"{request.Scheme}://{request.Host}{urlHelper.RouteUrl(result.RouteValueDictionary)}";
        }

#if CORE2
        public static string ActionAbsolute(this IUrlHelper urlHelper, IConvertToActionResult result)
        {
            return urlHelper.ActionAbsolute(result.Convert());
        }
#endif

        public static string RouteUrl(this IUrlHelper urlHelper, IActionResult result)
        {
            return urlHelper.RouteUrl(null, result, null, null);
        }

        public static string RouteUrl(this IUrlHelper urlHelper, string routeName, IActionResult result, string protocol = null, string hostName = null, string fragment = null)
        {
            return urlHelper.RouteUrl(routeName, result.GetR4ActionResult(), protocol, hostName, fragment);
        }

        public static string RouteUrl<TAction>(this IUrlHelper urlHelper, Task<TAction> taskResult)
            where TAction : IActionResult
        {
            return urlHelper.RouteUrl(null, taskResult.Result, null, null);
        }

        public static string RouteUrl<TAction>(this IUrlHelper urlHelper, string routeName, Task<TAction> taskResult, string protocol = null, string hostName = null, string fragment = null)
            where TAction : IActionResult
        {
            return urlHelper.RouteUrl(routeName, taskResult.Result, protocol, hostName, fragment);
        }

        public static string RouteUrl<TAction>(this IUrlHelper urlHelper, Task taskResult)
        {
            return urlHelper.RouteUrl(null, taskResult.GetActionResult(), null, null);
        }

        public static string RouteUrl<TAction>(this IUrlHelper urlHelper, string routeName, Task taskResult, string protocol = null, string hostName = null, string fragment = null)
        {
            return urlHelper.RouteUrl(routeName, taskResult.GetActionResult(), protocol, hostName, fragment);
        }

        public static string RouteUrl(this IUrlHelper urlHelper, string routeName, IR4ActionResult result, string protocol = null, string hostName = null, string fragment = null)
        {
            return urlHelper.RouteUrl(routeName, result.RouteValueDictionary, protocol ?? result.Protocol, hostName, fragment);
        }

#if CORE2
        public static string RouteUrl<TAction>(this IUrlHelper urlHelper, IConvertToActionResult result)
        {
            return urlHelper.RouteUrl(null, result.Convert(), null, null);
        }

        public static string RouteUrl<TAction>(this IUrlHelper urlHelper, string routeName, IConvertToActionResult result, string protocol = null, string hostName = null, string fragment = null)
        {
            return urlHelper.RouteUrl(routeName, result.Convert(), protocol, hostName, fragment);
        }
#endif
    }
}
