using Microsoft.AspNetCore.Routing;

namespace Microsoft.AspNetCore.Mvc
{
    public static class T4Extensions
    {
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
