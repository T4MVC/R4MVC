namespace R4Mvc
{
	using System;
	using System.Diagnostics;

	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.Framework.DependencyInjection;
	using Microsoft.Framework.Runtime;

	public class MVCRouteCompilerModule : ICompileModule
	{
		private readonly IServiceProvider _appProvider;

		public MVCRouteCompilerModule(IServiceProvider provider)
		{
			_appProvider = provider; 
		}

		public void BeforeCompile(IBeforeCompileContext context)
		{
			Debugger.Launch();

			var applicationEnvironment = _appProvider.GetRequiredService<IApplicationEnvironment>();
			var projectResolver = _appProvider.GetRequiredService<IProjectResolver>();
			var compiler = context.CSharpCompilation;

			foreach (var tree in compiler.SyntaxTrees)
			{
				var newNode = new ControllerRewriter(compiler.GetSemanticModel(tree)).Visit(tree.GetRoot());
				if (!newNode.IsEquivalentTo(tree.GetRoot()))
				{
					// node has changed, should write to file
					compiler.ReplaceSyntaxTree(tree, newNode.SyntaxTree);
					CSharpSyntaxTree.Create(newNode as CSharpSyntaxNode, null, tree.FilePath);
				}
			}

			//var compilerOptionsProvider = _appProvider.GetRequiredService<ICompilerOptionsProvider>();
			//var compilationSettings = compilerOptionsProvider.GetCompilationSettings(applicationEnvironment);

			//var setup = new RazorViewEngineOptionsSetup(applicationEnvironment);
			//var sc = new ServiceCollection();
			//sc.ConfigureOptions(setup);
			//sc.AddMvc();
		}

		public void AfterCompile(IAfterCompileContext context)
		{
		}
	}
}