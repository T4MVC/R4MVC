using System;
using System.Collections.Generic;
using System.IO;

namespace R4Mvc.Tools.Locators
{
    public class DefaultRazorViewLocator : IViewLocator
    {
        public IEnumerable<View> Find(string projectRoot)
        {
            var viewsPath = Path.Combine(projectRoot, "Views");
            if (Directory.Exists(viewsPath))
            {
                foreach (var controllerPath in Directory.GetDirectories(viewsPath))
                {
                    var controllerName = Path.GetFileName(controllerPath);
                    foreach (var file in Directory.GetFiles(Path.Combine(viewsPath, controllerName), "*.cshtml"))
                    {
                        yield return new View(string.Empty, controllerName + "Controller", Path.GetFileNameWithoutExtension(file), new Uri($"~/Views/{controllerName}/{Path.GetFileName(file)}", UriKind.Relative));
                    }
                }
            }

            var areasPath = Path.Combine(projectRoot, "Areas");
            if (Directory.Exists(areasPath))
            {
                foreach (var areaPath in Directory.GetDirectories(areasPath))
                {
                    var areaName = Path.GetFileName(areaPath);
                    viewsPath = Path.Combine(areaPath, "Views");
                    if (Directory.Exists(viewsPath))
                    {
                        foreach (var controllerPath in Directory.GetDirectories(viewsPath))
                        {
                            var controllerName = Path.GetFileName(controllerPath);
                            foreach (var file in Directory.GetFiles(Path.Combine(viewsPath, controllerName), "*.cshtml"))
                            {
                                yield return new View(areaName, controllerName + "Controller", Path.GetFileNameWithoutExtension(file), new Uri($"~/Areas/{areaName}/Views/{controllerName}/{Path.GetFileName(file)}", UriKind.Relative));
                            }
                        }
                    }
                }
            }
        }
    }
}
