using R4Mvc.Tools.Extensions;
using System;
using System.Linq;

namespace R4Mvc.Tools
{
    public class StaticFile
    {
        public StaticFile(Uri relativePath)
        {
            var pathParts = relativePath.ToString().Split('/', '\\');

            FileName = pathParts.Last().Replace(new[] { '.', '-', ' ' }, "_");
            RelativePath = relativePath;
            Container = string.Join("/", pathParts.Take(pathParts.Length - 1));
        }

        public string FileName { get; set; }
        public Uri RelativePath { get; set; }
        public string Container { get; set; }
    }
}
