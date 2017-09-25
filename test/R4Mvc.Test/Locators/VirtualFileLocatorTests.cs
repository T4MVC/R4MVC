using Xunit;

namespace R4Mvc.Test.Locators
{
    /// <summary>
    /// Making sure the VirtualFileLocator works as intended :P
    /// </summary>
    public class VirtualFileLocatorTests
    {
        private VirtualFileLocator _locator = new VirtualFileLocator(new[]
        {
            @"D:\Project\Program.cs",
            @"D:\Project\Startup.cs",
            @"D:\Project\Areas\Admin\Controllers\HomeController.cs",
            @"D:\Project\Areas\Admin\Views\Home\Index.cshtml",
            @"D:\Project\Areas\Admin\Views\Shared\_Layout.cshtml",
            @"D:\Project\Controllers\UsersController.cshtml",
            @"D:\Project\Views\Users\Index.cshtml",
            @"D:\Project\Views\Users\Details.cshtml",
            @"D:\Project\wwwroot\js\site.js",
            @"D:\Project\wwwroot\css\site.css",
            @"D:\Project\wwwroot\favicon.ico",
        });

        [Fact]
        public void VirtualFileLocator_PathExists()
        {
            Assert.True(_locator.DirectoryExists(@"D:\Project"));
            Assert.True(_locator.DirectoryExists(@"D:\Project\"));
            Assert.True(_locator.DirectoryExists(@"D:\Project\Areas"));
            Assert.True(_locator.DirectoryExists(@"D:\Project\Areas\Admin\Views\Home"));
            Assert.True(_locator.DirectoryExists(@"D:\Project\Areas\Admin\Views\Home\"));
            Assert.False(_locator.DirectoryExists(@"C:\Project"));
            Assert.False(_locator.DirectoryExists(@"D:\Project\Areas\Views\Home\Index"));
            Assert.False(_locator.DirectoryExists(@"D:\Project\Areas\Views\Home\Index.cshtml"));
        }

        [Fact]
        public void VirtualFileLocator_Directories()
        {
            Assert.Collection(_locator.GetDirectories(@"D:\Project"),
                p => Assert.Equal(@"D:\Project\Areas", p),
                p => Assert.Equal(@"D:\Project\Controllers", p),
                p => Assert.Equal(@"D:\Project\Views", p),
                p => Assert.Equal(@"D:\Project\wwwroot", p)
            );
            Assert.Collection(_locator.GetDirectories(@"D:\Project\"),
                p => Assert.Equal(@"D:\Project\Areas", p),
                p => Assert.Equal(@"D:\Project\Controllers", p),
                p => Assert.Equal(@"D:\Project\Views", p),
                p => Assert.Equal(@"D:\Project\wwwroot", p)
            );
            Assert.Collection(_locator.GetDirectories(@"D:\Project\Areas"),
                p => Assert.Equal(@"D:\Project\Areas\Admin", p)
            );
            Assert.Collection(_locator.GetDirectories(@"D:\Project\Areas\Admin"),
                p => Assert.Equal(@"D:\Project\Areas\Admin\Controllers", p),
                p => Assert.Equal(@"D:\Project\Areas\Admin\Views", p)
            );
            Assert.Empty(_locator.GetDirectories(@"D:\Project\Areas\Admin\Controllers"));
            Assert.Empty(_locator.GetDirectories(@"C:\Project"));
        }

        [Fact]
        public void VirtualFileLocator_Files()
        {
            Assert.Collection(_locator.GetFiles(@"D:\Project", "*"),
                f => Assert.Equal(@"D:\Project\Program.cs", f),
                f => Assert.Equal(@"D:\Project\Startup.cs", f)
            );
            Assert.Empty(_locator.GetFiles(@"D:\Project\Views", "*"));
            Assert.Empty(_locator.GetFiles(@"D:\Project\Views", "*.cshtml"));
            Assert.Collection(_locator.GetFiles(@"D:\Project\Views", "*.cshtml", true),
                f => Assert.Equal(@"D:\Project\Views\Users\Index.cshtml", f),
                f => Assert.Equal(@"D:\Project\Views\Users\Details.cshtml", f)
            );
            Assert.Collection(_locator.GetFiles(@"D:\Project\wwwroot", "*", true),
                f => Assert.Equal(@"D:\Project\wwwroot\js\site.js", f),
                f => Assert.Equal(@"D:\Project\wwwroot\css\site.css", f),
                f => Assert.Equal(@"D:\Project\wwwroot\favicon.ico", f)
            );
            Assert.Empty(_locator.GetFiles(@"D:\Project\wwwroot", "*.cshtml", true));
            Assert.Empty(_locator.GetFiles(@"C:\Project", "*"));
        }
    }
}
