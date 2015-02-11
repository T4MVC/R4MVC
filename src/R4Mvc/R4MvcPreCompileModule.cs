namespace R4Mvc
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;

	using Microsoft.CodeAnalysis.CSharp.Syntax;
	using Microsoft.Framework.Runtime;
	using Microsoft.Framework.Runtime.Roslyn;

	public class R4MVCCompilerModule : ICompileModule
	{
		private Project project;

		private readonly List<ClassDeclarationSyntax> MvcClasses = new List<ClassDeclarationSyntax>();

		private bool filesGenerated;

		public void BeforeCompile(IBeforeCompileContext context)
		{
#if !ASPNETCORE50
			// TODO: Fix writing out generated files in both frameworks
			if (filesGenerated)
			{
				// TODO compilation is run a second time after files are modified and generated
				return;
			}

			Debugger.Launch();
			this.project = ((CompilationContext)(context)).Project;

			var compiler = context.CSharpCompilation;
			foreach (var tree in compiler.SyntaxTrees.Where(x => !x.FilePath.EndsWith(R4MvcHelpers.R4MvcFileName)))
			{
				var controllerRewriter = new ControllerRewriter(compiler);
				var newNode = controllerRewriter.Visit(tree.GetRoot());
				this.MvcClasses.AddRange(controllerRewriter.MvcControllerClassNodes);
				if (newNode.IsEquivalentTo(tree.GetRoot()))
				{
					continue;
				}

				// node has changed, update syntaxtree and persist to file
				compiler.ReplaceSyntaxTree(tree, newNode.SyntaxTree);
				newNode.WriteFile(tree.FilePath);
			}

			this.filesGenerated = R4MvcGenerator.Generate(compiler, this.MvcClasses.ToArray());
#endif
		}

		public void AfterCompile(IAfterCompileContext context)
		{
			// need to touch project file to invalidate klr source cache
			//File.SetLastWriteTimeUtc(project.ProjectFilePath, DateTime.UtcNow);
		}
	}
}