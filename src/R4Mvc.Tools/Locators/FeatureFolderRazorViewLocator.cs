using System;
using System.Collections.Generic;
using System.Linq;
using Path = System.IO.Path;

namespace R4Mvc.Tools.Locators
{
    public class FeatureFolderRazorViewLocator : DefaultRazorViewLocator
    {
        private readonly IFileLocator _fileLocator;
        private readonly Settings _settings;
        private bool _allAreasAreFeatureFolders = false;

        public FeatureFolderRazorViewLocator(IFileLocator fileLocator, Settings settings)
            : base(fileLocator, settings)
        {
            _fileLocator = fileLocator;
            _settings = settings;
        }

        protected override string GetViewsRoot(string projectRoot) => Path.Combine(projectRoot, _settings.FeatureFolders.FeaturesPath);
        protected override string GetAreaViewsRoot(string areaRoot, string areaName)
            => _allAreasAreFeatureFolders || _settings.FeatureFolders.FeatureOnlyAreas?.Contains(areaName, StringComparer.OrdinalIgnoreCase) == true
                ? areaRoot
                : Path.Combine(areaRoot, _settings.FeatureFolders.FeaturesPath);

        public override IEnumerable<View> Find(string projectRoot)
        {
            if (_settings.FeatureFolders?.Enabled != true)
                return new View[0];
            _allAreasAreFeatureFolders = _settings.FeatureFolders.FeatureOnlyAreas?.Contains("*") == true;
            return base.Find(projectRoot);
        }
    }
}
