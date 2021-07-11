using Microsoft.AspNetCore.Routing;

namespace R4Mvc.ModelUnbinders
{
    public interface IModelUnbinder
    {
        void UnbindModel(RouteValueDictionary routeValueDictionary, string routeName, object routeValue);
    }

    public interface IModelUnbinder<in T> where T : class
    {
        void UnbindModel(RouteValueDictionary routeValueDictionary, string routeName, T routeValue);
    }
}
