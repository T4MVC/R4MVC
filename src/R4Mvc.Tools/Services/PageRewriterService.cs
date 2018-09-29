using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using R4Mvc.Tools.Extensions;

namespace R4Mvc.Tools.Services
{
    public class PageRewriterService : IPageRewriterService
    {
        private readonly IFilePersistService _filePersistService;
        private readonly Settings _settings;
        public PageRewriterService(IFilePersistService filePersistService, Settings settings)
        {
            _filePersistService = filePersistService;
            _settings = settings;
        }

        public IList<PageDefinition> RewritePages(CSharpCompilation compiler)
        {
            var pages = new Dictionary<string, PageDefinition>();

            foreach (var tree in compiler.SyntaxTrees.Where(x => !x.FilePath.EndsWith(".generated.cs")))
            {
                // if syntaxtree has errors, skip code generation
                if (tree.GetDiagnostics().Any(x => x.Severity == DiagnosticSeverity.Error)) continue;

                // this first part, finds all the controller classes, modifies them and saves the changes
                var controllerRewriter = new PageRewriter(compiler);
                SyntaxNode newNode;
                try
                {
                    newNode = controllerRewriter.Visit(tree.GetRoot());
                }
                catch
                {
                    // If roslyn can't get the root of the tree, just continue to the next item
                    continue;
                }

                // save the controller nodes from each visit to pass to the generator
                foreach (var controllerNode in controllerRewriter.MvcPageClassNodes)
                {
                    var cNamespace = controllerNode.FirstAncestorOrSelf<NamespaceDeclarationSyntax>().Name.ToFullString().Trim();
                    var cSymbol = compiler.GetSemanticModel(tree).GetDeclaredSymbol(controllerNode);
                    var cFullName = cNamespace + "." + cSymbol.Name;
                    if (pages.ContainsKey(cFullName))
                    {
                        pages[cFullName].FilePaths.Add(tree.FilePath);
                        continue;
                    }
                    var isSecure = cSymbol.GetAttributes().Any(a => a.AttributeClass.InheritsFrom<RequireHttpsAttribute>());

                    pages[cFullName] = new PageDefinition(
                        cNamespace,
                        cSymbol.Name.TrimEnd("Model"),
                        isSecure,
                        cSymbol,
                        new List<string> { tree.FilePath });
                }

                if (!newNode.IsEquivalentTo(tree.GetRoot()))
                {
                    // node has changed, update syntaxtree and persist to file

                    // Updating the new syntax tree with the syntax options from the original tree
                    // Seems like the new syntax tree might be generated with a different language version than the original. (see #79)
                    var newTree = newNode.SyntaxTree.WithRootAndOptions(newNode.SyntaxTree.GetRoot(), tree.Options);

                    compiler = compiler.ReplaceSyntaxTree(tree, newTree);
                    _filePersistService.WriteFile(newNode, tree.FilePath);
                }
            }

            return pages.Values.ToList();
        }
    }
}
