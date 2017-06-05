using System;

namespace R4Mvc.Tools
{
    public class View
    {
        public View(string areaName, string controllerName, string viewName, Uri relativePath)
        {
            AreaName = areaName;
            ControllerName = controllerName;
            ViewName = viewName;
            RelativePath = relativePath;
        }

        public string AreaName { get; }
        public string ControllerName { get; }
        public string ViewName { get; }
        public Uri RelativePath { get; }
    }
}
