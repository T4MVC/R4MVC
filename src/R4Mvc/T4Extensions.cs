using System;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace Microsoft.AspNetCore.Mvc
{
    public static class T4Extensions
    {
        public static IR4MvcActionResult GetR4MvcResult(this IActionResult result)
        {
            var actionResult = result as IR4MvcActionResult;
            if (actionResult == null)
                throw new InvalidOperationException("R4MVC was called incorrectly. You may need to force it to regenerate by running `dotnet r4mvc`");
            return actionResult;
        }

        public static RouteValueDictionary GetRouteValueDictionary(this IActionResult result)
        {
            return result.GetR4MvcResult().RouteValueDictionary;
        }

        public static void InitMVCT4Result(this IR4MvcActionResult result, string area, string controller, string action, string protocol = null)
        {
            result.Controller = controller;
            result.Action = action;
            result.Protocol = protocol;
            result.RouteValueDictionary = new RouteValueDictionary()
            {
                { "Area", area ?? string.Empty },
                { "Controller", controller },
                { "Action", action }
            };
        }
    }
}
