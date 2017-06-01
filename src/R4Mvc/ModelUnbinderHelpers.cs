using Microsoft.AspNetCore.Routing;

namespace Microsoft.AspNetCore.Mvc
{
    public class ModelUnbinderHelpers
    {
        public static void AddRouteValues(RouteValueDictionary routeValueDictionary, string routeName, object routeValue)
        {
            routeValueDictionary.Add(routeName, routeValue);
        }
    }
}
