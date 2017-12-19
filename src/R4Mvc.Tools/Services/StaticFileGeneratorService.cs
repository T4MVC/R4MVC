using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using R4Mvc.Tools.Extensions;
using R4Mvc.Tools.Locators;
using static R4Mvc.Tools.Extensions.SyntaxNodeHelpers;

namespace R4Mvc.Tools.Services
{
    public class StaticFileGeneratorService : IStaticFileGeneratorService
    {
        private readonly IEnumerable<IStaticFileLocator> _staticFileLocators;
        private readonly Settings _settings;

        public StaticFileGeneratorService(IEnumerable<IStaticFileLocator> staticFileLocators, Settings settings)
        {
            _staticFileLocators = staticFileLocators;
            _settings = settings;
        }

        public MemberDeclarationSyntax GenerateStaticFiles(string projectRoot)
        {
            var staticFilesRoot = GetStaticFilesPath(projectRoot);
            var staticfiles = _staticFileLocators.SelectMany(x => x.Find(staticFilesRoot))
                .Where(r=> _settings.IncludeContainer(r.Container));

            var linksClass = SyntaxFactory.ClassDeclaration(_settings.LinksNamespace)
                .WithModifiers(SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword, SyntaxKind.PartialKeyword)
                .WithGeneratedNonUserCodeAttributes();
            linksClass = AddStaticFiles(linksClass, string.Empty, staticfiles);
            return linksClass;
        }

        // This will eventually read the Startup class, to identify the location(s) of the static roots
        public string GetStaticFilesPath(string projectRoot) => Path.Combine(projectRoot, _settings.wwwroot);

        public ClassDeclarationSyntax AddStaticFiles(ClassDeclarationSyntax parentClass, string path, IEnumerable<StaticFile> files)
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
                var className = childPath.Substring(path.Length > 0 ? path.Length + 1 : 0).SanitiseFieldName();
                var containerClass = SyntaxFactory.ClassDeclaration(className)
                    .WithModifiers(SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword, SyntaxKind.PartialKeyword)
                    .WithGeneratedNonUserCodeAttributes();
                containerClass = AddStaticFiles(containerClass, childPath, childFiles);
                parentClass = parentClass.AddMembers(containerClass);
            }

            var localFiles = files.Where(f => f.Container == path);
            foreach (var file in localFiles)
            {
                parentClass = parentClass.AddMembers(
                    CreateStringFieldDeclaration(file.FileName.SanitiseFieldName(), "~/" + file.RelativePath.ToString(), SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword));
            }
            return parentClass;
        }
    }
}
