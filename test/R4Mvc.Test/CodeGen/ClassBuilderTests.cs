using Microsoft.CodeAnalysis.CSharp;
using R4Mvc.Tools.CodeGen;
using Xunit;

namespace R4Mvc.Test.CodeGen
{
    public class ClassBuilderTests
    {
        const string GeneratedAttribute = "[GeneratedCode(\"R4Mvc\",\"1.0\")]";
        const string GeneratedNonUserCodeAttribute = "[GeneratedCode(\"R4Mvc\",\"1.0\"),DebuggerNonUserCode]";

        [Theory]
        [InlineData("ClassName")]
        [InlineData("Entity")]
        public void Class_Name(string name)
        {
            var classBuilder = new ClassBuilder(name);
            var result = classBuilder.Build();

            result.AssertIsClass(name);
            Assert.Equal(name, classBuilder.Name);
            Assert.Empty(result.GetModifiers());
            Assert.Equal($"class{name}{{}}", result.ToString());
        }

        [Fact]
        public void Class_Generated()
        {
            var classBuilder = new ClassBuilder("ClassName");

            Assert.False(classBuilder.IsGenerated);
            Assert.Equal("classClassName{}", classBuilder.Build().ToString());

            classBuilder.WithGeneratedNonUserCodeAttributes();

            Assert.True(classBuilder.IsGenerated);
            Assert.Equal($"{GeneratedNonUserCodeAttribute}classClassName{{}}", classBuilder.Build().ToString());
        }

        [Theory]
        [InlineData(SyntaxKind.PublicKeyword)]
        [InlineData(SyntaxKind.InternalKeyword)]
        [InlineData(SyntaxKind.PublicKeyword, SyntaxKind.PartialKeyword)]
        public void Class_Modifiers(params SyntaxKind[] modifiers)
        {
            var result = new ClassBuilder("ClassName")
                .WithModifiers(modifiers)
                .Build();

            result.AssertIs(modifiers);
            Assert.Equal($"{modifiers.ToStringModifiers()}classClassName{{}}", result.ToString());
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

            Assert.Equal($"classClassName<{string.Join(",", typeParameters)}>{{}}", result.ToString());
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
        [InlineData("Name", "object", "Entity", SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword)]
        [InlineData("Name", "object", "Entity", SyntaxKind.PublicKeyword, SyntaxKind.ConstKeyword)]
        [InlineData("_id", "Entity", "Entity2", SyntaxKind.PrivateKeyword)]
        [InlineData("_id", "Entity", "Entity2")]
        public void Class_WithField(string name, string type, string valueType, params SyntaxKind[] modifiers)
        {
            var result = new ClassBuilder("ClassName")
                .WithField(name, type, valueType, modifiers)
                .Build();

            Assert.Equal($"classClassName{{{modifiers.ToStringModifiers()}{type}{name}=new{valueType}();}}", result.ToString());
        }

        [Theory]
        [InlineData("Name", "object", "Name", SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword)]
        [InlineData("Name", "object", "Name", SyntaxKind.PublicKeyword, SyntaxKind.ConstKeyword)]
        [InlineData("_id", "Entity", "StaticType.Name", SyntaxKind.PrivateKeyword)]
        [InlineData("_id", "Entity", "StaticType.Name")]
        public void Class_WithValueField(string name, string type, string value, params SyntaxKind[] modifiers)
        {
            var result = new ClassBuilder("ClassName")
                .WithValueField(name, type, value, modifiers)
                .Build();

            Assert.Equal($"classClassName{{{modifiers.ToStringModifiers()}{type}{name}={value};}}", result.ToString());
        }

        [Theory]
        [InlineData("Name", "name1", true, SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword)]
        [InlineData("Email", "email", false, SyntaxKind.PublicKeyword, SyntaxKind.ConstKeyword)]
        [InlineData("_id", "Entity", false)]
        public void Class_WithStringField(string name, string value, bool classGenerated, params SyntaxKind[] modifiers)
        {
            var classBuilder = new ClassBuilder("ClassName");
            if (classGenerated)
                classBuilder.WithGeneratedNonUserCodeAttributes();

            var result = classBuilder
                .WithStringField(name, value, modifiers)
                .Build();

            Assert.Equal($"{(classGenerated ? GeneratedNonUserCodeAttribute : null)}classClassName{{{(!classGenerated ? GeneratedAttribute : null)}{modifiers.ToStringModifiers()}string{name}=\"{value}\";}}", result.ToString());
        }

        [Theory]
        [InlineData("Name", "object", SyntaxKind.PublicKeyword)]
        [InlineData("_id", "Entity", SyntaxKind.PrivateKeyword)]
        public void Class_WithProperty(string name, string type, SyntaxKind modifier)
        {
            var result = new ClassBuilder("ClassName")
                .WithProperty(name, type, modifier)
                .Build();

            Assert.Equal($@"classClassName{{{modifier.ToStringModifier()}{type}{name}{{get;set;}}}}", result.ToString());
        }

        [Theory]
        [InlineData("Name", "object", true, SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword)]
        [InlineData("Name", "object", false, SyntaxKind.PublicKeyword, SyntaxKind.ConstKeyword)]
        [InlineData("_id", "Entity", true, SyntaxKind.PrivateKeyword)]
        [InlineData("_id", "Entity", false)]
        public void Class_WithFieldBackedProperty(string name, string type, bool classGenerated, params SyntaxKind[] modifiers)
        {
            var classBuilder = new ClassBuilder("ClassName");
            if (classGenerated)
                classBuilder.WithGeneratedNonUserCodeAttributes();

            var result = classBuilder
                .WithStaticFieldBackedProperty(name, type, modifiers)
                .Build();

            Assert.Equal($"{(classGenerated ? GeneratedNonUserCodeAttribute : null)}classClassName{{{(!classGenerated ? GeneratedAttribute : null)}static readonly {type}s_{name}=new{type}();{(!classGenerated ? GeneratedNonUserCodeAttribute : null)}{modifiers.ToStringModifiers()}{type}{name}=>s_{name};}}", result.ToString());
        }

        [Theory]
        [InlineData("Name", "object", "OtherProp", SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword)]
        [InlineData("Name", "object", "OtherProp.Name", SyntaxKind.PublicKeyword, SyntaxKind.ConstKeyword)]
        [InlineData("_id", "Entity", "ID", SyntaxKind.PrivateKeyword)]
        [InlineData("_id", "Entity", "ID")]
        public void Class_WithExpressionProperty(string name, string type, string source, params SyntaxKind[] modifiers)
        {
            var result = new ClassBuilder("ClassName")
                .WithExpressionProperty(name, type, source, modifiers)
                .Build();

            Assert.Equal($"classClassName{{{GeneratedNonUserCodeAttribute}{modifiers.ToStringModifiers()}{type}{name}=>{source};}}", result.ToString());
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
