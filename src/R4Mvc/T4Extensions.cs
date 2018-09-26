using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;

namespace Microsoft.AspNetCore.Mvc
{
    public static class T4Extensions
    {
        public static IR4ActionResult GetR4ActionResult(this IActionResult result)
        {
            if (!(result is IR4ActionResult actionResult))
                throw new InvalidOperationException("R4MVC was called incorrectly. You may need to force it to regenerate by running `dotnet r4mvc`");
            return actionResult;
        }

        public static IR4ActionResult GetR4ActionResult<TActionResult>(this Task<TActionResult> taskResult)
            where TActionResult : IActionResult
        {
            return GetR4ActionResult(taskResult.Result);
        }

        public static IR4ActionResult GetR4ActionResult(this Task taskResult)
        {
            return GetR4ActionResult(GetActionResult(taskResult));
        }

        [Obsolete("Please use GetR4ActionResult instead")]
        public static IR4MvcActionResult GetR4MvcResult(this IActionResult result)
        {
            if (!(result is IR4MvcActionResult actionResult))
                throw new InvalidOperationException("R4MVC was called incorrectly. You may need to force it to regenerate by running `dotnet r4mvc`");
            return actionResult;
        }

        [Obsolete("Please use GetR4ActionResult instead")]
        public static IR4MvcActionResult GetR4MvcResult<TActionResult>(this Task<TActionResult> taskResult)
            where TActionResult : IActionResult
        {
            return GetR4MvcResult(taskResult.Result);
        }

        [Obsolete("Please use GetR4ActionResult instead")]
        public static IR4MvcActionResult GetR4MvcResult(this Task taskResult)
        {
            return GetR4MvcResult(GetActionResult(taskResult));
        }

        internal static IActionResult GetActionResult(this Task task)
        {
            switch (task)
            {
                case Task<IActionResult> taskIActionResult:
                    return taskIActionResult.Result;
                case Task<ActionResult> taskIActionResult:
                    return taskIActionResult.Result;
                case Task<ContentResult> taskIActionResult:
                    return taskIActionResult.Result;
                case Task<FileResult> taskIActionResult:
                    return taskIActionResult.Result;
                case Task<FileStreamResult> taskIActionResult:
                    return taskIActionResult.Result;
                case Task<PhysicalFileResult> taskIActionResult:
                    return taskIActionResult.Result;
                case Task<VirtualFileResult> taskIActionResult:
                    return taskIActionResult.Result;
                case Task<JsonResult> taskIActionResult:
                    return taskIActionResult.Result;
                case Task<RedirectResult> taskIActionResult:
                    return taskIActionResult.Result;
                case Task<RedirectToActionResult> taskIActionResult:
                    return taskIActionResult.Result;
                case Task<RedirectToRouteResult> taskIActionResult:
                    return taskIActionResult.Result;
                case Task<LocalRedirectResult> taskIActionResult:
                    return taskIActionResult.Result;
                default:
                    if (task.GetType().GetTypeInfo().IsGenericType)
                    {
                        var resultProp = task.GetType().GetTypeInfo().GetProperty("Result");
                        if (resultProp != null)
                        {
                            var result = resultProp.GetValue(task);
                            if (result is IActionResult actionResult)
                                return actionResult;
                        }
                    }
                    return null;
            }
        }

        public static RouteValueDictionary GetRouteValueDictionary(this IActionResult result)
        {
            return result.GetR4ActionResult().RouteValueDictionary;
        }

        public static void InitMVCT4Result(this IR4MvcActionResult result, string area, string controller, string action, string protocol = null)
        {
            result.Controller = controller;
            result.Action = action;
            result.Protocol = protocol;
            result.RouteValueDictionary = new RouteValueDictionary
            {
                { "Area", area ?? string.Empty },
                { "Controller", controller },
                { "Action", action }
            };
        }

        public static void InitMVCT4Result(this IR4PageActionResult result, string pageName, string pageHandler, string protocol = null)
        {
            result.PageName = pageName;
            result.PageHandler = pageHandler;
            result.Protocol = protocol;
            result.RouteValueDictionary = new RouteValueDictionary
            {
                { "Page", pageName },
                { "Handler", pageHandler },
            };
        }

        public static IActionResult AddRouteValues(this IActionResult result, object routeValues)
        {
            return result.AddRouteValues(new RouteValueDictionary(routeValues));
        }

        public static IActionResult AddRouteValues(this IActionResult result, RouteValueDictionary routeValues)
        {
            RouteValueDictionary currentRouteValues = result.GetRouteValueDictionary();

            // Add all the extra values
            foreach (var pair in routeValues)
            {
                ModelUnbinderHelpers.AddRouteValues(currentRouteValues, pair.Key, pair.Value);
            }

            return result;
        }

        //public static IActionResult AddRouteValues(this IActionResult result, NameValueCollection nameValueCollection)
        //{
        //    // Copy all the values from the NameValueCollection into the route dictionary
        //    if (nameValueCollection.AllKeys.Any(m => m == null))  //if it has a null, the CopyTo extension will crash!
        //    {
        //        var filtered = new NameValueCollection(nameValueCollection);
        //        filtered.Remove(null);
        //        filtered.CopyTo(result.GetRouteValueDictionary(), 0);
        //    }
        //    else
        //    {
        //        nameValueCollection.CopyTo(result.GetRouteValueDictionary(), replaceEntries: true);
        //    }
        //    return result;
        //}

        public static IActionResult AddRouteValue(this IActionResult result, string name, object value)
        {
            RouteValueDictionary routeValues = result.GetRouteValueDictionary();
            ModelUnbinderHelpers.AddRouteValues(routeValues, name, value);
            return result;
        }
    }
}
