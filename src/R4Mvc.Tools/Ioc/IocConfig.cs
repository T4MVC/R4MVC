using Microsoft.Extensions.DependencyInjection;
using R4Mvc.Tools.Locators;
using R4Mvc.Tools.Services;
using System.Collections.Generic;

namespace R4Mvc.Tools.Ioc
{
    public static class IocConfig
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton(typeof(IEnumerable<IViewLocator>), new[] { new DefaultRazorViewLocator() });
            services.AddSingleton(typeof(IEnumerable<IStaticFileLocator>), new[] { new DefaultStaticFileLocator() });
            services.AddTransient<IViewLocatorService, ViewLocatorService>();
            services.AddTransient<IStaticFileGeneratorService, StaticFileGeneratorService>();
            services.AddTransient<IControllerRewriterService, ControllerRewriterService>();
            services.AddTransient<IControllerGeneratorService, ControllerGeneratorService>();
            services.AddTransient<R4MvcGenerator, R4MvcGenerator>();
        }
    }
}
