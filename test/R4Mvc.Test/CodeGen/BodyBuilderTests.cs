using R4Mvc.Tools.CodeGen;
using Xunit;

namespace R4Mvc.Test.CodeGen
{
    public class BodyBuilderTests
    {
        [Fact]
        public void BodyEmpty()
        {
            var result = new BodyBuilder()
                .Build();

            Assert.Equal("{}", result.ToString());
        }

        [Theory]
        [InlineData("result", "ActionResult", "controller")]
        [InlineData("result", "ActionResult", "controller", "action")]
        [InlineData("obj", "object")]
        public void Body_WithVariableFromObject(string name, string type, params string[] arguments)
        {
            var result = new BodyBuilder()
                .VariableFromNewObject(name, type, arguments)
                .Build();

            Assert.Equal($"{{var{name}=new{type}({string.Join(",", arguments)});}}", result.ToString());
        }

        [Theory]
        [InlineData("result", "action", "ToString")]
        [InlineData("obj", "value", "GetValues", "param1")]
        [InlineData("obj", "value", "GetValues", "param1", "param2")]
        public void Body_WithVariableFromMethod(string name, string entity, string method, params string[] arguments)
        {
            var result = new BodyBuilder()
                .VariableFromMethodCall(name, entity, method, arguments)
                .Build();

            Assert.Equal($"{{var{name}={entity}.{method}({string.Join(",", arguments)});}}", result.ToString());
        }

        [Theory]
        [InlineData("result")]
        [InlineData("value")]
        public void Body_ReturnsVariable(string name)
        {
            var result = new BodyBuilder()
                .ReturnVariable(name)
                .Build();

            Assert.Equal($"{{return{name};}}", result.ToString());
        }

        [Theory]
        [InlineData("ActionResult", "controller")]
        [InlineData("ActionResult", "controller", "action")]
        [InlineData("object")]
        public void Body_ReturnsNewObject(string type, params string[] arguments)
        {
            var result = new BodyBuilder()
                .ReturnNewObject(type, arguments)
                .Build();

            Assert.Equal($"{{returnnew{type}({string.Join(",", arguments)});}}", result.ToString());
        }

        [Theory]
        [InlineData("action", "ToUrl", "controller")]
        [InlineData("action", "ToUrl", "controller", "action")]
        [InlineData("entity", "ToString")]
        public void Body_ReturnsMethodCall(string entity, string method, params string[] arguments)
        {
            var result = new BodyBuilder()
                .ReturnMethodCall(entity, method, arguments)
                .Build();

            Assert.Equal($"{{return{entity}.{method}({string.Join(",", arguments)});}}", result.ToString());
        }

        [Theory]
        [InlineData("action", "ToUrl", "controller")]
        [InlineData("action", "ToUrl", "controller", "action")]
        [InlineData("entity", "ToString")]
        public void Body_MethodCall(string entity, string method, params string[] arguments)
        {
            var result = new BodyBuilder()
                .MethodCall(entity, method, arguments)
                .Build();

            Assert.Equal($"{{{entity}.{method}({string.Join(",", arguments)});}}", result.ToString());
        }

        [Fact]
        public void Body_MultiMethods()
        {
            var result = new BodyBuilder()
                .VariableFromNewObject("result", "object")
                .ReturnVariable("result")
                .Build();

            Assert.Equal("{varresult=newobject();returnresult;}", result.ToString());
        }

        [Fact]
        public void Body_Statement()
        {
            var result = new BodyBuilder()
                .Statement(b => b
                    .VariableFromNewObject("result", "object")
                    .ReturnVariable("result"))
                .Build();

            Assert.Equal("{varresult=newobject();returnresult;}", result.ToString());
        }
    }
}
