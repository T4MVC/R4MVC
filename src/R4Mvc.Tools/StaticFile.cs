using R4Mvc.Tools.Extensions;
using System;

namespace R4Mvc.Tools
{
    public class StaticFile
    {
        public StaticFile(string fileName, Uri relativePath, Uri collectionName)
        {
            FileName = fileName.Replace(new[] { '.', '-', ' ' }, "_");
            RelativePath = relativePath;
            CollectionName = collectionName;
        }

        public string FileName { get; set; }

        public Uri RelativePath { get; set; }

        public Uri CollectionName { get; set; }
    }
}
