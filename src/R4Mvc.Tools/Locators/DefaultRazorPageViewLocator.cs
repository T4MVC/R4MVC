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
            foreach (var filePath in _fileLocator.GetFiles(Path.Combine(projectRoot, PagesFolder), "*.cshtml", recurse: true))
            {
                yield return GetView(projectRoot, filePath);
            }
        }

        private PageView GetView(string projectRoot, string filePath)
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

            return new PageView(Path.GetFileNameWithoutExtension(filePath), filePath, filePath.GetRelativePath(projectRoot).Replace("\\", "/"), isPage);
        }
    }
}
