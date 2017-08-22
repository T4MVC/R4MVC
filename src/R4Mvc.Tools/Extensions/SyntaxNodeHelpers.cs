using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
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
            while (true)
            {
                if (symbol.TypeKind == TypeKind.Class && symbol.ToString() == typeof(T).FullName)
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

        public static TypeSyntax PredefinedStringType()
            => PredefinedType(Token(SyntaxKind.StringKeyword));

        public static SyntaxToken[] CreateModifiers(params SyntaxKind[] kinds)
        {
            return kinds.Select(m => Token(TriviaList(), m, TriviaList(Space))).ToArray();
        }

        public static bool IsNotR4MVCGenerated(this ISymbol method)
        {
            return !method.GetAttributes().Any(a => a.AttributeClass.ToDisplayString() == typeof(GeneratedCodeAttribute).FullName);
        }

        public static IEnumerable<IMethodSymbol> GetPublicNonGeneratedMethods(this ITypeSymbol controller)
        {
            return controller.GetMembers()
                .OfType<IMethodSymbol>()
                .Where(m => m.DeclaredAccessibility == Accessibility.Public && m.MethodKind == MethodKind.Ordinary)
                .Where(IsNotR4MVCGenerated);
        }

        public static FieldDeclarationSyntax CreateFieldWithDefaultInitializer(string fieldName, string typeName, params SyntaxKind[] modifiers)
            => CreateFieldWithDefaultInitializer(fieldName, typeName, typeName, modifiers);

        public static FieldDeclarationSyntax CreateFieldWithDefaultInitializer(string fieldName, string typeName, string valueTypeName, params SyntaxKind[] modifiers)
        {
            return FieldDeclaration(
                VariableDeclaration(
                    IdentifierName(typeName),
                    fieldName,
                    ObjectCreationExpression(IdentifierName(valueTypeName))
                        .WithArgumentList(ArgumentList())))
                .WithModifiers(modifiers);
        }

        public static FieldDeclarationSyntax CreateStringFieldDeclaration(string fieldName, string fieldValue, params SyntaxKind[] modifiers)
        {
            return FieldDeclaration(
                VariableDeclaration(
                    PredefinedStringType(),
                    fieldName,
                    LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(fieldValue))))
                .WithModifiers(modifiers);
        }

        public static PropertyDeclarationSyntax CreateProperty(string propertyName, string typeName, ExpressionSyntax value, params SyntaxKind[] modifiers)
        {
            return PropertyDeclaration(IdentifierName(typeName), Identifier(propertyName))
                .WithExpressionBody(ArrowExpressionClause(value))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                .WithModifiers(modifiers);
        }

        public static PropertyDeclarationSyntax CreateAutoProperty(string propertyName, TypeSyntax type, params SyntaxKind[] modifiers)
        {
            return PropertyDeclaration(type, propertyName)
                .WithAccessorList(
                    AccessorList(
                        List<AccessorDeclarationSyntax>(
                            new[]
                            {
                                AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                                AccessorDeclaration(SyntaxKind.SetAccessorDeclaration).WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                            })))
                .WithModifiers(modifiers);
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

        private static AttributeListSyntax GeneratedNonUserCodeAttributeList()
            => AttributeList(SeparatedList(new[] { CreateGeneratedCodeAttribute(), Attribute(IdentifierName("DebuggerNonUserCode")) }));

        public static MethodDeclarationSyntax WithGeneratedNonUserCodeAttributes(this MethodDeclarationSyntax node)
            => node.AddAttributeLists(GeneratedNonUserCodeAttributeList());

        public static MethodDeclarationSyntax WithNonActionAttribute(this MethodDeclarationSyntax node)
            => node.AddAttributeLists(AttributeList(SingletonSeparatedList(Attribute(IdentifierName("NonAction")))));

        public static ConstructorDeclarationSyntax WithGeneratedNonUserCodeAttributes(this ConstructorDeclarationSyntax node)
            => node.AddAttributeLists(GeneratedNonUserCodeAttributeList());

        public static ClassDeclarationSyntax WithGeneratedNonUserCodeAttributes(this ClassDeclarationSyntax node)
            => node.AddAttributeLists(GeneratedNonUserCodeAttributeList());

        public static FieldDeclarationSyntax WithGeneratedAttribute(this FieldDeclarationSyntax node)
            => node.AddAttributeLists(AttributeList(SingletonSeparatedList(CreateGeneratedCodeAttribute())));

        public static PropertyDeclarationSyntax WithGeneratedAttribute(this PropertyDeclarationSyntax node)
            => node.AddAttributeLists(AttributeList(SingletonSeparatedList(CreateGeneratedCodeAttribute())));

        public static ClassDeclarationSyntax WithBaseTypes(this ClassDeclarationSyntax node, params string[] types)
        {
            return node.AddBaseListTypes(types.Select(x => SimpleBaseType(ParseTypeName(x))).Cast<BaseTypeSyntax>().ToArray());
        }

        public static string ToQualifiedName(this ITypeSymbol symbol)
        {
            return string.Format("{0}.{1}", symbol.ContainingNamespace.ToString(), symbol.Name);
        }

        public static T WithPragmaCodes<T>(this T node, bool enable, params string[] codes) where T : SyntaxNode
        {
            // BUG pragma is not put on newline with normalizewhitespace as expected
            var trivia = enable ? node.GetTrailingTrivia() : node.GetLeadingTrivia();
            var pramaStatus = enable ? SyntaxKind.RestoreKeyword : SyntaxKind.DisableKeyword;
            var pramaExpressions = SeparatedList(codes.Select(x => ParseExpression(x.ToString())));
            var prama =
                trivia.Add(ElasticCarriageReturnLineFeed)
                    .Add(
                        Trivia(
                            PragmaWarningDirectiveTrivia(Token(pramaStatus), pramaExpressions, true)
                                .NormalizeWhitespace()))
                    .Add(ElasticCarriageReturnLineFeed);

            return enable ? node.WithTrailingTrivia(prama) : node.WithLeadingTrivia(prama);
        }

        public static CompilationUnitSyntax WithUsings(this CompilationUnitSyntax node, params string[] namespaces)
        {
            var collection = namespaces.Select(ns => UsingDirective(ParseName(ns)));
            var usings = List(collection);
            return node.WithUsings(usings);
        }

        public static T WithHeader<T>(this T node, string headerText) where T : SyntaxNode
        {
            var leadingTrivia = node.GetLeadingTrivia()
                .Add(Comment(headerText))
                .Add(CarriageReturnLineFeed);
            return node.WithLeadingTrivia(leadingTrivia);
        }

        public static ClassDeclarationSyntax WithDefaultConstructor(this ClassDeclarationSyntax node, bool includeGeneratedAttributes = true, params SyntaxKind[] modifiers)
        {
            var ctorNode = ConstructorDeclaration(node.Identifier.ToString())
                .WithBody(Block())
                .WithModifiers(modifiers);
            if (includeGeneratedAttributes)
            {
                ctorNode = ctorNode.WithGeneratedNonUserCodeAttributes();
            }
            return node.AddMembers(ctorNode);
        }

        public static ClassDeclarationSyntax WithDefaultDummyBaseConstructor(this ClassDeclarationSyntax node, bool includeGeneratedAttributes = true, params SyntaxKind[] modifiers)
        {
            var ctorNode = ConstructorDeclaration(node.Identifier.ToString())
                .WithBody(Block())
                .WithModifiers(modifiers)
                .WithInitializer(ConstructorInitializer(SyntaxKind.BaseConstructorInitializer, ArgumentList(SeparatedList(new[] { Argument(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(Constants.DummyClass), IdentifierName(Constants.DummyClassInstance))) }))));
            if (includeGeneratedAttributes)
            {
                ctorNode = ctorNode.WithGeneratedNonUserCodeAttributes();
            }
            return node.AddMembers(ctorNode);
        }

        public static ClassDeclarationSyntax WithDummyConstructor(this ClassDeclarationSyntax node, bool includeGeneratedAttributes = true, params SyntaxKind[] modifiers)
        {
            var ctorNode = ConstructorDeclaration(node.Identifier.ToString())
                .WithBody(Block())
                .WithModifiers(modifiers)
                .AddParameterListParameters(Parameter(Identifier("d")).WithType(ParseTypeName(Constants.DummyClass)));
            if (includeGeneratedAttributes)
            {
                ctorNode = ctorNode.WithGeneratedNonUserCodeAttributes();
            }
            return node.AddMembers(ctorNode);
        }

        public static ClassDeclarationSyntax WithSubClassMembersAsStrings(this ClassDeclarationSyntax node, ITypeSymbol controllerClass, string className, params SyntaxKind[] modifiers)
        {
            // create ActionConstants sub class
            var actionNameClass = ClassDeclaration(className)
                .WithModifiers(SyntaxKind.PublicKeyword)
                .WithGeneratedNonUserCodeAttributes();
            foreach (var actionName in controllerClass.GetPublicNonGeneratedMethods().GroupBy(x => x.Name))
            {
                actionNameClass = actionNameClass.WithStringField(actionName.Key, actionName.Key, false, modifiers);
            }
            return node.AddMembers(actionNameClass);
        }

        public static ClassDeclarationSyntax WithStaticFieldBackedProperty(this ClassDeclarationSyntax node, string propertyName, string typeName, params SyntaxKind[] modifiers)
        {
            var fieldName = "s_" + propertyName;
            return node.AddMembers(
                CreateFieldWithDefaultInitializer(fieldName, typeName, SyntaxKind.StaticKeyword, SyntaxKind.ReadOnlyKeyword)
                    .WithGeneratedAttribute(),
                CreateProperty(propertyName, typeName, IdentifierName(fieldName), modifiers)
                    .WithGeneratedAttribute());
        }

        public static ClassDeclarationSyntax WithField(this ClassDeclarationSyntax node, string fieldName, string typeName, params SyntaxKind[] modifiers)
        {
            var field = CreateFieldWithDefaultInitializer(fieldName, typeName, modifiers)
                .WithGeneratedAttribute();
            return node.AddMembers(field);
        }

        public static ClassDeclarationSyntax WithProperty(this ClassDeclarationSyntax node, string propertyName, string typeName, ExpressionSyntax value, params SyntaxKind[] modifiers)
        {
            var property = CreateProperty(propertyName, typeName, value, modifiers)
                .WithGeneratedAttribute();
            return node.AddMembers(property);
        }

        public static ClassDeclarationSyntax WithAutoStringProperty(this ClassDeclarationSyntax node, string propertyName, params SyntaxKind[] modifiers)
            => WithAutoProperty(node, propertyName, PredefinedStringType(), modifiers);

        public static ClassDeclarationSyntax WithAutoProperty(this ClassDeclarationSyntax node, string propertyName, TypeSyntax type, params SyntaxKind[] modifiers)
        {
            var property = CreateAutoProperty(propertyName, type, modifiers);
            return node.AddMembers(property);
        }

        public static ClassDeclarationSyntax WithStringField(this ClassDeclarationSyntax node, string name, string value, bool includeGeneratedAttribute = true, params SyntaxKind[] modifiers)
        {
            var fieldDeclaration = CreateStringFieldDeclaration(name, value, modifiers);
            if (includeGeneratedAttribute) fieldDeclaration = fieldDeclaration.WithGeneratedAttribute();
            return node.AddMembers(fieldDeclaration);
        }

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
            return node.AddModifiers(CreateModifiers(modifiers));
        }

        public static VariableDeclarationSyntax VariableDeclaration(string name, ExpressionSyntax value)
            => VariableDeclaration(IdentifierName("var"), name, value);

        public static VariableDeclarationSyntax VariableDeclaration(TypeSyntax type, string name, ExpressionSyntax value)
        {
            return SyntaxFactory.VariableDeclaration(type)
                .WithVariables(
                SingletonSeparatedList(
                    VariableDeclarator(Identifier(name))
                        .WithInitializer(
                            EqualsValueClause(value))));
        }

        public static ParameterSyntax WithGenericType(this ParameterSyntax node, string genericName, params string[] typeArguments)
        {
            return node.WithType(
                GenericName(Identifier(genericName))
                    .WithTypeArgumentList(
                        TypeArgumentList(
                            SeparatedList<TypeSyntax>(
                                typeArguments.Select(t => IdentifierName(t))))));
        }

        public static MemberAccessExpressionSyntax MemberAccess(string entityName, string memberName)
        {
            return MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                IdentifierName(entityName),
                IdentifierName(memberName));
        }

        public static InvocationExpressionSyntax WithArgumentList(this InvocationExpressionSyntax node, params ExpressionSyntax[] arguments)
        {
            return node.WithArgumentList(
                ArgumentList(
                    SeparatedList(arguments.Select(e => Argument(e)))));
        }

        public static ObjectCreationExpressionSyntax WithArgumentList(this ObjectCreationExpressionSyntax node, params ExpressionSyntax[] arguments)
        {
            return node.WithArgumentList(
                ArgumentList(
                    SeparatedList(arguments.Select(e => Argument(e)))));
        }
    }
}
