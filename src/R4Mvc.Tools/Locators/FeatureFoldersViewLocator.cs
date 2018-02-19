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


        public IEnumerable<View> Find(string projectRoot, IEnumerable<ControllerDefinition> controllers)
        {
            foreach (var controller in controllers)
            {
                var path = Path.GetDirectoryName(controller.GetFilePath());
                foreach (var view in FindViews(path, projectRoot, controller.Area))
                    yield return view;
            }
        }


        IEnumerable<View> FindViews(string directory, string projectRoot, string areaName)
        {
            foreach (var file in _fileLocator.GetFiles(directory, "*.cshtml"))
            {
                yield return GetView(file, projectRoot, Path.GetFileName(directory), areaName, null);
            }
        }

        View GetView(string filePath, string projectRoot, string controllerName, string areaName, string templateKind = null)
        {
            var relativePath = "~\\" + Path.GetFullPath(filePath).Substring(Path.GetFullPath(projectRoot).Length + 1);
            return new View(areaName, controllerName, Path.GetFileNameWithoutExtension(filePath), new Uri(relativePath, UriKind.Relative), templateKind);
        }

    }

}
