using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.DependencyInjection.Fallback;

using R4Mvc.Locators;
using R4Mvc.Services;

namespace R4Mvc.Ioc
{
	public static class IocConfig
	{
		public static IServiceProvider RegisterServices(IEnumerable<IViewLocator> viewLocators, IEnumerable<IStaticFileLocator> staticFileLocators)
		{
			// register types for IServiceProvider here
			var serviceCollection = new ServiceCollection();
			viewLocators.Select(x => serviceCollection.AddInstance(x));
			staticFileLocators.Select(x => serviceCollection.AddInstance(x));
			serviceCollection.AddTransient<IViewLocatorService, ViewLocatorService>();
			serviceCollection.AddTransient<IStaticFileGeneratorService, StaticFileGeneratorService>();
			serviceCollection.AddTransient<IControllerRewriterService, ControllerRewriterService>();
			serviceCollection.AddTransient<IControllerGeneratorService, ControllerGeneratorService>();
			serviceCollection.AddTransient<R4MvcGenerator, R4MvcGenerator>();
			return serviceCollection.BuildServiceProvider();
		}

		public static IEnumerable<T> GetServices<T>(this IServiceProvider provider)
		{
			return provider.GetService<IEnumerable<T>>();
		} 
	}
}