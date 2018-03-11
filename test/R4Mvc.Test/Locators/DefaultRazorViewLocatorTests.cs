using R4Mvc.Tools;
using R4Mvc.Tools.Locators;
using Xunit;

namespace R4Mvc.Test.Locators
{
    public class DefaultRazorViewLocatorTests : RazorViewLocatorTestsBase
    {
        [Fact]
        public void BasicProject()
        {
            var locator = new DefaultRazorViewLocator(VirtualFileLocator.Default, new Settings());
            Assert.Collection(locator.Find(@"D:\Project"),
                v => AssertView(v, "", "EditorTemplates", "User", null, "~/Views/EditorTemplates/User.cshtml"),
                v => AssertView(v, "", "Users", "Index", null, "~/Views/Users/Index.cshtml"),
                v => AssertView(v, "", "Users", "Details", null, "~/Views/Users/Details.cshtml"),
                v => AssertView(v, "", "Users", "User", "EditorTemplates", "~/Views/Users/EditorTemplates/User.cshtml"),
                v => AssertView(v, "", "Users", "ProToolbar", "Toolbars", "~/Views/Users/Toolbars/ProToolbar.cshtml"),
                v => AssertView(v, "Admin", "Home", "Index",  null, "~/Areas/Admin/Views/Home/Index.cshtml"),
                v => AssertView(v, "Admin", "Shared", "_Layout", null, "~/Areas/Admin/Views/Shared/_Layout.cshtml"),
                v => AssertView(v, "Admin", "Shared", "User", "EditorTemplates",  "~/Areas/Admin/Views/Shared/EditorTemplates/User.cshtml")
            );
        }

        [Fact]
        public void AreaAsProjectPath()
        {
            var locator = new DefaultRazorViewLocator(VirtualFileLocator.Default, new Settings());
            Assert.Collection(locator.Find(@"D:\Project\Areas\Admin"),
                v => AssertView(v, "", "Home", "Index", null, "~/Views/Home/Index.cshtml"),
                v => AssertView(v, "", "Shared", "_Layout", null, "~/Views/Shared/_Layout.cshtml"),
                v => AssertView(v, "", "Shared", "User", "EditorTemplates", "~/Views/Shared/EditorTemplates/User.cshtml")
            );

        }

        [Theory]
        [InlineData(@"C:\Project")]
        [InlineData(@"D:\")]
        [InlineData(@"D:\Project\Views")]
        [InlineData(@"D:\Project\Areas")]
        public void WrongProjectPaths(string path)
        {
            var locator = new DefaultRazorViewLocator(VirtualFileLocator.Default, new Settings());
            Assert.Empty(locator.Find(path));
        }
    }
}
