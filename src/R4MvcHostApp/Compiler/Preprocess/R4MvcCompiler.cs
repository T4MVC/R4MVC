using System.Collections.Generic;

using R4Mvc;

namespace R4MvcHostApp.Compiler.Preprocess
{
	public class R4MvcCompiler : R4MVCCompilerModule
	{
		public R4MvcCompiler()
		{
			/*
			Expermenting with extensibility : The Plugins collection..because
			The IServiceProvider does not have any method of adding instances so not sure if this can be used
			It also does not have access to the full mvc stack in the precompiler stage, containing only a few services
			which are of limited use to r4mvc

			We *could* use roslyn to scan the target project for types but they won't be compiled and you have no control 
			over registration with duplicates etc and therefore which ones to pick
			The k runtime will however need to compile this class and in turn classes it uses which allows it
			to pick up and use implementations of plugins from the target project

			The PluginProvider can also ensure that only 1 instance is registered at a time

			Another advantage of this method is that all setup relating to R4MVC is in one place so which
			should aid discoverability and deployment of this compiler file with documented examples via nuget
			*/

			// You can override the default plugins here
			//Plugins.Add(new CustomViewLocator());
			//Plugins.Add(new CustomProjectLocator());
		}
	}

	public class CustomViewLocator : IViewLocator
	{
		public View[] Find()
		{
			return new View[0];
		}
	}

	public class CustomProjectLocator : IProjectLocator
	{
		public string GetProjectDirectory()
		{
			return "";
		}

		public IEnumerable<string> GetContentFiles()
		{
			return new string[0];
		}

		public IEnumerable<string> GetSourceFiles()
		{
			return new string[0];
		}

		public string GetGeneratedFilePath()
		{
			return "invalid";
		}
	}
}