using System;

namespace R4Mvc.Tools
{
    public class PageView : IView
    {
        public PageView(string name, string filePath, string relativePath, string pagePath, bool isRazorPage)
        {
            Name = name;
            FilePath = filePath;
            RelativePath = new Uri("~" + relativePath, UriKind.Relative);
            PagePath = pagePath;
            IsRazorPage = isRazorPage;

            var segments = pagePath.Split(new[] { '/', }, StringSplitOptions.RemoveEmptyEntries);
            Array.Resize(ref segments, segments.Length - 1);
            Segments = segments;
        }

        public string Name { get; }
        public string FilePath { get; }
        public Uri RelativePath { get; }
        public string PagePath { get; }
        public bool IsRazorPage { get; }
        public string[] Segments { get; }

        public PageDefinition Definition { get; set; }

        public string TemplateKind => null;
    }
}
