using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

//using Microsoft.Framework.Runtime;

namespace R4Mvc.Locators
{
	public class DefaultStaticFileLocator : IStaticFileLocator
	{
		//public Func<Project> ProjectDelegate;

		public IEnumerable<StaticFile> Find()
		{
			return new StaticFile[0];

			// TODO need to group by folders and create class hierarchy
			//var project = ProjectDelegate.Invoke();
			//var projectDirectory = project.ProjectDirectory;
			//var projectRootUrl = projectDirectory.EndsWith("/") ? new Uri(projectDirectory) : new Uri(projectDirectory + "/");

            // TODO: Refactor out our dependency on Project.ContentFiles
            return new List<StaticFile>();

           // return
				//project.ContentFiles.Select(
				//	x =>
				//		{
				//			var absoluteUrl = new Uri(x);
				//			var @namespace = absoluteUrl;
				//			return new StaticFile(Path.GetFileNameWithoutExtension(x), projectRootUrl.MakeRelativeUri(absoluteUrl), @namespace);
				//		});
		}
	}
}
