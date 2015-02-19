using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Framework.Runtime;
using Microsoft.Framework.Runtime.Roslyn;
using R4Mvc.Extensions;
using R4Mvc.Ioc;

namespace R4Mvc
{
    public class R4MvcPreCompileModule : ICompileModule
    {
        private readonly IServiceProvider _serviceProvider;

        public ICollection<IControllerLocator> ControllerLocators { get; private set; }
        public ICollection<IViewLocator> ViewLocators { get; private set; }
        public ICollection<IStaticFileLocator> StaticFileLocators { get; private set; }


        public R4MvcPreCompileModule(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            ControllerLocators = new Collection<IControllerLocator>();
            ViewLocators = new Collection<IViewLocator>();
            StaticFileLocators = new Collection<IStaticFileLocator>();

            RegisterDefaultLocators();
            RegisterCustomLocators();

            IocConfig.RegisterServices(serviceProvider);
        }

        public void BeforeCompile(IBeforeCompileContext context)
        {
            var compilerContext = (CompilationContext)(context);
            var generator = _serviceProvider.GetService<R4MvcGenerator>();

            generator.Generate(compilerContext);
        }

        public void AfterCompile(IAfterCompileContext context)
        {

        }

        public virtual void RegisterCustomLocators() { }

        private void RegisterDefaultLocators()
        {
            ControllerLocators.Add(new DefaultControllerLocator());
            ViewLocators.Add(new DefaultRazorViewLocator());
            StaticFileLocators.Add(new DefaultStaticFileLocator());
        }
    }
}