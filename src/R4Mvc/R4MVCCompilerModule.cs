using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Runtime;
using Microsoft.Framework.Runtime.Roslyn;

namespace R4Mvc
{
	public class R4MVCCompilerModule : ICompileModule
	{
		private readonly List<ClassDeclarationSyntax> _mvcClasses;

		private readonly IServiceProvider _serviceProvider;

		public bool filesGenerated;

		private Func<string> getFilePath;

		public R4MVCCompilerModule(IServiceProvider serviceProvider)
		{
			this._serviceProvider = serviceProvider;
			_mvcClasses = new List<ClassDeclarationSyntax>();
		}

		public Project Project { get; private set; }

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

			//Debugger.Launch();

			Project = ((CompilationContext)(context)).Project;
			var generatedFilePath = this.GetGeneratedFilePath(Project);

			var compiler = context.CSharpCompilation;
			foreach (var tree in compiler.SyntaxTrees.Where(x => !x.FilePath.Equals(generatedFilePath)))
			{
				// if syntaxtree has errors, skip code generation
				if (tree.GetDiagnostics().Any(x => x.Severity == DiagnosticSeverity.Error)) continue;

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

			// get the project view files
			var viewFinder = _serviceProvider.GetService<IViewLocator>()
			                 ?? new RazorViewLocator(Project.ContentFiles, new Uri(Project.ProjectDirectory + Path.DirectorySeparatorChar));
			var viewFiles = viewFinder.Find();

			// pass the controller classes to the R4MVC Generator and save file in Project root
			var generatedNode = R4MvcGenerator.Generate(compiler, _mvcClasses.ToArray(), viewFiles);
			generatedNode.WriteFile(generatedFilePath);
			compiler.AddSyntaxTrees(generatedNode.SyntaxTree);

			filesGenerated = true;
#endif
		}

		public void AfterCompile(IAfterCompileContext context)
		{
			// TODO need to touch Project file to invalidate klr source cache
			// otherwise you can to kill klr.exe process. an idea is to touch the
			// Project file but the line below won't compile for klr
			//File.SetLastWriteTimeUtc(Project.ProjectFilePath, DateTime.UtcNow);
		}

		private string GetGeneratedFilePath(Project project)
		{
			return getFilePath == null ? Path.Combine(project.ProjectDirectory, R4MvcFileName) : getFilePath.Invoke();
		}

		protected void SetGeneratedFilePath(Func<string> filePath)
		{
			getFilePath = filePath;
		}

		public const string R4MvcFileName = "R4MVC.generated.cs";
	}
}