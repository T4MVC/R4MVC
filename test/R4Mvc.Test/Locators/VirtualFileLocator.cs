using System;
using System.Linq;
using R4Mvc.Tools.Locators;

namespace R4Mvc.Test.Locators
{
    class VirtualFileLocator : IFileLocator
    {
        private readonly string[] _paths;
        public VirtualFileLocator(string[] paths)
        {
            _paths = paths;
        }

        public bool DirectoryExists(string path)
            => _paths.Any(p => p.StartsWith(path.TrimEnd('\\') + "\\"));

        public string[] GetDirectories(string parentPath)
            => _paths
                .Where(p => p.StartsWith(parentPath))
                .Where(p => p.Substring(parentPath.Length).TrimStart('\\').Count(c => c == '\\') >= 1)
                .Select(p => p.Substring(0, p.Substring(parentPath.Length + 1).IndexOf('\\') + parentPath.Length + 1))
                .Distinct()
                .ToArray();

        public string[] GetFiles(string parentPath, string filter, bool recurse = false)
            => _paths
                .Where(p => p.StartsWith(parentPath))
                .Where(p =>
                {
                    switch (filter)
                    {
                        case "*": return true;
                        case "*.cshtml": return p.EndsWith(".cshtml");
                        default: throw new NotSupportedException();
                    }
                })
                .Where(p => recurse || !p.Substring(parentPath.Length).TrimStart('\\').Contains("\\"))
                .ToArray();
    }
}
