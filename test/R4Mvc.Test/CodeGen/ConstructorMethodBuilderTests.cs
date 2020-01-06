using System;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using R4Mvc.Tools.CodeGen;
using Xunit;

namespace R4Mvc.Test.CodeGen
{
    public class ConstructorMethodBuilderTests
    {
        [Theory]
        [InlineData("ClassName")]
        [InlineData("Entity")]
        public void Constructor(string name)
        {
            var result = new ConstructorMethodBuilder(name)
                .Build();

            Assert.Equal(name + "(){}", result.ToString());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("p1")]
        [InlineData("p1", "p2")]
        public void Constructor_WithBaseConstructor(params string[] arguments)
        {
            if (arguments == null)
                arguments = Array.Empty<string>();
            var result = new ConstructorMethodBuilder("ClassName")
                .WithBaseConstructorCall(arguments.Select(a => SyntaxFactory.IdentifierName(a)).ToArray())
                .Build();

            Assert.Equal($"ClassName():base({string.Join(",", arguments)}){{}}", result.ToString());
        }
    }
}
