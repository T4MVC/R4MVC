namespace R4Mvc.Compiler.Preprocess
{
	using System;
	using System.Diagnostics;

	using Microsoft.Framework.DependencyInjection;
	using Microsoft.Framework.Runtime;

	public class MVCRouteCompilerModule : ICompileModule
	{
		private readonly IServiceProvider _appProvider;

		public MVCRouteCompilerModule(IServiceProvider provider)
		{
			Debugger.Launch();
			_appProvider = provider; 
		}

		public void BeforeCompile(IBeforeCompileContext context)
		{
			var applicationEnvironment = _appProvider.GetRequiredService<IApplicationEnvironment>();
			var projectResolver = _appProvider.GetRequiredService<IProjectResolver>();
			//var compilerOptionsProvider = _appProvider.GetRequiredService<ICompilerOptionsProvider>();
			//var compilationSettings = compilerOptionsProvider.GetCompilationSettings(applicationEnvironment);

			//var setup = new RazorViewEngineOptionsSetup(applicationEnvironment);
			//var sc = new ServiceCollection();
			//sc.ConfigureOptions(setup);
			//sc.AddMvc();

			// 1. How to find all public types inheriting from Controller
			//		change class to partial and add {ControllerName}.Generated.cs nested class
			//		if no default constructor, add one to generated class
			//		CodeAnalysis should run for public classes and methods to trigger above
			// 2. Get all public methods that return type inherited from actionresult
			//      create method stub with same number of arguments but change arg types to dummy[null]
		}

		public void AfterCompile(IAfterCompileContext context)
		{
		}
	}
}