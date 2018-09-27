using System;
using System.Collections.Generic;
using System.IO;
using R4Mvc.Tools.Extensions;

namespace R4Mvc.Tools.Locators
{
    public class DefaultRazorPageViewLocator : IPageViewLocator
    {
        protected const string PagesFolder = "Pages";

        private readonly IFileLocator _fileLocator;
        private readonly Settings _settings;

        public DefaultRazorPageViewLocator(IFileLocator fileLocator, Settings settings)
        {
            _fileLocator = fileLocator;
            _settings = settings;
        }

        public IEnumerable<PageView> Find(string projectRoot)
        {
            var pagesRoot = Path.Combine(projectRoot, PagesFolder);
            if (Directory.Exists(pagesRoot))
                foreach (var filePath in _fileLocator.GetFiles(pagesRoot, "*.cshtml", recurse: true))
                {
                    yield return GetView(projectRoot, pagesRoot, filePath);
                }
        }

        private PageView GetView(string projectRoot, string pagesRoot, string filePath)
        {
            bool isPage = false;
            using (var file = File.OpenRead(filePath))
            using (var reader = new StreamReader(file))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var trimmedLine = line.TrimStart();
                    if (trimmedLine.Length == 0)
                        continue;
                    if (trimmedLine[0] != '@')
                        break;
                    if (trimmedLine.StartsWith("@page"))
                    {
                        isPage = true;
                        break;
                    }
                }
            }

            var relativePath = filePath.GetRelativePath(projectRoot).Replace("\\", "/");
            var pagePath = filePath.GetRelativePath(pagesRoot).Replace("\\", "/").TrimEnd(".cshtml");
            return new PageView(Path.GetFileNameWithoutExtension(filePath), filePath, relativePath, pagePath, isPage);
        }
    }
}
