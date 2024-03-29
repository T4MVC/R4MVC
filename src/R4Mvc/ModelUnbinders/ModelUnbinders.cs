﻿using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Routing;

namespace R4Mvc.ModelUnbinders
{
    public class ModelUnbinders
    {
        private class GenericModelUnbinderWrapper<T> : IModelUnbinder where T : class
        {
            private readonly IModelUnbinder<T> _unbinder;

            public GenericModelUnbinderWrapper(IModelUnbinder<T> unbinder)
            {
                _unbinder = unbinder;
            }

            public void UnbindModel(RouteValueDictionary routeValueDictionary, string routeName, object routeValue)
            {
                var typedObject = routeValue as T;
                _unbinder.UnbindModel(routeValueDictionary, routeName, typedObject);
            }
        }

        private readonly Dictionary<Type, IModelUnbinder> _unbinders = new Dictionary<Type, IModelUnbinder>();
        public virtual void Add(Type type, IModelUnbinder unbinder)
        {
            _unbinders[type] = unbinder;
        }
        public virtual void Add<T>(IModelUnbinder<T> unbinder) where T : class
        {
            Add(typeof(T), new GenericModelUnbinderWrapper<T>(unbinder));
        }
        public virtual IModelUnbinder FindUnbinderFor(Type type)
        {
            IModelUnbinder resultUnbinder = null;
            Type baseType = null;
            foreach (var unbinder in _unbinders)
            {
                if (unbinder.Key.IsAssignableFrom(type))
                {
                    if ((baseType == null) || baseType.IsAssignableFrom(unbinder.Key))
                    {
                        resultUnbinder = unbinder.Value;
                        baseType = unbinder.Key;
                    }
                }
            }
            return resultUnbinder;
        }

        public virtual void Clear()
        {
            _unbinders.Clear();
        }
    }
}
