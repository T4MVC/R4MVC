using R4Mvc.Tools.Extensions;
using Xunit;

namespace R4Mvc.Test.Extensions
{
    public class SimpleExtensionsTests
    {
        [Theory]
        [InlineData("helloworld", "helloworld")]
        [InlineData("hello-world", "hello_world")]
        [InlineData("hello world", "hello_world")]
        [InlineData("hello%world", "hello_world")]
        [InlineData("hello % world", "hello___world")]
        [InlineData("helloworld^", "helloworld_")]
        [InlineData("helloworld^^", "helloworld__")]
        [InlineData("^helloworld", "_helloworld")]
        [InlineData("^^helloworld", "__helloworld")]
        [InlineData("helloworld0", "helloworld0")]
        [InlineData("hello0world", "hello0world")]
        [InlineData("0helloworld", "_0helloworld")]
        public void SanitiseName(string name, string sanitisedName)
        {
            Assert.Equal(sanitisedName, name.SanitiseFieldName());
        }
    }
}
