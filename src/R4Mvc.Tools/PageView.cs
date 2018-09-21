using System;

namespace R4Mvc.Tools
{
    public class PageView : IView
    {
        public PageView(string viewName, string filePath, Uri relativePath, bool isPage)
        {
            ViewName = viewName;
            FilePath = filePath;
            RelativePath = relativePath;
            IsPage = isPage;

            var segments = relativePath.OriginalString.TrimStart('~').Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
            Array.Resize(ref segments, segments.Length - 1);
            Segments = segments;
        }

        public string ViewName { get; }
        public string FilePath { get; }
        public Uri RelativePath { get; }
        public bool IsPage { get; }
        public string[] Segments { get; }

        string IView.TemplateKind => null;
    }
}
