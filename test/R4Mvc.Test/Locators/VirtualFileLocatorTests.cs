using Xunit;

namespace R4Mvc.Test.Locators
{
    /// <summary>
    /// Making sure the VirtualFileLocator works as intended :P
    /// </summary>
    public class VirtualFileLocatorTests
    {
        [Theory]
        [InlineData(@"D:\Project")]
        [InlineData(@"D:\Project\")]
        [InlineData(@"D:\Project\Areas")]
        [InlineData(@"D:\Project\Areas\Admin\Views\Home")]
        [InlineData(@"D:\Project\Areas\Admin\Views\Home\")]
        public void PathExists(string path)
        {
            Assert.True(VirtualFileLocator.Default.DirectoryExists(path));
        }

        [Theory]
        [InlineData(@"C:\Project")]
        [InlineData(@"D:\Project\Areas\Views\Home\Index")]
        [InlineData(@"D:\Project\Areas\Views\Home\Index.cshtml")]
        public void PathExists_False(string path)
        {
            Assert.False(VirtualFileLocator.Default.DirectoryExists(path));
        }

        [Fact]
        public void Directories()
        {
            Assert.Collection(VirtualFileLocator.Default.GetDirectories(@"D:\Project"),
                p => Assert.Equal(@"D:\Project\Areas", p),
                p => Assert.Equal(@"D:\Project\Controllers", p),
                p => Assert.Equal(@"D:\Project\Views", p),
                p => Assert.Equal(@"D:\Project\wwwroot", p)
            );
            Assert.Collection(VirtualFileLocator.Default.GetDirectories(@"D:\Project\"),
                p => Assert.Equal(@"D:\Project\Areas", p),
                p => Assert.Equal(@"D:\Project\Controllers", p),
                p => Assert.Equal(@"D:\Project\Views", p),
                p => Assert.Equal(@"D:\Project\wwwroot", p)
            );
            Assert.Collection(VirtualFileLocator.Default.GetDirectories(@"D:\Project\Areas"),
                p => Assert.Equal(@"D:\Project\Areas\Admin", p)
            );
            Assert.Collection(VirtualFileLocator.Default.GetDirectories(@"D:\Project\Areas\Admin"),
                p => Assert.Equal(@"D:\Project\Areas\Admin\Controllers", p),
                p => Assert.Equal(@"D:\Project\Areas\Admin\Views", p)
            );
            Assert.Empty(VirtualFileLocator.Default.GetDirectories(@"D:\Project\Areas\Admin\Controllers"));
            Assert.Empty(VirtualFileLocator.Default.GetDirectories(@"C:\Project"));
        }

        [Fact]
        public void Files()
        {
            Assert.Collection(VirtualFileLocator.Default.GetFiles(@"D:\Project", "*"),
                f => Assert.Equal(@"D:\Project\Program.cs", f),
                f => Assert.Equal(@"D:\Project\Startup.cs", f)
            );
            Assert.Empty(VirtualFileLocator.Default.GetFiles(@"D:\Project\Views", "*"));
            Assert.Empty(VirtualFileLocator.Default.GetFiles(@"D:\Project\Views", "*.cshtml"));
            Assert.Collection(VirtualFileLocator.Default.GetFiles(@"D:\Project\Views", "*.cshtml", true),
                f => Assert.Equal(@"D:\Project\Views\EditorTemplates\User.cshtml", f),
                f => Assert.Equal(@"D:\Project\Views\Users\EditorTemplates\User.cshtml", f),
                f => Assert.Equal(@"D:\Project\Views\Users\Toolbars\ProToolbar.cshtml", f),
                f => Assert.Equal(@"D:\Project\Views\Users\Index.cshtml", f),
                f => Assert.Equal(@"D:\Project\Views\Users\Details.cshtml", f)
            );
            Assert.Collection(VirtualFileLocator.Default.GetFiles(@"D:\Project\Areas\", "*.cshtml", true),
                f => Assert.Equal(@"D:\Project\Areas\Admin\Views\Home\Index.cshtml", f),
                f => Assert.Equal(@"D:\Project\Areas\Admin\Views\Shared\EditorTemplates\User.cshtml", f),
                f => Assert.Equal(@"D:\Project\Areas\Admin\Views\Shared\_Layout.cshtml", f)
            );
            Assert.Collection(VirtualFileLocator.Default.GetFiles(@"D:\Project\wwwroot", "*", true),
                f => Assert.Equal(@"D:\Project\wwwroot\lib\jslib\core.js", f),
                f => Assert.Equal(@"D:\Project\wwwroot\js\site.js", f),
                f => Assert.Equal(@"D:\Project\wwwroot\css\site.css", f),
                f => Assert.Equal(@"D:\Project\wwwroot\favicon.ico", f)
            );
            Assert.Empty(VirtualFileLocator.Default.GetFiles(@"D:\Project\wwwroot", "*.cshtml", true));
            Assert.Empty(VirtualFileLocator.Default.GetFiles(@"C:\Project", "*"));
        }
    }
}
