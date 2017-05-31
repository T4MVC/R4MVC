//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.IO;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Framework.Runtime;
//using Microsoft.Framework.Runtime.Roslyn;
//using R4Mvc.Extensions;
//using R4Mvc.Ioc;
//using R4Mvc.Locators;
//using R4Mvc.Services;

//namespace R4Mvc
//{
//    public class R4MVCCompilerModule : ICompileModule
//    {
//        private readonly IServiceProvider _serviceProvider;

//        public ICollection<IViewLocator> ViewLocators { get; }

//        public ICollection<IStaticFileLocator> StaticFileLocators { get; }

//        private readonly DefaultRazorViewLocator _defaultRazorViewLocator = new DefaultRazorViewLocator();
//        private readonly DefaultStaticFileLocator _defaultStaticFileLocator = new DefaultStaticFileLocator();

//        public R4MVCCompilerModule(IServiceProvider serviceProvider)
//        {
//            ViewLocators = new Collection<IViewLocator>();
//            StaticFileLocators = new Collection<IStaticFileLocator>();

//            RegisterDefaultLocators();
//            RegisterCustomLocators();

//            _serviceProvider = IocConfig.RegisterServices(ViewLocators, StaticFileLocators);
//        }

//        public void BeforeCompile(IBeforeCompileContext context)
//        {
//            //Debugger.Launch();

//            var project = ((CompilationContext)(context)).Project;
//            var settings = LoadSettings(project);

//            // HACK to make project available to default view
//            _defaultRazorViewLocator.ProjectDelegate = () => project;
//            _defaultStaticFileLocator.ProjectDelegate = () => project;

//            // generate r4mvc syntaxtree
//            var generator = _serviceProvider.GetService<R4MvcGenerator>();
//            var generatedNode = generator.Generate((CompilationContext)context, settings);

//            // out to file
//            var generatedFilePath = Path.Combine(project.ProjectDirectory, R4MvcGenerator.R4MvcFileName);
//            generatedNode.WriteFile(generatedFilePath);

//            // update compilation
//            context.Compilation.AddSyntaxTrees(generatedNode.SyntaxTree);
//        }

//        public void AfterCompile(IAfterCompileContext context)
//        {
//        }

//        public virtual void RegisterCustomLocators() { }

//        private void RegisterDefaultLocators()
//        {
//            ViewLocators.Add(_defaultRazorViewLocator);
//            StaticFileLocators.Add(_defaultStaticFileLocator);
//        }

//        private ISettings LoadSettings(Project project)
//        {
//            return new Settings(project.ProjectDirectory);
//        }
//    }
//}
