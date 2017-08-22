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
