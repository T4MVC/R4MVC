using System;
using Microsoft.AspNetCore.Routing;

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
