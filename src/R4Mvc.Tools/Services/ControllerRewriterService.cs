using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Options;
using R4Mvc.Tools.Extensions;

namespace R4Mvc.Tools.Services
{
    public class ControllerRewriterService : IControllerRewriterService
    {
        private readonly IFilePersistService _filePersistService;
        private readonly IControllerGeneratorService _controllerGenerator;
        private readonly Settings _settings;
        public ControllerRewriterService(IFilePersistService filePersistService, IControllerGeneratorService controllerGenerator, IOptions<Settings> settings)
        {
            _filePersistService = filePersistService;
            _controllerGenerator = controllerGenerator;
            _settings = settings.Value;
        }

        public IList<ControllerDefinition> RewriteControllers(CSharpCompilation compiler)
        {
            var controllers = new Dictionary<string, ControllerDefinition>();

            foreach (var tree in compiler.SyntaxTrees.Where(x => !x.FilePath.EndsWith(".generated.cs")))
            {
                // if syntaxtree has errors, skip code generation
                if (tree.GetDiagnostics().Any(x => x.Severity == DiagnosticSeverity.Error)) continue;

                // this first part, finds all the controller classes, modifies them and saves the changes
                var controllerRewriter = new ControllerRewriter(compiler);
                var newNode = controllerRewriter.Visit(tree.GetRoot());

                // save the controller nodes from each visit to pass to the generator
                foreach (var controllerNode in controllerRewriter.MvcControllerClassNodes)
                {
                    var cNamespace = controllerNode.FirstAncestorOrSelf<NamespaceDeclarationSyntax>().Name.ToFullString().Trim();
                    var cSymbol = compiler.GetSemanticModel(tree).GetDeclaredSymbol(controllerNode);
                    var cFullName = cNamespace + "." + cSymbol.Name;
                    if (controllers.ContainsKey(cFullName))
                    {
                        controllers[cFullName].FilePaths.Add(tree.FilePath);
                        continue;
                    }

                    var cAreaName = _controllerGenerator.GetControllerArea(cSymbol);
                    controllers[cFullName] = new ControllerDefinition
                    {
                        Namespace = cNamespace,
                        Name = cSymbol.Name.TrimEnd("Controller"),
                        Area = cAreaName,
                        Symbol = cSymbol,
                        FilePaths = new List<string> { tree.FilePath },
                    };
                }

                if (!newNode.IsEquivalentTo(tree.GetRoot()))
                {
                    // node has changed, update syntaxtree and persist to file
                    compiler = compiler.ReplaceSyntaxTree(tree, newNode.SyntaxTree);
                    _filePersistService.WriteFile(newNode, tree.FilePath);
                }
            }

            return controllers.Values.ToList();
        }
    }
}
