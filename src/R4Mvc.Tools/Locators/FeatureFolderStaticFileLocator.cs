using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace R4Mvc.Tools.Locators
{
    public class FeatureFolderStaticFileLocator : IStaticFileLocator
    {
        private readonly IFileLocator _fileLocator;
        private readonly Settings _settings;

        public FeatureFolderStaticFileLocator(IFileLocator fileLocator, Settings settings)
        {
            _fileLocator = fileLocator;
            _settings = settings;
        }

        public IEnumerable<StaticFile> Find(string projectRoot, string staticPathRoot)
        {
            if (_settings.FeatureFolders.StaticFileAccess == false)
                return Array.Empty<StaticFile>();

            var files = _fileLocator.GetFiles(projectRoot, "*", recurse: true).AsEnumerable();
            files = files.Where(r => r.StartsWith(staticPathRoot, StringComparison.InvariantCultureIgnoreCase) == false);

            if (_settings.ExcludedStaticFileExtensions?.Length > 0)
                files = files.Where(f => !_settings.ExcludedStaticFileExtensions.Any(e => f.EndsWith(e)));
            if (!projectRoot.EndsWith("/"))
                projectRoot += "/";
            var rootUri = new Uri(projectRoot);
            if(_settings.FeatureFolders.IncludedStaticFileExtensions?.Length >0)
                files = files.Where(r => _settings.FeatureFolders.IncludedStaticFileExtensions.Contains(Path.GetExtension(r)));
            return files.Select(f => new StaticFile(rootUri.MakeRelativeUri(new Uri(f))));

        }

    }
}
