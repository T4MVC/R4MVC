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
        public PageDefinition(string cNamespace, string name, bool isSecure, INamedTypeSymbol symbol, List<string> filePaths)
        {
            Namespace = cNamespace;
            Name = name;
            IsSecure = isSecure;
            Symbol = symbol;
            FilePaths = filePaths;
        }

        public string Namespace { get; }
        public string Name { get; }
        public bool IsSecure { get; }
        public INamedTypeSymbol Symbol { get; }

        public IList<string> FilePaths = new List<string>();

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
    }
}
