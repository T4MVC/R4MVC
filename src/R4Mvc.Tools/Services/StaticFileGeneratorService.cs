using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Options;
using R4Mvc.Tools.Extensions;
using R4Mvc.Tools.Locators;
using static R4Mvc.Tools.Extensions.SyntaxNodeHelpers;

namespace R4Mvc.Tools.Services
{
    public class StaticFileGeneratorService : IStaticFileGeneratorService
    {
        private readonly IEnumerable<IStaticFileLocator> _staticFileLocators;
        private readonly Settings _settings;

        public StaticFileGeneratorService(IEnumerable<IStaticFileLocator> staticFileLocators, IOptions<Settings> settings)
        {
            _staticFileLocators = staticFileLocators;
            _settings = settings.Value;
        }

        public MemberDeclarationSyntax GenerateStaticFiles(string projectRoot)
        {
            var staticFilesRoot = Path.Combine(projectRoot, "wwwroot");
            var staticfiles = _staticFileLocators.SelectMany(x => x.Find(staticFilesRoot));

            var linksClass = CreateClass(_settings.LinksNamespace, null, SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword, SyntaxKind.PartialKeyword)
                .WithAttributes(CreateGeneratedCodeAttribute(), CreateDebugNonUserCodeAttribute());
            linksClass = AddStaticFiles(linksClass, string.Empty, staticfiles);
            return linksClass;
        }

        private string SanitiseName(string name)
        {
            name = Regex.Replace(name, @"[\W\b]", "_", RegexOptions.IgnoreCase);
            name = Regex.Replace(name, @"^\d", @"_$0");

            int i = 0;
            while (SyntaxFacts.GetKeywordKind(name) != SyntaxKind.None ||
                SyntaxFacts.GetContextualKeywordKind(name) != SyntaxKind.None ||
                !SyntaxFacts.IsValidIdentifier(name))
            {
                if (i++ > 10)
                    return name; // Sanity check.. The look might be loopy!
                name = "_" + name;
            }
            return name;
        }

        private ClassDeclarationSyntax AddStaticFiles(ClassDeclarationSyntax parentClass, string path, IEnumerable<StaticFile> files)
        {
            var paths = files
                .Select(f => f.Container)
                .Distinct()
                .OrderBy(p => p)
                .Where(c => c.StartsWith(path) && c.Length > path.Length)
                .Select(c =>
                {
                    var index = c.IndexOf('/', path.Length > 0 ? path.Length + 1 : 0);
                    if (index == -1)
                        return c;
                    return c.Substring(0, index);
                })
                .Distinct();

            foreach (var childPath in paths)
            {
                var childFiles = files.Where(f => f.Container.StartsWith(childPath));
                var className = SanitiseName(childPath.Substring(path.Length > 0 ? path.Length + 1 : 0));
                var containerClass = CreateClass(className, null, SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword, SyntaxKind.PartialKeyword)
                    .WithAttributes(CreateGeneratedCodeAttribute(), CreateDebugNonUserCodeAttribute());
                containerClass = AddStaticFiles(containerClass, childPath, childFiles);
                parentClass = parentClass.AddMembers(containerClass);
            }

            var localFiles = files.Where(f => f.Container == path);
            foreach (var file in localFiles)
            {
                parentClass = parentClass.AddMembers(
                    CreateStringFieldDeclaration(SanitiseName(file.FileName), "~/" + file.RelativePath.ToString(), SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword));
            }
            return parentClass;
        }
    }
}
