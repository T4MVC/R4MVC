using System;
using System.Collections.Generic;
using System.IO;

namespace R4Mvc.Tools.Locators
{
    public class DefaultRazorViewLocator : IViewLocator
    {
        public IEnumerable<View> Find(string projectRoot)
        {
            foreach (var view in FindViews(projectRoot, string.Empty))
                yield return view;

            var areasPath = Path.Combine(projectRoot, "Areas");
            if (Directory.Exists(areasPath))
            {
                foreach (var areaPath in Directory.GetDirectories(areasPath))
                {
                    var areaName = Path.GetFileName(areaPath);
                    foreach (var view in FindViews(areaPath, areaName))
                        yield return view;
                }
            }
        }

        private IEnumerable<View> FindViews(string root, string areaName)
        {
            var viewsPath = Path.Combine(root, "Views");
            if (Directory.Exists(viewsPath))
            {
                foreach (var controllerPath in Directory.GetDirectories(viewsPath))
                {
                    var controllerName = Path.GetFileName(controllerPath);
                    foreach (var file in Directory.GetFiles(Path.Combine(viewsPath, controllerName), "*.cshtml"))
                    {
                        var relativePath = !string.IsNullOrEmpty(areaName)
                            ? $"~/Areas/{areaName}/Views/{controllerName}/{Path.GetFileName(file)}"
                            : $"~/Views/{controllerName}/{Path.GetFileName(file)}";
                        yield return GetView(file, controllerName, areaName);
                    }

                    var templatesPath = Path.Combine(controllerPath, "DisplayTemplates");
                    if (Directory.Exists(templatesPath))
                    {
                        foreach (var file in Directory.GetFiles(templatesPath, "*.cshtml"))
                        {
                            yield return GetView(file, controllerName, areaName, Path.GetFileName(templatesPath));
                        }
                    }

                    templatesPath = Path.Combine(controllerPath, "EditorTemplates");
                    if (Directory.Exists(templatesPath))
                    {
                        foreach (var file in Directory.GetFiles(templatesPath, "*.cshtml"))
                        {
                            yield return GetView(file, controllerName, areaName, Path.GetFileName(templatesPath));
                        }
                    }
                }
            }
        }

        private View GetView(string filePath, string controllerName, string areaName, string templateKind = null)
        {
            var relativePath = !string.IsNullOrEmpty(areaName)
                ? $"~/Areas/{areaName}/Views/{controllerName}/{Path.GetFileName(filePath)}"
                : $"~/Views/{controllerName}/{Path.GetFileName(filePath)}";
            return new View(areaName, controllerName, Path.GetFileNameWithoutExtension(filePath), new Uri(relativePath, UriKind.Relative), templateKind);
        }
    }
}
