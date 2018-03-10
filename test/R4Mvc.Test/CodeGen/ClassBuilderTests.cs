using Microsoft.CodeAnalysis.CSharp;
using R4Mvc.Tools.CodeGen;
using Xunit;


namespace R4Mvc.Test.CodeGen
{
    public class ClassBuilderTests
    {
        const string GeneratedAttribute = "[GeneratedCode(\"R4Mvc\",\"1.0\")]";

        [Fact]
        public void Class_Plain()
        {
            var className = "ClassName";

            var result = new ClassBuilder(className)
                .Build();

            result.AssertIsClass(className);
            Assert.Empty(result.GetModifiers());
            Assert.Equal("classClassName{}", result.ToString());
        }

        [Theory]
        [InlineData("public", SyntaxKind.PublicKeyword)]
        [InlineData("internal", SyntaxKind.InternalKeyword)]
        [InlineData("public partial", SyntaxKind.PublicKeyword, SyntaxKind.PartialKeyword)]
        public void Class_Modifiers(string expectedModifier, params SyntaxKind[] modifier)
        {
            var result = new ClassBuilder("ClassName")
                .WithModifiers(modifier)
                .Build();

            result.AssertIs(modifier);
            Assert.Equal(expectedModifier + " classClassName{}", result.ToString());
        }

        [Theory]
        [InlineData("ClassBase")]
        [InlineData("ClassBase", "IClass")]
        public void Class_BaseTypes(params string[] parentClasses)
        {
            var result = new ClassBuilder("ClassName")
                .WithBaseTypes(parentClasses)
                .Build();

            var parentClassesString = string.Join(",", parentClasses);
            Assert.Equal($"classClassName:{parentClassesString}{{}}", result.ToString());
        }

        [Theory]
        [InlineData("TEntity")]
        [InlineData("TEntity", "T2")]
        public void Class_TypeParams(params string[] typeParameters)
        {
            var result = new ClassBuilder("ClassName")
                .WithTypeParameters(typeParameters)
                .Build();

            var typeParamsString = string.Join(",", typeParameters);
            Assert.Equal($"classClassName<{typeParamsString}>{{}}", result.ToString());
        }

        [Theory]
        [InlineData("Name", "object", "public static ", SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword)]
        [InlineData("Name", "object", "public const ", SyntaxKind.PublicKeyword, SyntaxKind.ConstKeyword)]
        [InlineData("_id", "Entity", "private ", SyntaxKind.PrivateKeyword)]
        [InlineData("_id", "Entity", "")]
        public void Class_WithField(string name, string type, string expectedModifiers, params SyntaxKind[] modifiers)
        {
            var result = new ClassBuilder("ClassName")
                .WithField(name, type, modifiers)
                .Build();

            Assert.Equal($"classClassName{{{GeneratedAttribute}{expectedModifiers}{type}{name}=new{type}();}}", result.ToString());
        }

        [Theory]
        [InlineData("Name", "object", "Entity", "public static ", SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword)]
        [InlineData("Name", "object", "Entity", "public const ", SyntaxKind.PublicKeyword, SyntaxKind.ConstKeyword)]
        [InlineData("_id", "Entity", "Entity2", "private ", SyntaxKind.PrivateKeyword)]
        [InlineData("_id", "Entity", "Entity2", "")]
        public void Class_WithFieldInitialised(string name, string type, string valueType, string expectedModifiers, params SyntaxKind[] modifiers)
        {
            var result = new ClassBuilder("ClassName")
                .WithFieldInitialised(name, type, valueType, modifiers)
                .Build();

            Assert.Equal($"classClassName{{{expectedModifiers}{type}{name}=new{valueType}();}}", result.ToString());
        }

        [Theory]
        [InlineData("Name", "object", "public ", SyntaxKind.PublicKeyword)]
        [InlineData("_id", "Entity", "private ", SyntaxKind.PrivateKeyword)]
        public void Class_WithProperty(string name, string type, string expectedModifier, SyntaxKind modifier)
        {
            var result = new ClassBuilder("ClassName")
                .WithProperty(name, type, modifier)
                .Build();

            Assert.Equal($@"classClassName{{{expectedModifier}{type}{name}{{get;set;}}}}", result.ToString());
        }
    }
}
