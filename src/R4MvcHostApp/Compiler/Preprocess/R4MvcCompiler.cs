using System.IO;

using R4Mvc;

namespace R4MvcHostApp.Compiler.Preprocess
{
	public class R4MvcCompiler : R4MVCCompilerModule
	{
		public R4MvcCompiler()
		{
			// You can override the generated file location here
			// this.SetGeneratedFilePath(() => Path.Combine(this.Project.ProjectDirectory, "R4Custom.cs"));
        }
	}
}