using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace R4Mvc
{
	public class R4MvcPluginProvider
	{
		private readonly Dictionary<Type,IR4MvcPlugin> _plugins = new Dictionary<Type, IR4MvcPlugin>();

		internal T Get<T>() where T : IR4MvcPlugin
		{
			// looks for an existing plugin, if not found, then returns instance passed
			var key = typeof(T);
			return (T)_plugins[key];
		}

		internal void Register<T>(T instance) where T : class, IR4MvcPlugin
		{
			var key = GetDirectInheritor(instance.GetType());
			if (!_plugins.ContainsKey(key))
			{
				_plugins.Add(key, instance);
			}
		}

		private static Type GetDirectInheritor(Type type)
		{
			// Need to register the interface that directly inherits IR4MvcPlugin
			// not the interface itself, concrete implementations or subclasses
			return
				type.GetTypeInfo().ImplementedInterfaces.SingleOrDefault(x => x.GetTypeInfo().ImplementedInterfaces.SingleOrDefault(p => p == typeof(IR4MvcPlugin)) != null);
		}
			
		public void Add<T>(T instance) where T : class, IR4MvcPlugin
		{
			// adds or overwrites a registered plugin instance
			var key = GetDirectInheritor(typeof(T));
			if (_plugins.ContainsKey(key))
			{
				_plugins[key] = instance;
			}
			else
			{
				_plugins.Add(key, instance);
			}
		}
	}
}