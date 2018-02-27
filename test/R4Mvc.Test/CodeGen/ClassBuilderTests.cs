using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using R4Mvc.Tools.CodeGen;
using Xunit;


namespace R4Mvc.Test.CodeGen
{
    public class ClassBuilderTests
    {
        [Fact]
        public void Class()
        {
            var className = "className";

            var result = new ClassBuilder(className)

                .Build();

            result.AssertIsClass(className);
        }
    }
}
