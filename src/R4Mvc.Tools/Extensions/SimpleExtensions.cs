using System.IO;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.CSharp;

namespace R4Mvc.Tools.Extensions
{
    public static class SimpleExtensions
    {
        public static string TrimEnd(this string value, string suffix)
        {
            if (value == null)
                return null;
            if (!value.EndsWith(suffix))
                return value;
            return value.Substring(0, value.Length - suffix.Length);
        }

        public static string SanitiseFieldName(this string name)
        {
            name = Regex.Replace(name, @"[\W\b]", "_", RegexOptions.IgnoreCase);
            name = Regex.Replace(name, @"^\d", @"_$0");

            int i = 0;
            while (SyntaxFacts.GetKeywordKind(name) != SyntaxKind.None ||
                SyntaxFacts.GetContextualKeywordKind(name) != SyntaxKind.None ||
                !SyntaxFacts.IsValidIdentifier(name))
            {
                if (i++ > 10)
                    return name; // Sanity check.. The loop might be loopy!
                name = "_" + name;
            }
            return name;
        }

        public static string GetRelativePath(this string path, string rootPath)
        {
            path = Path.GetFullPath(path);
            rootPath = Path.GetFullPath(rootPath);
            if (!path.StartsWith(rootPath))
                return path;
            return path.Substring(rootPath.Length);
        }
    }
}
