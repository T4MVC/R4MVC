using System.Collections.Generic;
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

            var r4ControllerClass = ClassDeclaration(className)
                .WithModifiers(SyntaxKind.PublicKeyword, SyntaxKind.PartialKeyword)
                .WithGeneratedNonUserCodeAttributes()
                .WithBaseTypes(controller.Symbol.ToQualifiedName())
                .WithDefaultDummyBaseConstructor(false, SyntaxKind.PublicKeyword);
            r4ControllerClass = AddMethodOverrides(r4ControllerClass, controller.Symbol);
            return r4ControllerClass;
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
                            .ReturnNew(Constants.ActionResultClass, "Area", "Name", "ActionNames." + method.Key)));
        }

        private ClassDeclarationSyntax AddMethodOverrides(ClassDeclarationSyntax node, ITypeSymbol mvcSymbol)
        {
            const string overrideMethodSuffix = "Override";
            var methods = mvcSymbol.GetPublicNonGeneratedMethods()
                .SelectMany(m =>
                {
                    var callInfoType = Constants.ActionResultClass;
                    if (m.ReturnType.InheritsFrom<JsonResult>())
                        callInfoType = Constants.JsonResultClass;
                    else if (m.ReturnType.InheritsFrom<ContentResult>())
                        callInfoType = Constants.ContentResultClass;
                    else if (m.ReturnType.InheritsFrom<RedirectResult>())
                        callInfoType = Constants.RedirectResultClass;
                    else if (m.ReturnType.InheritsFrom<RedirectToActionResult>())
                        callInfoType = Constants.RedirectToActionResultClass;
                    else if (m.ReturnType.InheritsFrom<RedirectToRouteResult>())
                        callInfoType = Constants.RedirectToRouteResultClass;

                    var statements = new List<StatementSyntax>
                    {
                        // var callInfo = new R4Mvc_Microsoft_AspNetCore_Mvc_ActionResult(Area, Name, ActionNames.{Action});
                        LocalDeclarationStatement(
                            SyntaxNodeHelpers.VariableDeclaration("callInfo",
                                ObjectCreationExpression(IdentifierName(callInfoType))
                                    .WithArgumentList(
                                        IdentifierName("Area"),
                                        IdentifierName("Name"),
                                        SyntaxNodeHelpers.MemberAccess("ActionNames", m.Name)))),
                    };
                    foreach (var param in m.Parameters)
                        statements.Add(
                            ExpressionStatement(
                                InvocationExpression(
                                    SyntaxNodeHelpers.MemberAccess("ModelUnbinderHelpers", "AddRouteValues"))
                                    .WithArgumentList(
                                        SyntaxNodeHelpers.MemberAccess("callInfo", "RouteValueDictionary"),
                                        LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(param.Name)),
                                        IdentifierName(param.Name))));
                    statements.Add(
                        // {Action}Override(callInfo, {parameters});
                        ExpressionStatement(
                            InvocationExpression(IdentifierName(m.Name + overrideMethodSuffix))
                                .WithArgumentList(
                                    new[] { IdentifierName("callInfo") }
                                        .Concat(m.Parameters.Select(p => IdentifierName(p.Name)))
                                        .ToArray())));
                    var returnType = m.ReturnType as INamedTypeSymbol;
                    statements.Add(
                        // return callInfo;
                        returnType.InheritsFrom<Task>() == true
                            ? ReturnStatement(
                                InvocationExpression(
                                    SyntaxNodeHelpers.MemberAccess(typeof(Task).FullName, "FromResult"))
                                    .WithArgumentList(
                                        returnType.TypeArguments.Length > 0
                                            ? (ExpressionSyntax)BinaryExpression(
                                                SyntaxKind.AsExpression,
                                                IdentifierName("callInfo"),
                                                IdentifierName(returnType.TypeArguments[0].ToString()))
                                            : IdentifierName("callInfo")
                                        ))
                            : ReturnStatement(IdentifierName("callInfo")));
                    return new[]
                    {
                        MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)), Identifier(m.Name + overrideMethodSuffix))
                            .WithModifiers(SyntaxKind.PartialKeyword)
                            .WithNonActionAttribute()
                            .AddParameterListParameters(
                                Parameter(Identifier("callInfo")).WithType(IdentifierName(callInfoType)))
                            .AddParameterListParameters(m.Parameters
                                .Select(p => Parameter(Identifier(p.Name))
                                    .WithType(IdentifierName(p.Type.ToString())))
                                .ToArray())
                            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                        MethodDeclaration(IdentifierName(m.ReturnType.ToString()), Identifier(m.Name))
                            .WithModifiers(SyntaxKind.PublicKeyword, SyntaxKind.OverrideKeyword)
                            .WithNonActionAttribute()
                            .AddParameterListParameters(m.Parameters
                                .Select(p => Parameter(Identifier(p.Name))
                                    .WithType(IdentifierName(p.Type.ToString())))
                                .ToArray())
                            .WithBody(
                                Block(statements.ToArray())),
                    };
                });
            return node.AddMembers(methods.ToArray());
        }

        internal static string GetR4MVCControllerClassName(INamedTypeSymbol controllerClass)
        {
            return string.Format("R4MVC_{0}", controllerClass.Name);
        }
    }
}
