using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

namespace R4Mvc.Test
{
    public static class TestExtensions
    {
        public static ClassDeclarationSyntax AssertIs(this ClassDeclarationSyntax @class, params SyntaxKind[] kinds)
        {
            foreach (var kind in kinds)
                Assert.Contains(@class.Modifiers, m => m.IsKind(kind));
            return @class;
        }
        public static ClassDeclarationSyntax AssertIsPublic(this ClassDeclarationSyntax @class)
        {
            Assert.Contains(@class.Modifiers, m => m.IsKind(SyntaxKind.PublicKeyword));
            return @class;
        }
        public static ConstructorDeclarationSyntax AssertIsPublic(this ConstructorDeclarationSyntax constructor)
        {
            Assert.Contains(constructor.Modifiers, m => m.IsKind(SyntaxKind.PublicKeyword));
            return constructor;
        }
        public static PropertyDeclarationSyntax AssertIsPublic(this PropertyDeclarationSyntax property)
        {
            Assert.Contains(property.Modifiers, m => m.IsKind(SyntaxKind.PublicKeyword));
            return property;
        }
        public static MethodDeclarationSyntax AssertIsPublic(this MethodDeclarationSyntax method)
        {
            Assert.Contains(method.Modifiers, m => m.IsKind(SyntaxKind.PublicKeyword));
            return method;
        }

        public static ClassDeclarationSyntax AssertName(this ClassDeclarationSyntax @class, string expectedName)
        {
            Assert.Equal(expectedName, @class.Identifier.Value);
            return @class;
        }
        public static PropertyDeclarationSyntax AssertName(this PropertyDeclarationSyntax property, string expectedName)
        {
            Assert.Equal(expectedName, property.Identifier.Value);
            return property;
        }
        public static MethodDeclarationSyntax AssertName(this MethodDeclarationSyntax method, string expectedName)
        {
            Assert.Equal(expectedName, method.Identifier.Value);
            return method;
        }

        public static ClassDeclarationSyntax AssertIsClass(this MemberDeclarationSyntax member, string name)
        {
            var classMember = Assert.IsType<ClassDeclarationSyntax>(member);
            Assert.Equal(name, classMember.Identifier.Value);
            return classMember;
        }

        public static FieldDeclarationSyntax AssertIsSingleField(this MemberDeclarationSyntax member, string name)
        {
            var field = Assert.IsType<FieldDeclarationSyntax>(member);
            Assert.Collection(field.Declaration.Variables, v =>
            {
                var variable = Assert.IsType<VariableDeclaratorSyntax>(v);
                Assert.Equal(name, variable.Identifier.Value);
            });
            return field;
        }
    }
}
