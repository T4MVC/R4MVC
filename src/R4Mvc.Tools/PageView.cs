using System;

namespace R4Mvc.Tools
{
    public class PageView : IView
    {
        public PageView(string name, string filePath, string relativePath, string pagePath, bool isPage)
        {
            Name = name;
            FilePath = filePath;
            RelativePath = new Uri("~" + relativePath, UriKind.Relative);
            PagePath = pagePath;
            IsPage = isPage;

            var segments = pagePath.Split(new[] { '/', }, StringSplitOptions.RemoveEmptyEntries);
            Array.Resize(ref segments, segments.Length - 1);
            Segments = segments;
        }

        public string Name { get; }
        public string FilePath { get; }
        public Uri RelativePath { get; }
        public string PagePath { get; }
        public bool IsPage { get; }
        public string[] Segments { get; }

        public PageDefinition Definition { get; set; }

        public string TemplateKind => null;
    }
}
