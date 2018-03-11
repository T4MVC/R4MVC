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

        [Fact]
        public void Class_ChildClass()
        {
            var result = new ClassBuilder("ClassName")
                .WithChildClass("ChildClass", c => c
                    .WithModifiers(SyntaxKind.PrivateKeyword))
                .Build();

            Assert.Equal("classClassName{private classChildClass{}}", result.ToString());
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
        [InlineData("Name", "name1", true, "public static ", SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword)]
        [InlineData("Email", "email", false, "public const ", SyntaxKind.PublicKeyword, SyntaxKind.ConstKeyword)]
        [InlineData("_id", "Entity", false, "")]
        public void Class_WithStringField(string name, string value, bool includeGenerated, string expectedModifiers, params SyntaxKind[] modifiers)
        {
            var result = new ClassBuilder("ClassName")
                .WithStringField(name, value, includeGenerated, modifiers)
                .Build();

            Assert.Equal($"classClassName{{{(includeGenerated ? GeneratedAttribute : null)}{expectedModifiers}string{name}=\"{value}\";}}", result.ToString());
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

        [Theory]
        [InlineData("Name", "public ", SyntaxKind.PublicKeyword)]
        [InlineData("_id", "private ", SyntaxKind.PrivateKeyword)]
        public void Class_WithStringProperty(string name, string expectedModifier, SyntaxKind modifier)
        {
            var result = new ClassBuilder("ClassName")
                .WithStringProperty(name, modifier)
                .Build();

            Assert.Equal($"classClassName{{{expectedModifier}string{name}{{get;set;}}}}", result.ToString());
        }

        [Theory]
        [InlineData("Name", "object", "public static ", SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword)]
        [InlineData("Name", "object", "public const ", SyntaxKind.PublicKeyword, SyntaxKind.ConstKeyword)]
        [InlineData("_id", "Entity", "private ", SyntaxKind.PrivateKeyword)]
        [InlineData("_id", "Entity", "")]
        public void Class_WithAutoProperty(string name, string type, string expectedModifiers, params SyntaxKind[] modifiers)
        {
            var result = new ClassBuilder("ClassName")
                .WithStaticFieldBackedProperty(name, type, modifiers)
                .Build();

            Assert.Equal($"classClassName{{{GeneratedAttribute}static readonly {type}s_{name}=new{type}();{GeneratedAttribute}{expectedModifiers}{type}{name}=>s_{name};}}", result.ToString());
        }

        [Theory]
        [InlineData("Name", "object", "OtherProp", "public static ", SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword)]
        [InlineData("Name", "object", "OtherProp.Name", "public const ", SyntaxKind.PublicKeyword, SyntaxKind.ConstKeyword)]
        [InlineData("_id", "Entity", "ID", "private ", SyntaxKind.PrivateKeyword)]
        [InlineData("_id", "Entity", "ID", "")]
        public void Class_WithExpressionProperty(string name, string type, string source, string expectedModifiers, params SyntaxKind[] modifiers)
        {
            var result = new ClassBuilder("ClassName")
                .WithExpressionProperty(name, type, source, modifiers)
                .Build();

            Assert.Equal($"classClassName{{{GeneratedAttribute}{expectedModifiers}{type}{name}=>{source};}}", result.ToString());
        }

        [Theory]
        [InlineData("Test", "string")]
        [InlineData("Method", "IActionResult")]
        public void Class_Method(string name, string type)
        {
            var result = new ClassBuilder("ClassName")
                .WithMethod(name, type, m => { })
                .Build();

            Assert.Equal($"classClassName{{{type}{name}(){{}}}}", result.ToString());
        }
    }
}
