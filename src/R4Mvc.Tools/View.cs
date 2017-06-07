using System;

namespace R4Mvc.Tools
{
    public class View
    {
        public View(string areaName, string controllerName, string viewName, Uri relativePath, string templateKind)
        {
            AreaName = areaName;
            ControllerName = controllerName;
            ViewName = viewName;
            RelativePath = relativePath;
            TemplateKind = templateKind;
        }

        public string AreaName { get; }
        public string ControllerName { get; }
        public string ViewName { get; }
        public Uri RelativePath { get; }
        public string TemplateKind { get; set; }
    }
}
