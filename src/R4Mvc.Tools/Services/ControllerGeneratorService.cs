using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Options;
using R4Mvc.Tools.Extensions;
using System.Collections.Generic;
using System.Linq;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static R4Mvc.Tools.Extensions.SyntaxNodeHelpers;

namespace R4Mvc.Tools.Services
{
    public class ControllerGeneratorService : IControllerGeneratorService
    {
        private readonly IViewLocatorService _viewLocator;
        private readonly Settings _settings;

        public ControllerGeneratorService(IViewLocatorService viewLocator, IOptions<Settings> settings)
        {
            _viewLocator = viewLocator;
            _settings = settings.Value;
        }

        public string GetControllerArea(INamedTypeSymbol controllerSymbol)
        {
            var areaAttribute = SearchAreaAttribute(controllerSymbol);

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


        AttributeData SearchAreaAttribute(INamedTypeSymbol controllerSymbol)
        {
            var current = controllerSymbol;
            AttributeData result = null;

            while (current != null && result == null)
            {
                result = current.GetAttributes().Where(a => a.AttributeClass.InheritsFrom<AreaAttribute>()).FirstOrDefault();
                current = current.BaseType;
            }

            return result;
        }


        public ClassDeclarationSyntax GeneratePartialController(INamedTypeSymbol controllerSymbol, string areaKey, string areaName, string controllerName, string projectRoot)
        {
            // build controller partial class node 
            // add a default constructor if there are some but none are zero length
            var genControllerClass = ClassDeclaration(controllerSymbol.Name)
                .WithModifiers(SyntaxKind.PublicKeyword, SyntaxKind.PartialKeyword);
            var controllerTypeParams = controllerSymbol.TypeParameters.Select(tp => TypeParameter(tp.Name)).ToArray();
            if (controllerTypeParams.Length > 0)
                genControllerClass = genControllerClass.AddTypeParameterListParameters(controllerTypeParams);

            var gotCustomConstructors = controllerSymbol.Constructors
                .Where(c => c.DeclaredAccessibility == Accessibility.Public)
                .Where(SyntaxNodeHelpers.IsNotR4MVCGenerated)
                .Where(c => !c.IsImplicitlyDeclared)
                .Any();
            if (!gotCustomConstructors)
            {
                genControllerClass = genControllerClass.WithDefaultConstructor(true, SyntaxKind.PublicKeyword);
            }
            genControllerClass = genControllerClass.WithDummyConstructor(true, SyntaxKind.ProtectedKeyword);
            genControllerClass = AddRedirectMethods(genControllerClass);

            // add all method stubs, TODO criteria for this: only public virtual actionresults?
            // add subclasses, fields, properties, constants for action names
            genControllerClass = AddParameterlessMethods(genControllerClass, controllerSymbol);
            var actionsExpression = !string.IsNullOrEmpty(areaKey)
                ? SyntaxNodeHelpers.MemberAccess(_settings.HelpersPrefix + "." + areaKey, controllerName)
                : SyntaxNodeHelpers.MemberAccess(_settings.HelpersPrefix, controllerName);
            genControllerClass =
                genControllerClass
                    .WithProperty("Actions", controllerSymbol.Name, actionsExpression, SyntaxKind.PublicKeyword)
                    .WithStringField(
                        "Area",
                        areaName,
                        true,
                        SyntaxKind.PublicKeyword,
                        SyntaxKind.ReadOnlyKeyword)
                    .WithStringField(
                        "Name",
                        controllerName,
                        true,
                        SyntaxKind.PublicKeyword,
                        SyntaxKind.ReadOnlyKeyword)
                    .WithStringField(
                        "NameConst",
                        controllerName,
                        true,
                        SyntaxKind.PublicKeyword,
                        SyntaxKind.ConstKeyword)
                    .WithStaticFieldBackedProperty("ActionNames", "ActionNamesClass", SyntaxKind.PublicKeyword)
                    .WithActionNameClass(controllerSymbol)
                    .WithActionConstantsClass(controllerSymbol)
                    .WithViewsClass(controllerName, areaName, _viewLocator.FindViews(projectRoot));

            return genControllerClass;
        }

        public ClassDeclarationSyntax GenerateR4Controller(INamedTypeSymbol controllerSymbol)
        {
            // create R4MVC_[Controller] class inheriting from partial
            var r4ControllerClass = ClassDeclaration(GetR4MVCControllerClassName(controllerSymbol))
                .WithModifiers(SyntaxKind.PublicKeyword, SyntaxKind.PartialKeyword)
                .WithGeneratedNonUserCodeAttributes()
                .WithBaseTypes(controllerSymbol.ToQualifiedName())
                .WithDefaultDummyBaseConstructor(false, SyntaxKind.PublicKeyword);
            r4ControllerClass = AddMethodOverrides(r4ControllerClass, controllerSymbol);
            return r4ControllerClass;
        }

        private ClassDeclarationSyntax AddRedirectMethods(ClassDeclarationSyntax node)
        {
            var methods = new[]
            {
                MethodDeclaration(IdentifierName("RedirectToRouteResult"), Identifier("RedirectToAction"))
                    .WithModifiers(SyntaxKind.ProtectedKeyword)
                    .WithGeneratedNonUserCodeAttributes()
                    .AddParameterListParameters(
                        Parameter(Identifier("result")).WithType(IdentifierName("IActionResult")))
                    .WithBody(
                        Block(
                            // var callInfo = result.GetR4MvcResult();
                            LocalDeclarationStatement(
                                SyntaxNodeHelpers.VariableDeclaration("callInfo",
                                    InvocationExpression(SyntaxNodeHelpers.MemberAccess("result", "GetR4MvcResult")))),
                            // return RedirectToRoute(callInfo.RouteValueDictionary);
                            ReturnStatement(
                                InvocationExpression(IdentifierName("RedirectToRoute"))
                                    .WithArgumentList(
                                        SyntaxNodeHelpers.MemberAccess("callInfo", "RouteValueDictionary"))))),
                MethodDeclaration(IdentifierName("RedirectToRouteResult"), Identifier("RedirectToAction"))
                    .WithModifiers(SyntaxKind.ProtectedKeyword)
                    .WithGeneratedNonUserCodeAttributes()
                    .AddParameterListParameters(
                        Parameter(Identifier("taskResult")).WithGenericType("Task", "IActionResult"))
                    .WithBody(
                        Block(
                            // return RedirectToAction(taskResult.Result);
                            ReturnStatement(
                                InvocationExpression(IdentifierName("RedirectToAction"))
                                    .WithArgumentList(
                                        SyntaxNodeHelpers.MemberAccess("taskResult", "Result"))))),
                MethodDeclaration(IdentifierName("RedirectToRouteResult"), Identifier("RedirectToActionPermanent"))
                    .WithModifiers(SyntaxKind.ProtectedKeyword)
                    .WithGeneratedNonUserCodeAttributes()
                    .AddParameterListParameters(
                        Parameter(Identifier("result")).WithType(IdentifierName("IActionResult")))
                    .WithBody(
                        Block(
                            // var callInfo = result.GetR4MvcResult();
                            LocalDeclarationStatement(
                                SyntaxNodeHelpers.VariableDeclaration("callInfo",
                                    InvocationExpression(SyntaxNodeHelpers.MemberAccess("result", "GetR4MvcResult")))),
                            // return RedirectToRoutePermanent(callInfo.RouteValueDictionary);
                            ReturnStatement(
                                InvocationExpression(IdentifierName("RedirectToRoutePermanent"))
                                    .WithArgumentList(
                                        SyntaxNodeHelpers.MemberAccess("callInfo", "RouteValueDictionary"))))),
                MethodDeclaration(IdentifierName("RedirectToRouteResult"), Identifier("RedirectToActionPermanent"))
                    .WithModifiers(SyntaxKind.ProtectedKeyword)
                    .WithGeneratedNonUserCodeAttributes()
                    .AddParameterListParameters(
                        Parameter(Identifier("taskResult")).WithGenericType("Task", "IActionResult"))
                    .WithBody(
                        Block(
                            // return RedirectToActionPermanent(taskResult.Result);
                            ReturnStatement(
                                InvocationExpression(IdentifierName("RedirectToActionPermanent"))
                                    .WithArgumentList(
                                        SyntaxNodeHelpers.MemberAccess("taskResult", "Result"))))),
            };
            return node.AddMembers(methods);
        }

        private ClassDeclarationSyntax AddParameterlessMethods(ClassDeclarationSyntax node, ITypeSymbol mvcSymbol)
        {
            var methods = mvcSymbol.GetPublicNonGeneratedMethods()
                .GroupBy(m => m.Name)
                .Where(g => !g.Any(m => m.Parameters.Length == 0))
                .Select(g => MethodDeclaration(IdentifierName("IActionResult"), Identifier(g.Key))
                    .WithModifiers(SyntaxKind.PublicKeyword, SyntaxKind.VirtualKeyword)
                    .WithNonActionAttribute()
                    .WithGeneratedNonUserCodeAttributes()
                    .WithBody(
                        Block(
                            // return new R4Mvc_Microsoft_AspNetCore_Mvc_ActionResult(Area, Name, ActionNames.{Action});
                            ReturnStatement(
                                ObjectCreationExpression(IdentifierName(Constants.ActionResultClass))
                                    .WithArgumentList(
                                        IdentifierName("Area"),
                                        IdentifierName("Name"),
                                        SyntaxNodeHelpers.MemberAccess("ActionNames", g.Key))))));
            return node.AddMembers(methods.ToArray());
        }

        private ClassDeclarationSyntax AddMethodOverrides(ClassDeclarationSyntax node, ITypeSymbol mvcSymbol)
        {
            const string overrideMethodSuffix = "Override";
            var methods = mvcSymbol.GetPublicNonGeneratedMethods()
                .SelectMany(m =>
                {
                    var statements = new List<StatementSyntax>
                    {
                        // var callInfo = new R4Mvc_Microsoft_AspNetCore_Mvc_ActionResult(Area, Name, ActionNames.{Action});
                        LocalDeclarationStatement(
                            SyntaxNodeHelpers.VariableDeclaration("callInfo",
                                ObjectCreationExpression(IdentifierName(Constants.ActionResultClass))
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
                    statements.Add(
                        // return callInfo;
                        m.ReturnType.ToString().Contains("Task<")
                            ? ReturnStatement(
                                InvocationExpression(
                                    SyntaxNodeHelpers.MemberAccess("Task", "FromResult"))
                                    .WithArgumentList(
                                        BinaryExpression(
                                            SyntaxKind.AsExpression,
                                            IdentifierName("callInfo"),
                                            IdentifierName(m.ReturnType.ToString().Substring(m.ReturnType.ToString().IndexOf('<') + 1).TrimEnd('>')))))
                            : ReturnStatement(IdentifierName("callInfo")));
                    return new[]
                    {
                        MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)), Identifier(m.Name + overrideMethodSuffix))
                            .WithModifiers(SyntaxKind.PartialKeyword)
                            .WithNonActionAttribute()
                            .AddParameterListParameters(
                                Parameter(Identifier("callInfo")).WithType(IdentifierName(Constants.ActionResultClass)))
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
