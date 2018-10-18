using System;

namespace R4Mvc.Tools
{
    public class View : IView
    {
        public View(string areaName, string controllerName, string viewName, Uri relativePath, string templateKind)
        {
            AreaName = areaName;
            ControllerName = controllerName;
            Name = viewName;
            RelativePath = relativePath;
            TemplateKind = templateKind;
        }

        public string AreaName { get; }
        public string ControllerName { get; }
        public string Name { get; }
        public Uri RelativePath { get; }
        public string TemplateKind { get; set; }
    }
}
