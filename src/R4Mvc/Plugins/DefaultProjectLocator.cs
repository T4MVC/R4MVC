using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.Framework.Runtime;
using Microsoft.Framework.Runtime.Roslyn;

namespace R4Mvc
{
	public class DefaultProjectLocator : IProjectLocator
	{
		private const string R4MvcFileName = "R4MVC.generated.cs";

		private readonly CompilationContext _compilation;

		public DefaultProjectLocator(IBeforeCompileContext context)
		{
			if (context == null)
			{
				throw new ArgumentNullException(nameof(context));
			}
			_compilation = (CompilationContext)context;
		}

		public Project GetProject()
		{
			return _compilation.Project;
		}

		public string GetProjectDirectory()
		{
			return GetProject().ProjectDirectory;
		}

		public IEnumerable<string> GetContentFiles()
		{
			return GetProject().ContentFiles;
		}

		public IEnumerable<string> GetSourceFiles()
		{
			return GetProject().SourceFiles;
		}

		public string GetGeneratedFilePath()
		{
			return Path.Combine(GetProjectDirectory(), R4MvcFileName);
		}
	}
}