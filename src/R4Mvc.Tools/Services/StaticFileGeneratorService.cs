using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using R4Mvc.Tools.Extensions;
using R4Mvc.Tools.Locators;
using System.Collections.Generic;
using System.Linq;
using static R4Mvc.Tools.Extensions.SyntaxNodeHelpers;

namespace R4Mvc.Tools.Services
{
    public class StaticFileGeneratorService : IStaticFileGeneratorService
    {
        private readonly IEnumerable<IStaticFileLocator> _staticFileLocators;

        public StaticFileGeneratorService(IEnumerable<IStaticFileLocator> staticFileLocators)
        {
            _staticFileLocators = staticFileLocators;
        }

        public MemberDeclarationSyntax GenerateStaticFiles(Settings settings)
        {
            var staticfiles = _staticFileLocators.SelectMany(x => x.Find());
            staticfiles = SanitiseFileNamesWithNoConflicts(staticfiles);

            // create static Links class (scripts, content, bundles?)
            var linksNamespace = CreateNamespace(settings.LinksNamespace);
            foreach (var grouping in staticfiles.GroupBy(x => x.CollectionName))
            {
                var sanitizedGrouping = SanitiseFileNamesWithNoConflicts(grouping);
                var groupNode = CreateClass(grouping.Key.Segments.Last(), null, SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword)
                        .WithAttributes(CreateGeneratedCodeAttribute(), CreateDebugNonUserCodeAttribute())
                        .WithStaticFieldsForFiles(sanitizedGrouping)
                        .WithUrlMethods()
                        .WithStringField("URLPATH", "~/" + grouping.Key, false, SyntaxKind.PrivateKeyword, SyntaxKind.ConstKeyword);

                linksNamespace = linksNamespace.AddMembers(groupNode);
            }
            return linksNamespace;
        }

        private IEnumerable<StaticFile> SanitiseFileNamesWithNoConflicts(IEnumerable<StaticFile> staticFiles)
        {
            // TODO t4mvc used codeProvider.CreateEscapedIdentifier
            // need to find robust method of creating valid member names, not only invalid chars but also for reserved language tokens
            return staticFiles;
        }
    }
}
