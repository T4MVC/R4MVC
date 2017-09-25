using System;
using System.Collections.Generic;
using System.Linq;

namespace R4Mvc.Tools.Locators
{
    public class DefaultStaticFileLocator : IStaticFileLocator
    {
        private readonly IFileLocator _fileLocator;
        public DefaultStaticFileLocator(IFileLocator fileLocator)
        {
            _fileLocator = fileLocator;
        }

        public IEnumerable<StaticFile> Find(string staticPathRoot)
        {
            var files = _fileLocator.GetFiles(staticPathRoot, "*", recurse: true);
            if (!staticPathRoot.EndsWith("/"))
                staticPathRoot += "/";
            var rootUri = new Uri(staticPathRoot);
            return files.Select(f => new StaticFile(rootUri.MakeRelativeUri(new Uri(f))));
        }
    }
}
