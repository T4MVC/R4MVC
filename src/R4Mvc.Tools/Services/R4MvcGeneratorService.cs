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
        private readonly IStaticFileGeneratorService _staticFileGenerator;
        private readonly IFilePersistService _filePersistService;
        private readonly Settings _settings;

        public R4MvcGeneratorService(
            IControllerRewriterService controllerRewriter,
            IControllerGeneratorService controllerGenerator,
            IStaticFileGeneratorService staticFileGenerator,
            IFilePersistService filePersistService,
            Settings settings)
        {
            _controllerRewriter = controllerRewriter;
            _controllerGenerator = controllerGenerator;
            _staticFileGenerator = staticFileGenerator;
            _filePersistService = filePersistService;
            _settings = settings;
        }

        public void Generate(string projectRoot, IList<ControllerDefinition> controllers)
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
                        var controllerFile = new CodeFileBuilder(_settings)
                            .WithNamespace(namespaceNode);
                        _filePersistService.WriteFile(controllerFile.Build(), generatedFilePath);
                        namespaceNode = NamespaceDeclaration(ParseName(namespaceGroup.Key));
                    }
                }

                // If SplitIntoMultipleFiles is NOT set, bundle them all in R4Mvc
                if (!_settings.SplitIntoMultipleFiles)
                    generatedControllers.Add(namespaceNode);
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
                    .WithField(Constants.DummyClassInstance, Constants.DummyClass, SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword)
                    .Build())
                .AddMembers(CreateViewOnlyControllerClasses(controllers).ToArray<MemberDeclarationSyntax>())
                .AddMembers(CreateAreaClasses(areaControllers).ToArray<MemberDeclarationSyntax>());

            // create static MVC class and add the area and controller fields
            var mvcStaticClass = new ClassBuilder(_settings.HelpersPrefix)
                .WithModifiers(SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword, SyntaxKind.PartialKeyword)
                .WithGeneratedNonUserCodeAttributes();
            foreach (var area in areaControllers.Where(a => !string.IsNullOrEmpty(a.Key)).OrderBy(a => a.Key))
            {
                mvcStaticClass.WithStaticFieldBackedProperty(area.First().AreaKey, $"{_settings.R4MvcNamespace}.{area.Key}AreaClass", SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword);
            }
            foreach (var controller in areaControllers[string.Empty].OrderBy(c => c.Namespace == null).ThenBy(c => c.Name))
            {
                mvcStaticClass.WithFieldInitialised(
                    controller.Name,
                    controller.FullyQualifiedGeneratedName,
                    controller.FullyQualifiedR4ClassName ?? controller.FullyQualifiedGeneratedName,
                    SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword, SyntaxKind.ReadOnlyKeyword);
            }

            // Generate a list of all static files from the wwwroot path
            var staticFileNode = _staticFileGenerator.GenerateStaticFiles(projectRoot);

            var r4MvcFile = new CodeFileBuilder(_settings)
                    .WithMembers(
                        mvcStaticClass.Build(),
                        r4Namespace,
                        staticFileNode,
                        ActionResultClass(),
                        JsonResultClass(),
                        ContentResultClass(),
                        RedirectResultClass(),
                        RedirectToActionResultClass(),
                        RedirectToRouteResultClass())
                    .WithNamespaces(generatedControllers);
            Console.WriteLine("Generating " + Path.DirectorySeparatorChar + Constants.R4MvcFileName);
            _filePersistService.WriteFile(r4MvcFile.Build(), Path.Combine(projectRoot, Constants.R4MvcFileName));
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
                    .WithModifiers(SyntaxKind.PublicKeyword, SyntaxKind.PartialKeyword);
                _controllerGenerator.WithViewsClass(controllerClass, controller.Views);
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
                        .WithFieldInitialised(
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
                .WithModifiers(SyntaxKind.InternalKeyword, SyntaxKind.PartialKeyword)
                .WithBaseTypes(baseClassName, "IR4MvcActionResult")                     // : {baseClassName}, IR4MvcActionResult
                .WithConstructor(c => c
                    .WithOther(constructorParts)
                    .WithModifiers(SyntaxKind.PublicKeyword)                        // public ctor(
                    .WithStringParameter("area")                                    //  string area,
                    .WithStringParameter("controller")                              //  string controller,
                    .WithStringParameter("action")                                  //  string action,
                    .WithStringParameter("protocol", defaultsToNull: true)          //  string protocol = null)
                    .WithBody(b => b                                                // this.InitMVCT4Result(area, controller, action, protocol);
                        .MethodCall("this", "InitMVCT4Result", "area", "controller", "action", "protocol")))
                .WithStringProperty("Controller")                                       // public string Controller { get; set; }
                .WithStringProperty("Action")                                           // public string Action { get; set; }
                .WithStringProperty("Protocol")                                         // public string Protocol { get; set; }
                .WithProperty("RouteValueDictionary", "RouteValueDictionary");          // public RouteValueDictionary RouteValueDictionary { get; set; }

            return result.Build();
        }

        public ClassDeclarationSyntax ActionResultClass()
            => IActionResultDerivedClass(Constants.ActionResultClass, "ActionResult");

        public ClassDeclarationSyntax JsonResultClass()
            => IActionResultDerivedClass(Constants.JsonResultClass, "JsonResult",
                c => c.WithBaseConstructorCall(p => p.Null));                           // ctor : base(null)

        public ClassDeclarationSyntax ContentResultClass()
            => IActionResultDerivedClass(Constants.ContentResultClass, "ContentResult");

        public ClassDeclarationSyntax RedirectResultClass()
            => IActionResultDerivedClass(Constants.RedirectResultClass, "RedirectResult",
                c => c.WithBaseConstructorCall(p => p.Space));                          // ctor : base(" ")

        public ClassDeclarationSyntax RedirectToActionResultClass()
            => IActionResultDerivedClass(Constants.RedirectToActionResultClass, "RedirectToActionResult",
                c => c.WithBaseConstructorCall(p => p.Space, p => p.Space, p => p.Space));  // ctor : base(" ", " ", " ")

        public ClassDeclarationSyntax RedirectToRouteResultClass()
            => IActionResultDerivedClass(Constants.RedirectToRouteResultClass, "RedirectToRouteResult",
                c => c.WithBaseConstructorCall(p => p.Null));                           // ctor : base(null)
    }
}
