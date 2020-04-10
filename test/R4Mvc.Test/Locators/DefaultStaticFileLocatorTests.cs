using R4Mvc.Tools;
using R4Mvc.Tools.Locators;
using Xunit;

namespace R4Mvc.Test.Locators
{
    public class DefaultStaticFileLocatorTests
    {
        [Fact]
        public void StaticFileLocator()
        {
            var settings = new Settings();
            var locator = new DefaultStaticFileLocator(VirtualFileLocator.Default, settings);
            Assert.Collection(locator.Find(@"D:\Project", @"D:\Project\wwwroot"),
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

        [Fact]
        public void StaticFileLocator_Exclusions()
        {
            var settings = new Settings { ExcludedStaticFileExtensions = new[] { ".ico", ".css" } };
            var locator = new DefaultStaticFileLocator(VirtualFileLocator.Default, settings);
            Assert.Collection(locator.Find(@"D:\Project", @"D:\Project\wwwroot"),
                f => Assert.Equal("lib/jslib/core.js", f.RelativePath.ToString()),
                f => Assert.Equal("js/site.js", f.RelativePath.ToString())
            );
        }
    }
}
