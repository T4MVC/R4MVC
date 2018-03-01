using System;
using System.Collections.Generic;
using System.Linq;
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
            var classBuilder = new ClassBuilder(className);
            classOptions(classBuilder);
            _class = _class.AddMembers(classBuilder.Build());
            return this;
        }

        public ClassBuilder WithGeneratedNonUserCodeAttributes()
        {
            _class = _class.AddAttributeLists(SyntaxNodeHelpers.GeneratedNonUserCodeAttributeList());
            return this;
        }

        public ClassBuilder WithStringProperty(string name, SyntaxKind modifier = SyntaxKind.PublicKeyword)
            => WithStringProperty(name, new[] { modifier });
        public ClassBuilder WithStringProperty(string name, params SyntaxKind[] modifiers)
            => WithProperty(name, PredefinedType(Token(SyntaxKind.StringKeyword)), modifiers);
        public ClassBuilder WithProperty(string name, string type, SyntaxKind modifier = SyntaxKind.PublicKeyword)
            => WithProperty(name, type, new[] { modifier });
        public ClassBuilder WithProperty(string name, string type, params SyntaxKind[] modifiers)
            => WithProperty(name, IdentifierName(type), modifiers);
        public ClassBuilder WithProperty(string name, TypeSyntax type, params SyntaxKind[] modifiers)
        {
            _class = _class.WithAutoProperty(name, type, modifiers);
            return this;
        }
        public ClassBuilder WithExpressionProperty(string name, string type, ExpressionSyntax value, params SyntaxKind[] modifiers)
        {
            var property = SyntaxNodeHelpers.CreateProperty(name, type, value, modifiers)
                .WithGeneratedAttribute();
            _class = _class.AddMembers(property);
            return this;
        }

        public ClassBuilder WithStringField(string name, string value, bool includeGeneratedAttribute = true, params SyntaxKind[] modifiers)
        {
            var fieldDeclaration = SyntaxNodeHelpers.CreateStringFieldDeclaration(name, value, modifiers);
            if (includeGeneratedAttribute)
                fieldDeclaration = fieldDeclaration.WithGeneratedAttribute();
            _class = _class.AddMembers(fieldDeclaration);
            return this;
        }

        public ClassBuilder WithField(string name, string type, params SyntaxKind[] modifiers)
        {
            _class = _class.WithField(name, type, modifiers);
            return this;
        }

        public ClassBuilder WithStaticFieldBackedProperty(string name, string type, params SyntaxKind[] modifiers)
        {
            var fieldName = "s_" + name;
            _class = _class.AddMembers(
                SyntaxNodeHelpers.CreateFieldWithDefaultInitializer(fieldName, type, SyntaxKind.StaticKeyword, SyntaxKind.ReadOnlyKeyword)
                    .WithGeneratedAttribute(),
                SyntaxNodeHelpers.CreateProperty(name, type, IdentifierName(fieldName), modifiers)
                    .WithGeneratedAttribute());
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
