using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using R4Mvc.Tools.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace R4Mvc.Tools.CodeGen
{
    public class ClassBuilder
    {
        private readonly string _className;
        private ClassDeclarationSyntax _class;

        public ClassBuilder(string className)
        {
            _className = className;
            _class = ClassDeclaration(className);
        }

        public ClassBuilder WithModifiers(params SyntaxKind[] modifiers)
        {
            _class = _class.WithModifiers(modifiers);
            return this;
        }

        public ClassBuilder WithBaseTypes(params string[] classNames)
        {
            _class = _class.WithBaseTypes(classNames);
            return this;
        }

        public ClassBuilder WithMember(MemberDeclarationSyntax method)
        {
            _class = _class.AddMembers(method);
            return this;
        }

        public ClassBuilder WithMethod(MemberDeclarationSyntax method) => WithMember(method);
        public ClassBuilder WithConstructor(MemberDeclarationSyntax method) => WithMember(method);

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

        public ClassBuilder WithField(string name, string value, bool includeGeneratedAttribute = true, params SyntaxKind[] modifiers)
        {
            var fieldDeclaration = SyntaxNodeHelpers.CreateStringFieldDeclaration(name, value, modifiers);
            if (includeGeneratedAttribute)
                fieldDeclaration = fieldDeclaration.WithGeneratedAttribute();
            _class = _class.AddMembers(fieldDeclaration);
            return this;
        }

        public ClassBuilder WithFieldBackedProperty()
        {
            return this;
        }

        internal ClassDeclarationSyntax Build()
        {
            return _class;
        }
    }
}
