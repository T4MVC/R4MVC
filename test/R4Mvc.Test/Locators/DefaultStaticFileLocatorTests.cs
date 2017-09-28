using R4Mvc.Tools.Locators;
using Xunit;

namespace R4Mvc.Test.Locators
{
    public class DefaultStaticFileLocatorTests
    {
        [Fact]
        public void StaticFileLocator()
        {
            var locator = new DefaultStaticFileLocator(VirtualFileLocator.Default);
            Assert.Collection(locator.Find(@"D:\Project\wwwroot"),
                f =>
                {
                    Assert.Equal("core.js", f.FileName);
                    Assert.Equal("lib/jslib/core.js", f.RelativePath.ToString());
                    Assert.Equal("lib/jslib", f.Container);
                },
                f =>
                {
                    Assert.Equal("site.js", f.FileName);
                    Assert.Equal("js/site.js", f.RelativePath.ToString());
                    Assert.Equal("js", f.Container);
                },
                f =>
                {
                    Assert.Equal("site.css", f.FileName);
                    Assert.Equal("css/site.css", f.RelativePath.ToString());
                    Assert.Equal("css", f.Container);
                },
                f =>
                {
                    Assert.Equal("favicon.ico", f.FileName);
                    Assert.Equal("favicon.ico", f.RelativePath.ToString());
                    Assert.Equal("", f.Container);
                }
            );
        }
    }
}
