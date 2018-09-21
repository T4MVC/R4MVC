using System;

namespace R4Mvc.Tools
{
    public class PageView
    {
        public PageView(string viewName, Uri relativePath, bool isPage)
        {
            ViewName = viewName;
            RelativePath = relativePath;
            IsPage = isPage;
        }

        public string ViewName { get; }
        public Uri RelativePath { get; }
        public bool IsPage { get; }
    }
}
