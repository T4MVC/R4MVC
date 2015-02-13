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

		public bool filesGenerated;

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
			this.project = ((CompilationContext)(context)).Project;

			var compiler = context.CSharpCompilation;
			foreach (var tree in compiler.SyntaxTrees.Where(x => !x.FilePath.EndsWith(SyntaxHelpers.R4MvcFileName)))
			{
				// this first part, finds all the controller classes, modifies them and saves the changes
				var controllerRewriter = new ControllerRewriter(compiler);
				var newNode = controllerRewriter.Visit(tree.GetRoot());

				if (!newNode.IsEquivalentTo(tree.GetRoot()))
				{
					// node has changed, update syntaxtree and persist to file
					compiler.ReplaceSyntaxTree(tree, newNode.SyntaxTree);
					newNode.WriteFile(tree.FilePath, false);
				}

				// save the controller nodes from each visit to pass to the generator
				this.MvcClasses.AddRange(controllerRewriter.MvcControllerClassNodes);
			}

			// pass the controller classes to the R4MVC Generator and save file in project root
			var generatedNode = R4MvcGenerator.Generate(compiler, this.MvcClasses.ToArray());
			var generatedFilePath = GetGeneratedFilePath(project);
			generatedNode.WriteFile(generatedFilePath, true);
			filesGenerated = true;
#endif
		}

		public void AfterCompile(IAfterCompileContext context)
		{
#if DEBUG
			// TODO need to touch project file to invalidate klr source cache
			// otherwise you can to kill klr.exe process. an idea is to touch the
			// project file but the line below won't compile for klr
			// File.SetLastWriteTimeUtc(project.ProjectFilePath, DateTime.UtcNow);
#endif
		}

		private static string GetGeneratedFilePath(Project project)
		{
			return Path.Combine(project.ProjectDirectory, SyntaxHelpers.R4MvcFileName);
		}
	}
}