using System;
using System.Collections.Generic;
using Path = System.IO.Path;

namespace R4Mvc.Tools.Locators
{
    public class DefaultRazorViewLocator : IViewLocator
    {
        private readonly IFileLocator _fileLocator;
        public DefaultRazorViewLocator(IFileLocator fileLocator)
        {
            _fileLocator = fileLocator;
        }

        public IEnumerable<View> Find(string projectRoot)
        {
            foreach (var view in FindViews(projectRoot, string.Empty))
                yield return view;

            var areasPath = Path.Combine(projectRoot, "Areas");
            if (_fileLocator.DirectoryExists(areasPath))
            {
                foreach (var areaPath in _fileLocator.GetDirectories(areasPath))
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
            if (_fileLocator.DirectoryExists(viewsPath))
            {
                foreach (var controllerPath in _fileLocator.GetDirectories(viewsPath))
                {
                    var controllerName = Path.GetFileName(controllerPath);
                    foreach (var file in _fileLocator.GetFiles(Path.Combine(viewsPath, controllerName), "*.cshtml"))
                    {
                        var relativePath = !string.IsNullOrEmpty(areaName)
                            ? $"~/Areas/{areaName}/Views/{controllerName}/{Path.GetFileName(file)}"
                            : $"~/Views/{controllerName}/{Path.GetFileName(file)}";
                        yield return GetView(file, controllerName, areaName);
                    }

                    var templatesPath = Path.Combine(controllerPath, "DisplayTemplates");
                    if (_fileLocator.DirectoryExists(templatesPath))
                    {
                        foreach (var file in _fileLocator.GetFiles(templatesPath, "*.cshtml"))
                        {
                            yield return GetView(file, controllerName, areaName, Path.GetFileName(templatesPath));
                        }
                    }

                    templatesPath = Path.Combine(controllerPath, "EditorTemplates");
                    if (_fileLocator.DirectoryExists(templatesPath))
                    {
                        foreach (var file in _fileLocator.GetFiles(templatesPath, "*.cshtml"))
                        {
                            yield return GetView(file, controllerName, areaName, Path.GetFileName(templatesPath));
                        }
                    }
                }
            }
        }

        private View GetView(string filePath, string controllerName, string areaName, string templateKind = null)
        {
            var templateKindSegment = templateKind != null ? templateKind + "/" : null;
            var relativePath = !string.IsNullOrEmpty(areaName)
                ? $"~/Areas/{areaName}/Views/{controllerName}/{templateKindSegment}{Path.GetFileName(filePath)}"
                : $"~/Views/{controllerName}/{templateKindSegment}{Path.GetFileName(filePath)}";
            return new View(areaName, controllerName, Path.GetFileNameWithoutExtension(filePath), new Uri(relativePath, UriKind.Relative), templateKind);
        }
    }
}
