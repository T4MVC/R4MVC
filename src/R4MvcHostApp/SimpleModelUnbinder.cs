using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using R4Mvc.ModelUnbinders;

namespace R4MvcHostApp
{
    public class SimplePropertyModelUnbinder : IModelUnbinder
    {
        public virtual void UnbindModel(RouteValueDictionary routeValueDictionary, string routeName, object routeValue)
        {
            var dict = new RouteValueDictionary(routeValue);
            foreach (var entry in dict)
            {
                var name = entry.Key;

                if (!(entry.Value is string) && (entry.Value is System.Collections.IEnumerable))
                {
                    if (IncludeProperty(routeValue, entry))
                    {
                        var enumerableValue = (System.Collections.IEnumerable)entry.Value;
                        var i = 0;
                        foreach (var enumerableElement in enumerableValue)
                        {
                            ModelUnbinderHelpers.AddRouteValues(routeValueDictionary, string.Format("{0}", name), enumerableElement);
                            i++;
                        }
                    }
                }
                else
                {
                    ModelUnbinderHelpers.AddRouteValues(routeValueDictionary, name, entry.Value);
                }
            }
        }

        bool IncludeProperty(object routeValue, KeyValuePair<string, object> entry)
        {
            var includeAttributes = routeValue.GetType().GetProperty(entry.Key).GetCustomAttributes(typeof(IncludeAttribute), true);
            return includeAttributes.Any();
        }
    }

    public class IncludeAttribute : Attribute
    {

    }
}
