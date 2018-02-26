using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using R4Mvc.Tools.CodeGen;
using R4Mvc.Tools.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static R4Mvc.Tools.Extensions.SyntaxNodeHelpers;

namespace R4Mvc.Tools.Services
{
    public class ControllerGeneratorService : IControllerGeneratorService
    {
        private readonly Settings _settings;

        public ControllerGeneratorService(Settings settings)
        {
            _settings = settings;
        }

        public string GetControllerArea(INamedTypeSymbol controllerSymbol)
        {
            AttributeData areaAttribute = null;
            var typeSymbol = controllerSymbol;
            while (typeSymbol != null && areaAttribute == null)
            {
                areaAttribute = typeSymbol.GetAttributes().FirstOrDefault(a => a.AttributeClass.InheritsFrom<AreaAttribute>());
                typeSymbol = typeSymbol.BaseType;
            }
            if (areaAttribute == null)
                return string.Empty;

            if (areaAttribute.AttributeClass.ToDisplayString() == typeof(AreaAttribute).FullName)
                return areaAttribute.ConstructorArguments[0].Value?.ToString();

            // parse the constructor to get the area name from derived types
            if (areaAttribute.AttributeClass.BaseType.ToDisplayString() == typeof(AreaAttribute).FullName)
            {
                // direct descendant. Reading the area name from the constructor
                var constructorInit = areaAttribute.AttributeConstructor.DeclaringSyntaxReferences
                    .SelectMany(s => s.SyntaxTree.GetRoot().DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>().Where(c => c.Identifier.Text == areaAttribute.AttributeClass.Name))
                    .SelectMany(s => s.DescendantNodesAndSelf().OfType<ConstructorInitializerSyntax>())
                    .First();
                if (constructorInit.ArgumentList.Arguments.Count > 0)
                {
                    var arg = constructorInit.ArgumentList.Arguments[0];
                    if (arg.Expression is LiteralExpressionSyntax litExp)
                    {
                        return litExp.Token.ValueText;
                    }
                }
            }
            return string.Empty;
        }

        public ClassDeclarationSyntax GeneratePartialController(ControllerDefinition controller)
        {
            // build controller partial class node 
            // add a default constructor if there are some but none are zero length
            var genControllerClass2 = new ClassBuilder(controller.Symbol.Name)              // public partial {controllerClass}
                .WithModifiers(SyntaxKind.PublicKeyword, SyntaxKind.PartialKeyword)
                .WithTypeParameters(controller.Symbol.TypeParameters.Select(tp => tp.Name).ToArray()); // optional <T1, T2, …>

            var gotCustomConstructors = controller.Symbol.Constructors
                .Where(c => c.DeclaredAccessibility == Accessibility.Public)
                .Where(SyntaxNodeHelpers.IsNotR4MVCGenerated)
                .Where(c => !c.IsImplicitlyDeclared)
                .Any();
            if (!gotCustomConstructors)
                genControllerClass2.WithConstructor(c => c                                  // public ctor() { }
                    .WithModifiers(SyntaxKind.PublicKeyword)
                    .WithGeneratedNonUserCodeAttributes());                                 // [GeneratedCode, DebuggerNonUserCode]
            genControllerClass2.WithConstructor(c => c                                      // public ctor(Dummy d) {}
                .WithModifiers(SyntaxKind.ProtectedKeyword)
                .WithGeneratedNonUserCodeAttributes()                                       // [GeneratedCode, DebuggerNonUserCode]
                .WithParameter("d", Constants.DummyClass));

            AddRedirectMethods(genControllerClass2);
            AddParameterlessMethods(genControllerClass2, controller.Symbol);

            var actionsExpression = controller.AreaKey != null
                ? SyntaxNodeHelpers.MemberAccess(_settings.HelpersPrefix + "." + controller.AreaKey, controller.Name)
                : SyntaxNodeHelpers.MemberAccess(_settings.HelpersPrefix, controller.Name);
            genControllerClass2
                .WithExpressionProperty("Actions", controller.Symbol.Name, actionsExpression, SyntaxKind.PublicKeyword)
                .WithStringField("Area", controller.Area, true, SyntaxKind.PublicKeyword, SyntaxKind.ReadOnlyKeyword)
                .WithStringField("Name", controller.Name, true, SyntaxKind.PublicKeyword, SyntaxKind.ReadOnlyKeyword)
                .WithStringField("NameConst", controller.Name, true, SyntaxKind.PublicKeyword, SyntaxKind.ConstKeyword)
                .WithStaticFieldBackedProperty("ActionNames", "ActionNamesClass", SyntaxKind.PublicKeyword);


            var genControllerClass = genControllerClass2.Build();

            // add all method stubs, TODO criteria for this: only public virtual actionresults?
            // add subclasses, fields, properties, constants for action names
            genControllerClass =
                genControllerClass
                    .WithActionNameClass(controller.Symbol)
                    .WithActionConstantsClass(controller.Symbol)
                    .WithViewsClass(controller.Name, controller.Area, controller.Views);

            return genControllerClass;
        }

        public ClassDeclarationSyntax GenerateR4Controller(ControllerDefinition controller)
        {
            // create R4MVC_[Controller] class inheriting from partial
            var className = GetR4MVCControllerClassName(controller.Symbol);
            controller.FullyQualifiedR4ClassName = $"{controller.Namespace}.{className}";

            var r4ControllerClass = new ClassBuilder(className)
                .WithModifiers(SyntaxKind.PublicKeyword, SyntaxKind.PartialKeyword)
                .WithGeneratedNonUserCodeAttributes()
                .WithBaseTypes(controller.Symbol.ToQualifiedName())
                .WithConstructor(c => c
                    .WithBaseConstructorCall(m => IdentifierName(Constants.DummyClass + "." + Constants.DummyClassInstance))
                    .WithModifiers(SyntaxKind.PublicKeyword));
            AddMethodOverrides(r4ControllerClass, controller.Symbol);
            return r4ControllerClass.Build();
        }

        private void AddRedirectMethods(ClassBuilder genControllerClass)
        {
            genControllerClass
                .WithMethod("RedirectToAction", "RedirectToRouteResult", m => m
                    .WithModifiers(SyntaxKind.ProtectedKeyword)
                    .WithGeneratedNonUserCodeAttributes()
                    .WithParameter("result", "IActionResult")
                    .WithBody(b => b
                        .VariableFromMethodCall("callInfo", "result", "GetR4MvcResult")                 // var callInfo = result.GetR4MvcResult();
                        .ReturnMethodCall(null, "RedirectToRoute", "callInfo.RouteValueDictionary")))   // return RedirectToRoute(callInfo.RouteValueDictionary);
                .WithMethod("RedirectToAction", "RedirectToRouteResult", m => m
                    .WithModifiers(SyntaxKind.ProtectedKeyword)
                    .WithGeneratedNonUserCodeAttributes()
                    .WithParameter("taskResult", "Task<IActionResult>")
                    .WithBody(b => b
                        .ReturnMethodCall(null, "RedirectToAction", "taskResult.Result")))              // return RedirectToAction(taskResult.Result);
                .WithMethod("RedirectToActionPermanent", "RedirectToRouteResult", m => m
                    .WithModifiers(SyntaxKind.ProtectedKeyword)
                    .WithGeneratedNonUserCodeAttributes()
                    .WithParameter("result", "IActionResult")
                    .WithBody(b => b
                        .VariableFromMethodCall("callInfo", "result", "GetR4MvcResult")                         // var callInfo = result.GetR4MvcResult();
                        .ReturnMethodCall(null, "RedirectToRoutePermanent", "callInfo.RouteValueDictionary")))  // return RedirectToRoutePermanent(callInfo.RouteValueDictionary);
                .WithMethod("RedirectToActionPermanent", "RedirectToRouteResult", m => m
                    .WithModifiers(SyntaxKind.ProtectedKeyword)
                    .WithGeneratedNonUserCodeAttributes()
                    .WithParameter("taskResult", "Task<IActionResult>")
                    .WithBody(b => b
                        .ReturnMethodCall(null, "RedirectToActionPermanent", "taskResult.Result")));    // return RedirectToActionPermanent(taskResult.Result);
        }

        private void AddParameterlessMethods(ClassBuilder genControllerClass, ITypeSymbol mvcSymbol)
        {
            var methods = mvcSymbol.GetPublicNonGeneratedMethods()
                .GroupBy(m => m.Name)
                .Where(g => !g.Any(m => m.Parameters.Length == 0));
            foreach (var method in methods)
                genControllerClass
                    .WithMethod(method.Key, "IActionResult", m => m
                        .WithModifiers(SyntaxKind.PublicKeyword, SyntaxKind.VirtualKeyword)
                        .WithNonActionAttribute()
                        .WithGeneratedNonUserCodeAttributes()
                        .WithBody(b => b                            // return new R4Mvc_Microsoft_AspNetCore_Mvc_ActionResult(Area, Name, ActionNames.{Action}); 
                            .ReturnNewObject(Constants.ActionResultClass, "Area", "Name", "ActionNames." + method.Key)));
        }

        private void AddMethodOverrides(ClassBuilder classBuilder, ITypeSymbol mvcSymbol)
        {
            const string overrideMethodSuffix = "Override";
            foreach (var method in mvcSymbol.GetPublicNonGeneratedMethods())
            {
                var callInfoType = Constants.ActionResultClass;
                if (method.ReturnType.InheritsFrom<JsonResult>())
                    callInfoType = Constants.JsonResultClass;
                else if (method.ReturnType.InheritsFrom<ContentResult>())
                    callInfoType = Constants.ContentResultClass;
                else if (method.ReturnType.InheritsFrom<RedirectResult>())
                    callInfoType = Constants.RedirectResultClass;
                else if (method.ReturnType.InheritsFrom<RedirectToActionResult>())
                    callInfoType = Constants.RedirectToActionResultClass;
                else if (method.ReturnType.InheritsFrom<RedirectToRouteResult>())
                    callInfoType = Constants.RedirectToRouteResultClass;


                var bodyBuilder = new BodyBuilder()
                    // var callInfo = new R4Mvc_Microsoft_AspNetCore_Mvc_ActionResult(Area, Name, ActionNames.{Action});
                    .VariableFromNewObject("callInfo", callInfoType, "Area", "Name", "ActionNames." + method.Name)
                    .ForMany(method.Parameters, (b, p) => b
                        // ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "paramName", paramName);
                        .MethodCall("ModelUnbinderHelpers", "AddRouteValues", "callInfo.RouteValueDictionary", ParameterSource.Instance.String(p.Name), p.Name))
                    // {Action}Override(callInfo, {parameters});
                    .MethodCall(null, method.Name + overrideMethodSuffix, new[] { "callInfo" }.Concat(method.Parameters.Select(p => p.Name)).ToArray());

                var returnType = method.ReturnType as INamedTypeSymbol;
                if (returnType.InheritsFrom<Task>() == true)
                {
                    var result = returnType.TypeArguments.Length > 0
                        ? "callInfo as " + returnType.TypeArguments[0]
                        : "callInfo";
                    // return Task.FromResult(callInfo as TResult);
                    bodyBuilder.ReturnMethodCall(typeof(Task).FullName, "FromResult", result);
                }
                else
                {
                    // return callInfo;
                    bodyBuilder.ReturnVariable("callInfo");
                }

                classBuilder
                    .WithMethod(method.Name + overrideMethodSuffix, null, m => m
                        .WithModifiers(SyntaxKind.PartialKeyword)
                        .WithNonActionAttribute()
                        .WithParameter("callInfo", callInfoType)
                        .ForMany(method.Parameters, (m2, p) => m2
                            .WithParameter(p.Name, p.Type.ToString()))
                        .WithNoBody())
                    .WithMethod(method.Name, method.ReturnType.ToString(), m => m
                        .WithModifiers(SyntaxKind.PublicKeyword, SyntaxKind.OverrideKeyword)
                        .WithNonActionAttribute()
                        .ForMany(method.Parameters, (m2, p) => m2
                            .WithParameter(p.Name, p.Type.ToString()))
                        .WithBody(bodyBuilder));
            }
        }

        internal static string GetR4MVCControllerClassName(INamedTypeSymbol controllerClass)
        {
            return string.Format("R4MVC_{0}", controllerClass.Name);
        }
    }
}
