using Microsoft.AspNetCore.Routing;

namespace R4Mvc.ModelUnbinders
{
    public class DefaultModelUnbinder : IModelUnbinder
    {
        public void UnbindModel(RouteValueDictionary routeValueDictionary, string routeName, object routeValue)
        {
            routeValueDictionary[routeName] = routeValue;
        }
    }
}
