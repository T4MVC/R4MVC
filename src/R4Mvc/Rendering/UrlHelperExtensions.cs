using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Mvc.Rendering
{
    public static class UrlHelperExtensions
    {
        public static string Action(this IUrlHelper urlHelper, IActionResult result, string protocol = null, string hostName = null, string fragment = null)
        {
            return urlHelper.RouteUrl(null, result.GetRouteValueDictionary(), protocol ?? result.GetR4MvcResult().Protocol, hostName, fragment);
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

        public static string ActionAbsolute(this IUrlHelper urlHelper, IActionResult result)
        {
            var request = urlHelper.ActionContext.HttpContext.Request;
            return $"{request.Scheme}://{request.Host}{urlHelper.RouteUrl(result.GetRouteValueDictionary())}";
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

        public static string RouteUrl(this IUrlHelper urlHelper, IActionResult result)
        {
            return urlHelper.RouteUrl(null, result, null, null);
        }

        public static string RouteUrl(this IUrlHelper urlHelper, string routeName, IActionResult result, string protocol = null, string hostName = null, string fragment = null)
        {
            return urlHelper.RouteUrl(routeName, result.GetRouteValueDictionary(), protocol ?? result.GetR4MvcResult().Protocol, hostName, fragment);
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
    }
}
