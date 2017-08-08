using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace R4Mvc.Tools.Locators
{
    public class DefaultStaticFileLocator : IStaticFileLocator
    {
        public IEnumerable<StaticFile> Find(string staticPathRoot)
        {
            var files = Directory.GetFiles(staticPathRoot, "*", SearchOption.AllDirectories);
            if (!staticPathRoot.EndsWith("/"))
                staticPathRoot += "/";
            var rootUri = new Uri(staticPathRoot);
            return files.Select(f => new StaticFile(rootUri.MakeRelativeUri(new Uri(f))));
        }
    }
}
