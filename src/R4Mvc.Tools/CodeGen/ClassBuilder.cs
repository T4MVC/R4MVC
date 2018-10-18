using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Routing;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using R4Mvc.Tools.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace R4Mvc.Tools.CodeGen
{
    public class ClassBuilder
    {
        private ClassDeclarationSyntax _class;

        public ClassBuilder(string className)
        {
            Name = className;
            _class = ClassDeclaration(className);
        }

        public string Name { get; }
        public bool IsGenerated { get; private set; }

        public ClassBuilder WithComment(string comment)
        {
            var trivia = _class.GetLeadingTrivia()
                .Add(Comment(comment));
            _class = _class.WithLeadingTrivia(trivia);
            return this;
        }

        public ClassBuilder WithModifiers(params SyntaxKind[] modifiers)
        {
            _class = _class.WithModifiers(modifiers);
            return this;
        }

        public ClassBuilder WithBaseTypes(params string[] classNames)
        {
            if (classNames.Length > 0)
                _class = _class.AddBaseListTypes(classNames.Select(c => SimpleBaseType(ParseTypeName(c))).ToArray<BaseTypeSyntax>());
            return this;
        }

        public ClassBuilder WithTypeParameters(params string[] typeParams)
        {
            if (typeParams.Length > 0)
                _class = _class.AddTypeParameterListParameters(typeParams.Select(tp => TypeParameter(tp)).ToArray());
            return this;
        }

        public ClassBuilder WithMember(MemberDeclarationSyntax method)
        {
            _class = _class.AddMembers(method);
            return this;
        }

        public ClassBuilder WithMethod(string name, string returnType, Action<MethodBuilder> methodParts)
        {
            var method = new MethodBuilder(name, returnType);
            methodParts(method);
            WithMember(method.Build());
            return this;
        }

        public ClassBuilder WithConstructor(Action<ConstructorMethodBuilder> constructorParts)
        {
            var constructor = new ConstructorMethodBuilder(Name);
            constructorParts(constructor);
            WithMember(constructor.Build());
            return this;
        }

        public ClassBuilder WithChildClass(string className, Action<ClassBuilder> classOptions)
        {
            var classBuilder = new ClassBuilder(className)
            {
                IsGenerated = IsGenerated,
            };
            classOptions(classBuilder);
            _class = _class.AddMembers(classBuilder.Build());
            return this;
        }

        public ClassBuilder WithGeneratedNonUserCodeAttributes()
        {
            if (!IsGenerated)
            {
                IsGenerated = true;
                _class = _class.AddAttributeLists(SyntaxNodeHelpers.GeneratedNonUserCodeAttributeList());
            }
            return this;
        }

        public ClassBuilder WithProperty(string name, string type, SyntaxKind modifier = SyntaxKind.PublicKeyword)
        {
            var prop = PropertyDeclaration(IdentifierName(type), name)
                .WithAccessorList(
                    AccessorList(
                        List(
                            new[]
                            {
                                AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                                AccessorDeclaration(SyntaxKind.SetAccessorDeclaration).WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                            })));
            if (modifier != 0)
                prop = prop.WithModifiers(new[] { modifier });
            _class = _class.AddMembers(prop);
            return this;
        }

        public ClassBuilder WithExpressionProperty(string name, string type, string value, params SyntaxKind[] modifiers)
        {
            var property = PropertyDeclaration(IdentifierName(type), name)
                .WithExpressionBody(ArrowExpressionClause(value != null
                    ? IdentifierName(value) as ExpressionSyntax
                    : LiteralExpression(SyntaxKind.NullLiteralExpression)))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                .WithModifiers(modifiers);
            if (!IsGenerated)
                property = property.WithGeneratedNonUserCodeAttribute();
            _class = _class.AddMembers(property);
            return this;
        }

        public ClassBuilder WithStringField(string name, string value, params SyntaxKind[] modifiers)
        {
            var fieldDeclaration = FieldDeclaration(
                VariableDeclaration(PredefinedType(Token(SyntaxKind.StringKeyword)))
                    .WithVariables(
                        SingletonSeparatedList(
                            VariableDeclarator(Identifier(name))
                                .WithInitializer(
                                    EqualsValueClause(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(value)))))))
                .WithModifiers(modifiers);
            if (!IsGenerated)
                fieldDeclaration = fieldDeclaration.WithGeneratedAttribute();
            _class = _class.AddMembers(fieldDeclaration);
            return this;
        }

        private FieldDeclarationSyntax CreateFieldInitialised(string name, string type, ExpressionSyntax value, params SyntaxKind[] modifiers)
            => FieldDeclaration(
                VariableDeclaration(IdentifierName(type))
                    .WithVariables(
                        SingletonSeparatedList(
                            VariableDeclarator(Identifier(name))
                                .WithInitializer(
                                    EqualsValueClause(value)))))
                .WithModifiers(modifiers);

        public ClassBuilder WithField(string name, string type, string valueType, params SyntaxKind[] modifiers)
        {
            var value = ObjectCreationExpression(IdentifierName(valueType))
                .WithArgumentList(ArgumentList());
            var field = CreateFieldInitialised(name, type, value, modifiers);
            _class = _class.AddMembers(field);
            return this;
        }

        public ClassBuilder WithValueField(string name, string type, string value, params SyntaxKind[] modifiers)
        {
            var field = CreateFieldInitialised(name, type, IdentifierName(value), modifiers);
            _class = _class.AddMembers(field);
            return this;
        }

        public ClassBuilder WithRouteValueField(string name, RouteValueDictionary routeValues, params SyntaxKind[] modifiers)
        {
            var value = ObjectCreationExpression(IdentifierName("RouteValueDictionary"))
                .WithInitializer(
                    InitializerExpression(SyntaxKind.CollectionInitializerExpression,
                        SeparatedList(routeValues
                            .Select(rv => (ExpressionSyntax)InitializerExpression(SyntaxKind.ComplexElementInitializerExpression,
                                SeparatedList(new ExpressionSyntax[] {
                                    LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(rv.Key)),
                                    LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(rv.Value.ToString())) }))))));

            var field = CreateFieldInitialised(name, "RouteValueDictionary", value, modifiers);
            if (!IsGenerated)
                field = field.WithGeneratedAttribute();
            _class = _class.AddMembers(field);
            return this;

        }

        public ClassBuilder WithStaticFieldBackedProperty(string name, string type, params SyntaxKind[] modifiers)
        {
            var fieldName = "s_" + name;
            var fieldValue = ObjectCreationExpression(IdentifierName(type))
                .WithArgumentList(ArgumentList());
            var field = CreateFieldInitialised(fieldName, type, fieldValue, SyntaxKind.StaticKeyword, SyntaxKind.ReadOnlyKeyword);
            var property = PropertyDeclaration(IdentifierName(type), Identifier(name))
                .WithExpressionBody(ArrowExpressionClause(IdentifierName(fieldName)))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                .WithModifiers(modifiers);
            if (!IsGenerated)
            {
                field = field.WithGeneratedAttribute();
                property = property.WithGeneratedNonUserCodeAttribute();
            }

            _class = _class.AddMembers(field, property);
            return this;
        }

        public ClassBuilder ForEach<TEntity>(IEnumerable<TEntity> items, Action<ClassBuilder, TEntity> action)
        {
            if (items != null)
                foreach (var item in items)
                    action(this, item);
            return this;
        }

        internal ClassDeclarationSyntax Build()
        {
            return _class;
        }
    }
}
