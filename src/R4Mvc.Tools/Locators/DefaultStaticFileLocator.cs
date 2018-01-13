using System;
using System.Collections.Generic;
using System.Linq;

namespace R4Mvc.Tools.Locators
{
    public class DefaultStaticFileLocator : IStaticFileLocator
    {
        private readonly IFileLocator _fileLocator;
        private readonly Settings _settings;
        public DefaultStaticFileLocator(IFileLocator fileLocator, Settings settings)
        {
            _fileLocator = fileLocator;
            _settings = settings;
        }

        public IEnumerable<StaticFile> Find(string staticPathRoot)
        {
            var files = _fileLocator.GetFiles(staticPathRoot, "*", recurse: true).AsEnumerable();
            if (_settings.ExcludedStaticFileExtensions?.Length > 0)
                files = files.Where(f => !_settings.ExcludedStaticFileExtensions.Any(e => f.EndsWith(e)));
            if (!staticPathRoot.EndsWith("/"))
                staticPathRoot += "/";
            var rootUri = new Uri(staticPathRoot);
            return files.Select(f => new StaticFile(rootUri.MakeRelativeUri(new Uri(f))));
        }
    }
}
