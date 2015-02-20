using System;
using System.Collections.Generic;

using Microsoft.AspNet.Builder;
using Microsoft.Framework.DependencyInjection;

using R4Mvc.Locators;
using R4Mvc.Services;

namespace R4Mvc.Ioc
{
	public static class IocConfig
	{
		public static IServiceProvider RegisterServices(IServiceProvider serviceProvider, IEnumerable<IViewLocator> viewLocators, IEnumerable<IStaticFileLocator> staticFileLocators)
		{
			var appBuilder = new ApplicationBuilder(serviceProvider);

			// register types for IServiceProvider here
			appBuilder.UseServices(
				svc =>
					{
						svc.AddInstance(typeof(IEnumerable<IViewLocator>), viewLocators);
						svc.AddInstance(typeof(IEnumerable<IStaticFileLocator>), staticFileLocators);
						svc.AddTransient<IViewLocatorService, ViewLocatorService>();
						svc.AddTransient<IStaticFileGeneratorService, StaticFileGeneratorService>();
						svc.AddTransient<IControllerRewriterService, ControllerRewriterService>();
						svc.AddTransient<IControllerGeneratorService, ControllerGeneratorService>();
						svc.AddTransient<R4MvcGenerator, R4MvcGenerator>();
					});

			return appBuilder.ApplicationServices;
		}
	}
}