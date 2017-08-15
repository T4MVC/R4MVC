using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace R4Mvc.Tools
{
    public class ControllerDefinition
    {
        public string Namespace { get; set; }
        public string Name { get; set; }
        public string Area { get; set; }
        public INamedTypeSymbol Symbol { get; set; }

        public IList<string> FilePaths = new List<string>();

        public string GetFilePath()
        {
            return FilePaths
                .OrderByDescending(f => f.Contains("/Controllers/"))
                .OrderBy(f => f)
                .FirstOrDefault();
        }
    }
}
