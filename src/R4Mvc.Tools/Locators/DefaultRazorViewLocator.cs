using System;
using System.Collections.Generic;
using R4Mvc.Tools.Extensions;
using Path = System.IO.Path;

namespace R4Mvc.Tools.Locators
{
    public class DefaultRazorViewLocator : IViewLocator
    {
        private const string ViewsFolder = "Views";

        private readonly IFileLocator _fileLocator;
        private readonly Settings _settings;

        public DefaultRazorViewLocator(IFileLocator fileLocator, Settings settings)
        {
            _fileLocator = fileLocator;
            _settings = settings;
        }

        public IEnumerable<View> Find(string projectRoot)
        {
            if (_settings.FeatureFolders?.Enabled == true && !string.Equals(ViewsFolder, _settings.FeatureFolders.FeaturesPath, StringComparison.OrdinalIgnoreCase))
                foreach (var view in FindAllViews(projectRoot, _settings.FeatureFolders.FeaturesPath))
                    yield return view;

            foreach (var view in FindAllViews(projectRoot, ViewsFolder))
                yield return view;
        }

        public IEnumerable<View> FindAllViews(string projectRoot, string viewsFolder)
        {
            foreach (var view in FindViews(projectRoot, projectRoot, string.Empty, viewsFolder))
                yield return view;

            var areasPath = Path.Combine(projectRoot, "Areas");
            if (_fileLocator.DirectoryExists(areasPath))
            {
                foreach (var areaPath in _fileLocator.GetDirectories(areasPath))
                {
                    var areaName = Path.GetFileName(areaPath);
                    foreach (var view in FindViews(projectRoot, areaPath, areaName, viewsFolder))
                        yield return view;
                }
            }
        }

        private IEnumerable<View> FindViews(string projectRoot, string root, string areaName, string viewsFolder)
        {
            var viewsPath = Path.Combine(root, viewsFolder);
            if (_fileLocator.DirectoryExists(viewsPath))
            {
                foreach (var controllerPath in _fileLocator.GetDirectories(viewsPath))
                {
                    var controllerName = Path.GetFileName(controllerPath);
                    foreach (var file in _fileLocator.GetFiles(Path.Combine(viewsPath, controllerName), "*.cshtml"))
                    {
                        yield return GetView(projectRoot, file, controllerName, areaName);
                    }

                    foreach (var directory in _fileLocator.GetDirectories(controllerPath))
                    {
                        foreach (var file in _fileLocator.GetFiles(directory, "*.cshtml"))
                        {
                            yield return GetView(projectRoot, file, controllerName, areaName, Path.GetFileName(directory));
                        }
                    }
                }
            }
        }

        private View GetView(string projectRoot, string filePath, string controllerName, string areaName, string templateKind = null)
        {
            var relativePath = new Uri("~" + filePath.GetRelativePath(projectRoot).Replace("\\", "/"), UriKind.Relative);
            var templateKindSegment = templateKind != null ? templateKind + "/" : null;
            return new View(areaName, controllerName, Path.GetFileNameWithoutExtension(filePath), relativePath, templateKind);
        }
    }
}
