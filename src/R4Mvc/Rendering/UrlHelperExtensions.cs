using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Mvc.Rendering
{
    public static class UrlHelperExtensions
    {
        public static string Action(this IUrlHelper urlHelper, IActionResult result, string protocol = null, string hostName = null, string fragment = null)
        {
            return urlHelper.RouteUrl(null, result.GetRouteValueDictionary(), protocol ?? result.GetR4MvcResult().Protocol, hostName, fragment);
        }

        public static string Action(this IUrlHelper urlHelper, Task<IActionResult> taskResult, string protocol = null, string hostName = null, string fragment = null)
        {
            return urlHelper.Action(taskResult.Result, protocol, hostName, fragment);
        }

        public static string ActionAbsolute(this IUrlHelper urlHelper, IActionResult result)
        {
            var request = urlHelper.ActionContext.HttpContext.Request;
            return $"{request.Scheme}://{request.Host}{urlHelper.RouteUrl(result.GetRouteValueDictionary())}";
        }

        public static string ActionAbsolute(this IUrlHelper urlHelper, Task<IActionResult> taskResult)
        {
            return urlHelper.ActionAbsolute(taskResult.Result);
        }

        public static string RouteUrl(this IUrlHelper urlHelper, IActionResult result)
        {
            return urlHelper.RouteUrl(null, result, null, null);
        }

        public static string RouteUrl(this IUrlHelper urlHelper, string routeName, IActionResult result, string protocol = null, string hostName = null, string fragment = null)
        {
            return urlHelper.RouteUrl(routeName, result.GetRouteValueDictionary(), protocol ?? result.GetR4MvcResult().Protocol, hostName, fragment);
        }

        public static string RouteUrl(this IUrlHelper urlHelper, Task<IActionResult> taskResult)
        {
            return urlHelper.RouteUrl(null, taskResult.Result, null, null);
        }

        public static string RouteUrl(this IUrlHelper urlHelper, string routeName, Task<IActionResult> taskResult, string protocol = null, string hostName = null, string fragment = null)
        {
            return urlHelper.RouteUrl(routeName, taskResult.Result, protocol, hostName, fragment);
        }
    }
}
