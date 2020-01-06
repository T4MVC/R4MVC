using System;
using Microsoft.CodeAnalysis.CSharp;
using R4Mvc.Tools.CodeGen;
using Xunit;

namespace R4Mvc.Test.CodeGen
{
    public class MethodBuilderTests
    {
        const string GeneratedNonUserCodeAttribute = "[GeneratedCode(\"R4Mvc\",\"1.0\"),DebuggerNonUserCode]";

        [Theory]
        [InlineData("VoidMethod", null)]
        [InlineData("Method", "string")]
        public void Method_Name(string name, string returnType)
        {
            var result = new MethodBuilder(name, returnType)
                .Build();

            Assert.Equal($"{returnType ?? "void"}{name}(){{}}", result.ToString());
        }

        [Theory]
        [InlineData(null)]
        [InlineData(SyntaxKind.PublicKeyword)]
        [InlineData(SyntaxKind.PrivateKeyword, SyntaxKind.StaticKeyword)]
        public void Method_Modifiers(params SyntaxKind[] modifiers)
        {
            if (modifiers == null)
                modifiers = Array.Empty<SyntaxKind>();
            var result = new MethodBuilder("MethodName")
                .WithModifiers(modifiers)
                .Build();

            Assert.Equal($"{modifiers.ToStringModifiers()}voidMethodName(){{}}", result.ToString());
        }

        [Fact]
        public void Method_Generated()
        {
            var result = new MethodBuilder("MethodName")
                .WithGeneratedNonUserCodeAttributes()
                .Build();

            Assert.Equal($"{GeneratedNonUserCodeAttribute}voidMethodName(){{}}", result.ToString());
        }

        [Fact]
        public void Method_NoBody()
        {
            var result = new MethodBuilder("MethodName")
                .WithNoBody()
                .Build();

            Assert.Equal("voidMethodName();", result.ToString());
        }

        [Fact]
        public void Method_NonAction()
        {
            var result = new MethodBuilder("MethodName")
                .WithNonActionAttribute()
                .Build();

            Assert.Equal("[NonAction]voidMethodName(){}", result.ToString());
        }

        [Fact]
        public void Method_NonHandler()
        {
            var result = new MethodBuilder("MethodName")
                .WithNonHandlerAttribute()
                .Build();

            Assert.Equal("[NonHandler]voidMethodName(){}", result.ToString());
        }

        [Theory]
        [InlineData("name", "string")]
        [InlineData("name", "string", true)]
        [InlineData("argument", "ActionResult")]
        public void Method_WithParameter(string name, string type, bool defaultsToNull = false)
        {
            var result = new MethodBuilder("MethodName")
                .WithParameter(name, type, defaultsToNull)
                .Build();

            Assert.Equal($"voidMethodName({type}{name}{(defaultsToNull ? "=null" : null)}){{}}", result.ToString());
        }

        [Fact]
        public void Method_WithMultipleParameters()
        {
            var result = new MethodBuilder("MethodName")
                .WithParameter("id", "int")
                .WithParameter("name", "string", true)
                .Build();

            Assert.Equal("voidMethodName(intid,stringname=null){}", result.ToString());
        }

        [Fact]
        public void Method_WithBody()
        {
            var result = new MethodBuilder("MethodName")
                .WithBody(b => b.ReturnVariable("result"))
                .Build();

            Assert.Equal("voidMethodName(){returnresult;}", result.ToString());
        }

        [Fact]
        public void Method_WithExpressionBody()
        {
            var result = new MethodBuilder("MethodName")
                .WithExpresisonBody(BodyBuilder.MethodCallExpression("entity", "Method", new object[0]))
                .Build();

            Assert.Equal("voidMethodName()=>entity.Method();", result.ToString());
        }
    }
}
