using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace R4Mvc.Tools
{
    public class PageDefinition
    {
        public string Namespace { get; set; }
        public string Name { get; set; }
        public bool IsSecure { get; set; }
        public INamedTypeSymbol Symbol { get; set; }

        public IList<string> FilePaths = new List<string>();
        public IList<PageView> Views { get; set; } = new List<PageView>();

        private string _fullyQualifiedGeneratedName = null;
        public string FullyQualifiedGeneratedName
        {
            get => _fullyQualifiedGeneratedName ?? $"{Namespace}.{Name}Model";
            set => _fullyQualifiedGeneratedName = value;
        }
        public string FullyQualifiedR4ClassName { get; set; }

        public string GetFilePath()
        {
            return FilePaths
                .OrderByDescending(f => f.EndsWith(".cshtml.cs") && File.Exists(f.Substring(0, f.Length - 3)))
                .ThenByDescending(f => !f.Contains(".generated.cs"))
                .ThenBy(f => f)
                .FirstOrDefault();
        }

        public string GetPagePath() => Views.FirstOrDefault()?.PagePath;
    }
}
