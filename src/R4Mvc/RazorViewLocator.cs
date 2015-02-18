using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace R4Mvc
{
	public class RazorViewLocator : IViewLocator
	{
		private readonly IEnumerable<string> _files;

		private readonly Uri _projectRoot;

		private const string _extension = ".cshtml";

		public RazorViewLocator(IEnumerable<string> files, Uri projectRoot)
		{
			this._files = files;
			_projectRoot = projectRoot;
		}

		public View[] Find()
		{
			// TODO replace unmockable calls to Path static
			// TODO handle invalid uri paths
			// TODO need virtual path provider perhaps?
			return
				_files.Where(x => x.EndsWith(_extension, StringComparison.CurrentCultureIgnoreCase))
					.Select(x =>
						{
							var controllerName = Path.GetDirectoryName(x)?.Split(Path.DirectorySeparatorChar).Last() + "Controller";
							var absoluteUrl = new Uri(x);
							return new View(controllerName, Path.GetFileNameWithoutExtension(x), _projectRoot.MakeRelativeUri(absoluteUrl));
						})
					.ToArray();
		}
	}
}