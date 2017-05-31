using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

//using Microsoft.Framework.Runtime;

namespace R4Mvc.Locators
{
	public class DefaultRazorViewLocator : IViewLocator
	{
		//public Func<Project> ProjectDelegate;

		private const string _extension = ".cshtml";

		public IEnumerable<View> Find()
		{
			//var project = ProjectDelegate.Invoke();
			//var projectDirectory = project.ProjectDirectory;
			//var projectRootUrl = projectDirectory.EndsWith("/") ? new Uri(projectDirectory) : new Uri(projectDirectory + "/");

            // TODO: Refactor out our dependency on Project.ContentFiles
            return new List<View>();

            // return
				//project.ContentFiles.Where(x => x.EndsWith(_extension, StringComparison.CurrentCultureIgnoreCase))
				//	.Select(x =>
				//	{
				//		var controllerName = Path.GetDirectoryName(x)?.Split(Path.DirectorySeparatorChar).Last() + "Controller";
				//		var absoluteUrl = new Uri(x);
				//		return new View(controllerName, Path.GetFileNameWithoutExtension(x), projectRootUrl.MakeRelativeUri(absoluteUrl));
				//	});
		}
	}
}
