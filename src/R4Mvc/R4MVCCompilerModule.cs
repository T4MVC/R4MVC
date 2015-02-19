using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Framework.Runtime;

namespace R4Mvc
{
	public class R4MVCCompilerModule : ICompileModule
	{
		private readonly List<ClassDeclarationSyntax> _mvcClasses;

		public bool filesGenerated;

		public R4MVCCompilerModule()
		{
			_mvcClasses = new List<ClassDeclarationSyntax>();
		}

		public R4MvcPluginProvider Plugins { get; } = new R4MvcPluginProvider();

		public void BeforeCompile(IBeforeCompileContext context)
		{
			// NOTE compilation and generation is always run a second time if files are modified on first run
#if !ASPNETCORE50
			// TODO: Fix writing out generated files in both frameworks
			if (filesGenerated)
			{
				// TODO compilation is run a second time after files are modified and generated
				return;
			}

			// Register plugins, will be ignored if target project has already registered
			Plugins.Register(new DefaultProjectLocator(context));
			var projectLocator = Plugins.Get<IProjectLocator>();
			Plugins.Register(new RazorViewLocator(projectLocator));

			// Plugin validation, check file paths etc?

			//Debugger.Launch();

			var compiler = context.CSharpCompilation;
			
			foreach (var tree in compiler.SyntaxTrees.Where(x => !x.FilePath.Equals(projectLocator.GetGeneratedFilePath())))
			{
				// if syntaxtree has errors, skip code generation
				if (tree.GetDiagnostics().Any(x => x.Severity == DiagnosticSeverity.Error))
				{
					continue;
				}

				// this first part, finds all the controller classes, modifies them and saves the changes
				var controllerRewriter = new ControllerRewriter(compiler);
				var newNode = controllerRewriter.Visit(tree.GetRoot());

				if (!newNode.IsEquivalentTo(tree.GetRoot()))
				{
					// node has changed, update syntaxtree and persist to file
					compiler = compiler.ReplaceSyntaxTree(tree, newNode.SyntaxTree);
					newNode.WriteFile(tree.FilePath);
				}

				// save the controller nodes from each visit to pass to the generator
				_mvcClasses.AddRange(controllerRewriter.MvcControllerClassNodes);
			}

			var viewFinder = Plugins.Get<IViewLocator>();
			var viewFiles = viewFinder.Find();

			// pass the controller classes to the R4MVC Generator and save file in Project root
			var generatedNode = R4MvcGenerator.Generate(compiler, _mvcClasses.ToArray(), viewFiles);
			generatedNode.WriteFile(projectLocator.GetGeneratedFilePath());

			compiler.AddSyntaxTrees(generatedNode.SyntaxTree);

			filesGenerated = true;
#endif
		}

		public void AfterCompile(IAfterCompileContext context)
		{
		}
	}
}