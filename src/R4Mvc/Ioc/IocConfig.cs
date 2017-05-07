using System;
using System.Collections.Generic;

using Microsoft.Extensions.DependencyInjection;

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
			serviceCollection.AddSingleton(typeof(IEnumerable<IViewLocator>), viewLocators);
			serviceCollection.AddSingleton(typeof(IEnumerable<IStaticFileLocator>), staticFileLocators);
			serviceCollection.AddTransient<IViewLocatorService, ViewLocatorService>();
			serviceCollection.AddTransient<IStaticFileGeneratorService, StaticFileGeneratorService>();
			serviceCollection.AddTransient<IControllerRewriterService, ControllerRewriterService>();
			serviceCollection.AddTransient<IControllerGeneratorService, ControllerGeneratorService>();
			serviceCollection.AddTransient<R4MvcGenerator, R4MvcGenerator>();
			return serviceCollection.BuildServiceProvider();
		}
	}
}
