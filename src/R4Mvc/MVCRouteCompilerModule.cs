namespace R4Mvc
{
	using System;
	using System.Diagnostics;
	using System.IO;

	using Microsoft.Framework.Runtime;
	using Microsoft.Framework.Runtime.Roslyn;

	public class MVCRouteCompilerModule : ICompileModule
	{
		private readonly IServiceProvider _appProvider;

		private Project project;

		public MVCRouteCompilerModule(IServiceProvider provider)
		{
			_appProvider = provider; 
		}

		public void BeforeCompile(IBeforeCompileContext context)
		{
			Debugger.Launch();
			project = ((CompilationContext)(context)).Project;

            var compiler = context.CSharpCompilation;
			foreach (var tree in compiler.SyntaxTrees)
			{
				var newNode = new ControllerRewriter(compiler.GetSemanticModel(tree)).Visit(tree.GetRoot());
				if (!newNode.IsEquivalentTo(tree.GetRoot()))
				{
					// node has changed, update syntaxtree and persist to file
					compiler.ReplaceSyntaxTree(tree, newNode.SyntaxTree);
					File.WriteAllText(tree.FilePath, newNode.ToFullString());
				}
			}

			//var applicationEnvironment = _appProvider.GetRequiredService<IApplicationEnvironment>();
			//var projectResolver = _appProvider.GetRequiredService<IProjectResolver>();
			//var compilerOptionsProvider = _appProvider.GetRequiredService<ICompilerOptionsProvider>();
			//var compilationSettings = compilerOptionsProvider.GetCompilationSettings(applicationEnvironment);

			//var setup = new RazorViewEngineOptionsSetup(applicationEnvironment);
			//var sc = new ServiceCollection();
			//sc.ConfigureOptions(setup);
			//sc.AddMvc();
		}

		public void AfterCompile(IAfterCompileContext context)
		{
			// need to touch project file to invalidate klr source cache
			File.SetLastWriteTimeUtc(project.ProjectFilePath, DateTime.UtcNow);
		}
	}
}