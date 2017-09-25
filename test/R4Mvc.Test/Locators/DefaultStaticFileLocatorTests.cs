using R4Mvc.Tools.Locators;
using Xunit;

namespace R4Mvc.Test.Locators
{
    public class DefaultStaticFileLocatorTests
    {
        private VirtualFileLocator _locator = new VirtualFileLocator(new[]
        {
            @"D:\Project\Program.cs",
            @"D:\Project\Startup.cs",
            @"D:\Project\Areas\Admin\Controllers\HomeController.cs",
            @"D:\Project\Areas\Admin\Views\Home\Index.cshtml",
            @"D:\Project\Areas\Admin\Views\Shared\EditorTemplates\User.cshtml",
            @"D:\Project\Areas\Admin\Views\Shared\_Layout.cshtml",
            @"D:\Project\Controllers\UsersController.cshtml",
            @"D:\Project\Views\EditorTemplates\User.cshtml",
            @"D:\Project\Views\Users\EditorTemplates\User.cshtml",
            @"D:\Project\Views\Users\Index.cshtml",
            @"D:\Project\Views\Users\Details.cshtml",
            @"D:\Project\wwwroot\lib\jslib\core.js",
            @"D:\Project\wwwroot\js\site.js",
            @"D:\Project\wwwroot\css\site.css",
            @"D:\Project\wwwroot\favicon.ico",
        });

        [Fact]
        public void StaticFileLocator()
        {
            var locator = new DefaultStaticFileLocator(_locator);
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
