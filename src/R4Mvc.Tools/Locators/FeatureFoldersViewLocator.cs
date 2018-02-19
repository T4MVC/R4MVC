using System;
using System.Collections.Generic;
using Path = System.IO.Path;

namespace R4Mvc.Tools.Locators
{
    public class FeatureFoldersViewLocator : IViewLocator
    {
        private readonly IFileLocator _fileLocator;
        public FeatureFoldersViewLocator(IFileLocator fileLocator)
        {
            _fileLocator = fileLocator;
        }

        public IEnumerable<View> Find(string projectRoot)
        {
            foreach (var view in FindViews(projectRoot, projectRoot, string.Empty))
                yield return view;

            var areasPath = Path.Combine(projectRoot, "Areas");
            if (_fileLocator.DirectoryExists(areasPath))
            {
                foreach (var areaPath in _fileLocator.GetDirectories(areasPath))
                {
                    var areaName = Path.GetFileName(areaPath);
                    foreach (var view in FindViews(areaPath, projectRoot, areaName))
                        yield return view;
                }
            }
        }

        IEnumerable<View> FindViews(string root, string projectRoot, string areaName)
        {
            foreach (var directory in _fileLocator.GetDirectories(root))
            {
                foreach (var file in _fileLocator.GetFiles(directory, "*.cshtml"))
                {
                    yield return GetView(file, projectRoot, Path.GetFileName(directory), areaName, null);
                }
            }
        }

        View GetView(string filePath, string projectRoot, string controllerName, string areaName, string templateKind = null)
        {
            var relativePath = "~\\" + Path.GetFullPath(filePath).Substring(Path.GetFullPath(projectRoot).Length + 1);

            //var templateKindSegment = templateKind != null ? templateKind + "/" : null;
            //var relativePath = !string.IsNullOrEmpty(areaName)
            //    ? $"~/Areas/{areaName}/{controllerName}/{Path.GetFileName(filePath)}"
            //    : $"~/Views/{controllerName}/{templateKindSegment}{Path.GetFileName(filePath)}";
            return new View(areaName, controllerName, Path.GetFileNameWithoutExtension(filePath), new Uri(relativePath, UriKind.Relative), templateKind);
        }

    }
}
