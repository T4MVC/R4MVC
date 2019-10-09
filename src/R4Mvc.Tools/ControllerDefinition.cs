using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace R4Mvc.Tools
{
    [DebuggerDisplay("Controller: {Area} {Name}")]
    public class ControllerDefinition
    {
        public string Namespace { get; set; }
        public string Name { get; set; }
        public string Area { get; set; }
        public bool IsSecure { get; set; }
        public INamedTypeSymbol Symbol { get; set; }

        public string AreaKey { get; set; }
        public IList<string> FilePaths = new List<string>();
        public IList<View> Views { get; set; } = new List<View>();

        private string _fullyQualifiedGeneratedName = null;
        public string FullyQualifiedGeneratedName
        {
            get => _fullyQualifiedGeneratedName ?? $"{Namespace}.{Name}Controller";
            set => _fullyQualifiedGeneratedName = value;
        }
        public string FullyQualifiedR4ClassName { get; set; }

        public string GetFilePath()
        {
            return FilePaths
                .OrderByDescending(f => Area == null || f.Contains($"/Areas/{Area}/Controllers/") || f.Contains($"\\Areas\\{Area}\\Controllers\\"))
                .ThenByDescending(f => f.Contains("/Controllers/") || f.Contains("\\Controllers\\"))
                .ThenByDescending(f => !f.Contains(".generated.cs"))
                .ThenBy(f => f)
                .First();
        }
    }
}
