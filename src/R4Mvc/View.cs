using System;

namespace R4Mvc
{
	public class View
	{
		public View(string controllerName, string viewName, Uri relativePath)
		{
			this.ControllerName = controllerName;
			this.ViewName = viewName;
			this.RelativePath =  relativePath;
		}

		public string ControllerName { get; private set; }

		public string ViewName { get; private set; }

		public Uri RelativePath { get; private set; }
	}
}