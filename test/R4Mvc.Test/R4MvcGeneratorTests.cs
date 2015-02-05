using System;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace R4Mvc.Test
{
    public class R4MvcGeneratorTests
    {
        [Fact]
        public void Generate_test()
        {
            var compilation = CSharpCompilation.Create("test");
                        
            R4MvcGenerator.Generate(compilation);
        }
    }
}