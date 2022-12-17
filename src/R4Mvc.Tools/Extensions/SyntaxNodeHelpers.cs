using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace R4Mvc.Tools.Extensions
{
    /// <summary>
    /// A collection of helper and fluent extension methods to help manipulate SyntaxNodes
    /// </summary>
    public static class SyntaxNodeHelpers
    {
        public static bool InheritsFrom<T>(this ITypeSymbol symbol)
        {
            var matchingTypeName = typeof(T).FullName;
            if (typeof(T).IsInterface)
            {
                return symbol.ToString() == matchingTypeName || symbol.AllInterfaces.Any(i => i.ToString() == matchingTypeName);
            }
            while (true)
            {
                if (symbol.TypeKind == TypeKind.Class && symbol.ToString() == matchingTypeName)
                {
                    return true;
                }
                if (symbol.BaseType != null)
                {
                    symbol = symbol.BaseType;
                    continue;
                }
                break;
            }
            return false;
        }

        public static SyntaxToken[] CreateModifiers(params SyntaxKind[] kinds)
        {
            return kinds.Select(m => Token(TriviaList(), m, TriviaList(Space))).ToArray();
        }

        public static bool IsNotR4MVCGenerated(this ISymbol method)
        {
            return !method.GetAttributes().Any(a => a.AttributeClass.InheritsFrom<GeneratedCodeAttribute>());
        }

        public static bool IsNotR4MvcExcluded(this ISymbol method)
        {
            return !method.GetAttributes().Any(a => a.AttributeClass.InheritsFrom<R4MvcExcludeAttribute>() || a.AttributeClass.Name == "R4MvcExclude");
        }

        private static string[] _controllerClassMethodNames = null;
        private static string[] _pageClassMethodNames = null;
        public static void PopulateControllerClassMethodNames(CSharpCompilation compilation)
        {
            var result = new List<string>();
            var typeSymbol = compilation.GetTypeByMetadataName("Microsoft.AspNetCore.Mvc.Controller");
            while (typeSymbol != null)
            {
                var methodNames = typeSymbol.GetMembers()
                    .Where(r => r.Kind == SymbolKind.Method && r.DeclaredAccessibility == Accessibility.Public && r.IsVirtual)
                    .Select(s => s.Name);
                result.AddRange(methodNames);
                typeSymbol = typeSymbol.BaseType;
            }
            _controllerClassMethodNames = result.Distinct().ToArray();

            result = new List<string>();
            typeSymbol = compilation.GetTypeByMetadataName("Microsoft.AspNetCore.Mvc.RazorPages.PageModel");
            while (typeSymbol != null)
            {
                var methodNames = typeSymbol.GetMembers()
                    .Where(r => r.Kind == SymbolKind.Method && r.DeclaredAccessibility == Accessibility.Public && r.IsVirtual)
                    .Select(s => s.Name);
                result.AddRange(methodNames);
                typeSymbol = typeSymbol.BaseType;
            }
            _pageClassMethodNames = result.Distinct().ToArray();
        }

        public static bool IsMvcAction(this IMethodSymbol method)
        {
            if (method.GetAttributes().Any(a => a.AttributeClass.InheritsFrom<NonActionAttribute>()))
                return false;
            if (_controllerClassMethodNames.Contains(method.Name))
                return false;
            return true;
        }

        public static bool IsRazorPageAction(this IMethodSymbol method)
        {
            if (method.GetAttributes().Any(a => a.AttributeClass.InheritsFrom<NonActionAttribute>()))
                return false;
            if (_pageClassMethodNames.Contains(method.Name))
                return false;
            if (!method.Name.StartsWith("On"))
                return false;
            return true;
        }

        public static IEnumerable<IMethodSymbol> GetPublicNonGeneratedControllerMethods(this ITypeSymbol controller)
        {
            return controller.GetMembers()
                .OfType<IMethodSymbol>()
                .Where(m => m.DeclaredAccessibility == Accessibility.Public && m.MethodKind == MethodKind.Ordinary)
                .Where(IsNotR4MVCGenerated)
                .Where(IsNotR4MvcExcluded)
                .Where(IsMvcAction);
        }

        public static IEnumerable<IMethodSymbol> GetPublicNonGeneratedPageMethods(this ITypeSymbol controller)
        {
            return controller.GetMembers()
                .OfType<IMethodSymbol>()
                .Where(m => m.DeclaredAccessibility == Accessibility.Public && m.MethodKind == MethodKind.Ordinary)
                .Where(IsNotR4MVCGenerated)
                .Where(IsNotR4MvcExcluded)
                .Where(IsRazorPageAction);
        }

        private static AttributeSyntax CreateGeneratedCodeAttribute()
        {
            var arguments =
                AttributeArgumentList(
                    SeparatedList(
                        new[]
                        {
                            AttributeArgument(
                                LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(Constants.ProjectName))),
                            AttributeArgument(
                                LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(Constants.Version)))
                        }));
            return Attribute(IdentifierName("GeneratedCode"), arguments);
        }

        public static AttributeListSyntax GeneratedNonUserCodeAttributeList()
            => AttributeList(SeparatedList(new[] { CreateGeneratedCodeAttribute(), Attribute(IdentifierName("DebuggerNonUserCode")) }));

        public static MethodDeclarationSyntax WithNonActionAttribute(this MethodDeclarationSyntax node)
            => node.AddAttributeLists(AttributeList(SingletonSeparatedList(Attribute(IdentifierName("NonAction")))));

        public static MethodDeclarationSyntax WithNonHandlerAttribute(this MethodDeclarationSyntax node)
            => node.AddAttributeLists(AttributeList(SingletonSeparatedList(Attribute(IdentifierName("NonHandler")))));

        public static FieldDeclarationSyntax WithGeneratedAttribute(this FieldDeclarationSyntax node)
            => node.AddAttributeLists(AttributeList(SingletonSeparatedList(CreateGeneratedCodeAttribute())));

        public static PropertyDeclarationSyntax WithGeneratedNonUserCodeAttribute(this PropertyDeclarationSyntax node)
            => node.AddAttributeLists(AttributeList(SeparatedList(new[] { CreateGeneratedCodeAttribute(), Attribute(IdentifierName("DebuggerNonUserCode")) })));

        /// TODO: Can this use a aeparated list?
        public static ClassDeclarationSyntax WithModifiers(this ClassDeclarationSyntax node, params SyntaxKind[] modifiers)
        {
            return node.AddModifiers(CreateModifiers(modifiers));
        }

        public static ConstructorDeclarationSyntax WithModifiers(this ConstructorDeclarationSyntax node, params SyntaxKind[] modifiers)
        {
            return node.AddModifiers(CreateModifiers(modifiers));
        }

        public static MethodDeclarationSyntax WithModifiers(this MethodDeclarationSyntax node, params SyntaxKind[] modifiers)
        {
            return node.AddModifiers(CreateModifiers(modifiers));
        }

        public static FieldDeclarationSyntax WithModifiers(this FieldDeclarationSyntax node, params SyntaxKind[] modifiers)
        {
            return node.AddModifiers(CreateModifiers(modifiers));
        }

        public static PropertyDeclarationSyntax WithModifiers(this PropertyDeclarationSyntax node, params SyntaxKind[] modifiers)
        {
            if (modifiers.Length == 0)
                return node;
            return node.AddModifiers(CreateModifiers(modifiers));
        }

        public static MemberAccessExpressionSyntax MemberAccess(string entityName, string memberName)
        {
            return MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                IdentifierName(entityName),
                IdentifierName(memberName));
        }

        public static string GetRouteName(this IParameterSymbol property)
        {
            return property.GetAttributes()
                .Where(attr => attr.AttributeClass.InheritsFrom<BindAttribute>())
                .SelectMany(attr => attr.NamedArguments.Where(arg => arg.Key == nameof(BindAttribute.Prefix)))
                .Select(arg => arg.Value.Value as string)
                .Where(prefix => !string.IsNullOrEmpty(prefix))
                .DefaultIfEmpty(property.Name)
                .First();
        }

        // https://andrewlock.net/creating-a-source-generator-part-5-finding-a-type-declarations-namespace-and-type-hierarchy/
        // determine the namespace the class/enum/struct is declared in, if any
        public static string GetNamespace(this BaseTypeDeclarationSyntax syntax)
        {
            // If we don't have a namespace at all we'll return an empty string
            // This accounts for the "default namespace" case
            string nameSpace = string.Empty;

            // Get the containing syntax node for the type declaration (could be a nested type, for example)
            SyntaxNode potentialNamespaceParent = syntax.Parent;

            // Keep moving "out" of nested classes etc until we get to a namespace or until we run out of parents
            while (potentialNamespaceParent is not (NamespaceDeclarationSyntax or FileScopedNamespaceDeclarationSyntax or null))
            {
                potentialNamespaceParent = potentialNamespaceParent.Parent;
            }

            // Build up the final namespace by looping until we no longer have a namespace declaration
            if (potentialNamespaceParent is BaseNamespaceDeclarationSyntax namespaceParent)
            {
                // We have a namespace. Use that as the type
                nameSpace = namespaceParent.Name.ToString();

                // Keep moving "out" of the namespace declarations until we run out of nested namespace declarations
                while (true)
                {
                    if (namespaceParent.Parent is not NamespaceDeclarationSyntax parent)
                    {
                        break;
                    }

                    // Add the outer namespace as a prefix to the final namespace
                    nameSpace = $"{namespaceParent.Name}.{nameSpace}";
                    namespaceParent = parent;
                }
            }

            // return the final namespace
            return nameSpace;
        }
    }
}
