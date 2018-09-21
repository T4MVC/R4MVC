using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using R4Mvc.Tools.CodeGen;
using R4Mvc.Tools.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace R4Mvc.Tools.Services
{
    public class R4MvcGeneratorService
    {
        private readonly IControllerRewriterService _controllerRewriter;
        private readonly IControllerGeneratorService _controllerGenerator;
        private readonly IPageGeneratorService _pageGenerator;
        private readonly IStaticFileGeneratorService _staticFileGenerator;
        private readonly IFilePersistService _filePersistService;
        private readonly Settings _settings;

        public R4MvcGeneratorService(
            IControllerRewriterService controllerRewriter,
            IControllerGeneratorService controllerGenerator,
            IPageGeneratorService pageGenerator,
            IStaticFileGeneratorService staticFileGenerator,
            IFilePersistService filePersistService,
            Settings settings)
        {
            _controllerRewriter = controllerRewriter;
            _controllerGenerator = controllerGenerator;
            _pageGenerator = pageGenerator;
            _staticFileGenerator = staticFileGenerator;
            _filePersistService = filePersistService;
            _settings = settings;
        }

        public void Generate(string projectRoot, IList<ControllerDefinition> controllers, IList<PageDefinition> pages)
        {
            var areaControllers = controllers.ToLookup(c => c.Area);

            // Processing controllers, generating partial and derived controller classes for R4Mvc
            var generatedControllers = new List<NamespaceDeclarationSyntax>();
            foreach (var namespaceGroup in controllers.Where(c => c.Namespace != null).GroupBy(c => c.Namespace).OrderBy(c => c.Key))
            {
                var namespaceNode = NamespaceDeclaration(ParseName(namespaceGroup.Key));
                foreach (var controller in namespaceGroup.OrderBy(c => c.Name))
                {
                    namespaceNode = namespaceNode.AddMembers(
                        _controllerGenerator.GeneratePartialController(controller),
                        _controllerGenerator.GenerateR4Controller(controller));

                    // If SplitIntoMultipleFiles is set, store the generated classes alongside the controller files.
                    if (_settings.SplitIntoMultipleFiles)
                    {
                        var generatedFilePath = controller.GetFilePath().TrimEnd(".cs") + ".generated.cs";
                        Console.WriteLine("Generating " + generatedFilePath.GetRelativePath(projectRoot));
                        var controllerFile = new CodeFileBuilder(_settings, true)
                            .WithNamespace(namespaceNode);
                        _filePersistService.WriteFile(controllerFile.Build(), generatedFilePath);
                        namespaceNode = NamespaceDeclaration(ParseName(namespaceGroup.Key));
                    }
                }

                // If SplitIntoMultipleFiles is NOT set, bundle them all in R4Mvc
                if (!_settings.SplitIntoMultipleFiles)
                    generatedControllers.Add(namespaceNode);
            }

            var generatedPages = new List<NamespaceDeclarationSyntax>();
            foreach (var namespaceGroup in pages.Where(p => p.Namespace != null).GroupBy(p => p.Namespace).OrderBy(p => p.Key))
            {
                var namespaceNode = NamespaceDeclaration(ParseName(namespaceGroup.Key));
                foreach (var page in namespaceGroup.OrderBy(p => p.Name))
                {
                    namespaceNode = namespaceNode.AddMembers(
                        _pageGenerator.GeneratePartialPage(page),
                        _pageGenerator.GenerateR4Page(page));

                    // If SplitIntoMultipleFiles is set, store the generated classes alongside the controller files.
                    if (_settings.SplitIntoMultipleFiles)
                    {
                        var generatedFilePath = page.GetFilePath().TrimEnd(".cs") + ".generated.cs";
                        Console.WriteLine("Generating " + generatedFilePath.GetRelativePath(projectRoot));
                        var pageFile = new CodeFileBuilder(_settings, true)
                            .WithNamespace(namespaceNode);
                        _filePersistService.WriteFile(pageFile.Build(), generatedFilePath);
                        namespaceNode = NamespaceDeclaration(ParseName(namespaceGroup.Key));
                    }
                }

                // If SplitIntoMultipleFiles is NOT set, bundle them all in R4Mvc
                if (!_settings.SplitIntoMultipleFiles)
                    generatedPages.Add(namespaceNode);
            }

            // R4MVC namespace used for the areas and Dummy class
            var r4Namespace = NamespaceDeclaration(ParseName(_settings.R4MvcNamespace))
                // add the dummy class uses in the derived controller partial class
                /* [GeneratedCode, DebuggerNonUserCode]
                 * public class Dummy
                 * {
                 *  private Dummy() {}
                 *  public static Dummy Instance = new Dummy();
                 * }
                 */
                 .AddMembers(new ClassBuilder(Constants.DummyClass)
                    .WithModifiers(SyntaxKind.PublicKeyword)
                    .WithGeneratedNonUserCodeAttributes()
                    .WithConstructor(c => c
                        .WithModifiers(SyntaxKind.PrivateKeyword))
                    .WithField(Constants.DummyClassInstance, Constants.DummyClass, Constants.DummyClass, SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword)
                    .Build())
                .AddMembers(CreateViewOnlyControllerClasses(controllers).ToArray<MemberDeclarationSyntax>())
                .AddMembers(CreateViewOnlyPageClasses(pages).ToArray<MemberDeclarationSyntax>())
                .AddMembers(CreateAreaClasses(areaControllers).ToArray<MemberDeclarationSyntax>());

            // create static MVC class and add the area and controller fields
            var mvcStaticClass = new ClassBuilder(_settings.HelpersPrefix)
                .WithModifiers(SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword, SyntaxKind.PartialKeyword)
                .WithGeneratedNonUserCodeAttributes();
            foreach (var area in areaControllers.Where(a => !string.IsNullOrEmpty(a.Key)).OrderBy(a => a.Key))
            {
                mvcStaticClass.WithStaticFieldBackedProperty(area.First().AreaKey, $"{_settings.R4MvcNamespace}.{area.Key}AreaClass", false, SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword);
            }
            foreach (var controller in areaControllers[string.Empty].OrderBy(c => c.Namespace == null).ThenBy(c => c.Name))
            {
                mvcStaticClass.WithField(
                    controller.Name,
                    controller.FullyQualifiedGeneratedName,
                    controller.FullyQualifiedR4ClassName ?? controller.FullyQualifiedGeneratedName,
                    SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword, SyntaxKind.ReadOnlyKeyword);
            }

            // Generate a list of all static files from the wwwroot path
            var staticFileNode = _staticFileGenerator.GenerateStaticFiles(projectRoot);

            var r4MvcFile = new CodeFileBuilder(_settings, true)
                    .WithMembers(
                        mvcStaticClass.Build(),
                        r4Namespace,
                        staticFileNode,
                        R4MvcHelpersClass(),
                        ActionResultClass(),
                        JsonResultClass(),
                        ContentResultClass(),
                        FileResultClass(),
                        RedirectResultClass(),
                        RedirectToActionResultClass(),
                        RedirectToRouteResultClass())
                    .WithNamespaces(generatedControllers)
                    .WithNamespaces(generatedPages);
            Console.WriteLine("Generating " + Path.DirectorySeparatorChar + Constants.R4MvcGeneratedFileName);
            _filePersistService.WriteFile(r4MvcFile.Build(), Path.Combine(projectRoot, Constants.R4MvcGeneratedFileName));
        }

        public IEnumerable<ClassDeclarationSyntax> CreateViewOnlyControllerClasses(IList<ControllerDefinition> controllers)
        {
            foreach (var controller in controllers.Where(c => c.Namespace == null).OrderBy(c => c.Area).ThenBy(c => c.Name))
            {
                var className = !string.IsNullOrEmpty(controller.Area)
                    ? $"{controller.Area}Area_{controller.Name}Controller"
                    : $"{controller.Name}Controller";
                controller.FullyQualifiedGeneratedName = $"{_settings.R4MvcNamespace}.{className}";

                var controllerClass = new ClassBuilder(className)
                    .WithModifiers(SyntaxKind.PublicKeyword, SyntaxKind.PartialKeyword)
                    .WithGeneratedNonUserCodeAttributes();
                _controllerGenerator.WithViewsClass(controllerClass, controller.Views);
                yield return controllerClass.Build();
            }
        }

        public IEnumerable<ClassDeclarationSyntax> CreateViewOnlyPageClasses(IList<PageDefinition> pages)
        {
            foreach (var page in pages.Where(c => c.Namespace == null).OrderBy(c => c.GetFilePath()))
            {
                var view = page.Views.FirstOrDefault();
                if (view == null)
                    continue;

                var className = string.Join("_", view.Segments) + "Model";
                page.FullyQualifiedGeneratedName = $"{_settings.R4MvcNamespace}.{className}";

                var controllerClass = new ClassBuilder(className)
                    .WithModifiers(SyntaxKind.PublicKeyword, SyntaxKind.PartialKeyword)
                    .WithGeneratedNonUserCodeAttributes();
                _controllerGenerator.WithViewsClass(controllerClass, page.Views);
                yield return controllerClass.Build();
            }
        }

        public IEnumerable<ClassDeclarationSyntax> CreateAreaClasses(ILookup<string, ControllerDefinition> areaControllers)
        {
            foreach (var area in areaControllers.Where(a => !string.IsNullOrEmpty(a.Key)).OrderBy(a => a.Key))
            {
                var areaClass = new ClassBuilder(area.Key + "AreaClass")
                    .WithModifiers(SyntaxKind.PublicKeyword, SyntaxKind.PartialKeyword)
                    .WithGeneratedNonUserCodeAttributes()
                    .WithStringField("Name", area.Key, false, SyntaxKind.PublicKeyword, SyntaxKind.ReadOnlyKeyword)
                    .ForEach(area.OrderBy(c => c.Namespace == null).ThenBy(c => c.Name), (cb, c) => cb
                        .WithField(
                            c.Name,
                            c.FullyQualifiedGeneratedName,
                            c.FullyQualifiedR4ClassName ?? c.FullyQualifiedGeneratedName,
                            SyntaxKind.PublicKeyword, SyntaxKind.ReadOnlyKeyword));
                yield return areaClass.Build();
            }
        }

        private ClassDeclarationSyntax IActionResultDerivedClass(string className, string baseClassName, Action<ConstructorMethodBuilder> constructorParts = null)
        {
            var result = new ClassBuilder(className)                                    // internal partial class {className}
                .WithGeneratedNonUserCodeAttributes()
                .WithModifiers(SyntaxKind.InternalKeyword, SyntaxKind.PartialKeyword)
                .WithBaseTypes(baseClassName, "IR4MvcActionResult")                     // : {baseClassName}, IR4MvcActionResult
                .WithConstructor(c => c
                    .WithOther(constructorParts)
                    .WithModifiers(SyntaxKind.PublicKeyword)                        // public ctor(
                    .WithParameter("area", "string")                                //  string area,
                    .WithParameter("controller", "string")                          //  string controller,
                    .WithParameter("action", "string")                              //  string action,
                    .WithParameter("protocol", "string", defaultsToNull: true)      //  string protocol = null)
                    .WithBody(b => b                                                    // this.InitMVCT4Result(area, controller, action, protocol);
                        .MethodCall("this", "InitMVCT4Result", "area", "controller", "action", "protocol")))
                .WithProperty("Controller", "string")                                   // public string Controller { get; set; }
                .WithProperty("Action", "string")                                       // public string Action { get; set; }
                .WithProperty("Protocol", "string")                                     // public string Protocol { get; set; }
                .WithProperty("RouteValueDictionary", "RouteValueDictionary");          // public RouteValueDictionary RouteValueDictionary { get; set; }

            return result.Build();
        }

        public ClassDeclarationSyntax R4MvcHelpersClass()
            => new ClassBuilder(Constants.R4MvcHelpersClass)
                .WithModifiers(SyntaxKind.InternalKeyword, SyntaxKind.StaticKeyword)
                .WithGeneratedNonUserCodeAttributes()
                .WithMethod(Constants.R4MvcHelpers_ProcessVirtualPath + "Default", "string", m => m
                    .WithModifiers(SyntaxKind.PrivateKeyword, SyntaxKind.StaticKeyword)
                    .WithParameter("virtualPath", "string")
                    .WithExpresisonBody(IdentifierName("virtualPath")))
                .WithValueField(Constants.R4MvcHelpers_ProcessVirtualPath, "Func<string, string>", Constants.R4MvcHelpers_ProcessVirtualPath + "Default", SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword)
                .Build();

        public ClassDeclarationSyntax ActionResultClass()
            => IActionResultDerivedClass(Constants.ActionResultClass, "ActionResult");

        public ClassDeclarationSyntax JsonResultClass()
            => IActionResultDerivedClass(Constants.JsonResultClass, "JsonResult",
                c => c.WithBaseConstructorCall(SimpleLiteral.Null));                           // ctor : base(null)

        public ClassDeclarationSyntax ContentResultClass()
            => IActionResultDerivedClass(Constants.ContentResultClass, "ContentResult");

        public ClassDeclarationSyntax FileResultClass()
            => IActionResultDerivedClass(Constants.FileResultClass, "FileResult",
                c => c.WithBaseConstructorCall(SimpleLiteral.Null));                           // ctor : base(null)

        public ClassDeclarationSyntax RedirectResultClass()
            => IActionResultDerivedClass(Constants.RedirectResultClass, "RedirectResult",
                c => c.WithBaseConstructorCall(SimpleLiteral.Space));                          // ctor : base(" ")

        public ClassDeclarationSyntax RedirectToActionResultClass()
            => IActionResultDerivedClass(Constants.RedirectToActionResultClass, "RedirectToActionResult",
                c => c.WithBaseConstructorCall(SimpleLiteral.Space, SimpleLiteral.Space, SimpleLiteral.Space));  // ctor : base(" ", " ", " ")

        public ClassDeclarationSyntax RedirectToRouteResultClass()
            => IActionResultDerivedClass(Constants.RedirectToRouteResultClass, "RedirectToRouteResult",
                c => c.WithBaseConstructorCall(SimpleLiteral.Null));                           // ctor : base(null)
    }
}
