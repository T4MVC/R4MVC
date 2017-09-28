using R4Mvc.Tools.Locators;
using Xunit;

namespace R4Mvc.Test.Locators
{
    public class DefaultRazorViewLocatorTests
    {
        [Fact]
        public void BasicProject()
        {
            var locator = new DefaultRazorViewLocator(VirtualFileLocator.Default);
            Assert.Collection(locator.Find(@"D:\Project"),
                v =>
                {
                    Assert.Equal("", v.AreaName);
                    Assert.Equal("EditorTemplates", v.ControllerName);
                    Assert.Equal("User", v.ViewName);
                    Assert.Equal("~/Views/EditorTemplates/User.cshtml", v.RelativePath.ToString());
                    Assert.Null(v.TemplateKind);
                },
                v =>
                {
                    Assert.Equal("", v.AreaName);
                    Assert.Equal("Users", v.ControllerName);
                    Assert.Equal("Index", v.ViewName);
                    Assert.Equal("~/Views/Users/Index.cshtml", v.RelativePath.ToString());
                    Assert.Null(v.TemplateKind);
                },
                v =>
                {
                    Assert.Equal("", v.AreaName);
                    Assert.Equal("Users", v.ControllerName);
                    Assert.Equal("Details", v.ViewName);
                    Assert.Equal("~/Views/Users/Details.cshtml", v.RelativePath.ToString());
                    Assert.Null(v.TemplateKind);
                },
                v =>
                {
                    Assert.Equal("", v.AreaName);
                    Assert.Equal("Users", v.ControllerName);
                    Assert.Equal("User", v.ViewName);
                    Assert.Equal("~/Views/Users/EditorTemplates/User.cshtml", v.RelativePath.ToString());
                    Assert.Equal("EditorTemplates", v.TemplateKind);
                },
                v =>
                {
                    Assert.Equal("Admin", v.AreaName);
                    Assert.Equal("Home", v.ControllerName);
                    Assert.Equal("Index", v.ViewName);
                    Assert.Equal("~/Areas/Admin/Views/Home/Index.cshtml", v.RelativePath.ToString());
                    Assert.Null(v.TemplateKind);
                },
                v =>
                {
                    Assert.Equal("Admin", v.AreaName);
                    Assert.Equal("Shared", v.ControllerName);
                    Assert.Equal("_Layout", v.ViewName);
                    Assert.Equal("~/Areas/Admin/Views/Shared/_Layout.cshtml", v.RelativePath.ToString());
                    Assert.Null(v.TemplateKind);
                },
                v =>
                {
                    Assert.Equal("Admin", v.AreaName);
                    Assert.Equal("Shared", v.ControllerName);
                    Assert.Equal("User", v.ViewName);
                    Assert.Equal("~/Areas/Admin/Views/Shared/EditorTemplates/User.cshtml", v.RelativePath.ToString());
                    Assert.Equal("EditorTemplates", v.TemplateKind);
                }
            );
        }

        [Fact]
        public void AreaAsProjectPath()
        {
            var locator = new DefaultRazorViewLocator(VirtualFileLocator.Default);
            Assert.Collection(locator.Find(@"D:\Project\Areas\Admin"),
                v =>
                {
                    Assert.Equal("", v.AreaName);
                    Assert.Equal("Home", v.ControllerName);
                    Assert.Equal("Index", v.ViewName);
                    Assert.Equal("~/Views/Home/Index.cshtml", v.RelativePath.ToString());
                    Assert.Null(v.TemplateKind);
                },
                v =>
                {
                    Assert.Equal("", v.AreaName);
                    Assert.Equal("Shared", v.ControllerName);
                    Assert.Equal("_Layout", v.ViewName);
                    Assert.Equal("~/Views/Shared/_Layout.cshtml", v.RelativePath.ToString());
                    Assert.Null(v.TemplateKind);
                },
                v =>
                {
                    Assert.Equal("", v.AreaName);
                    Assert.Equal("Shared", v.ControllerName);
                    Assert.Equal("User", v.ViewName);
                    Assert.Equal("~/Views/Shared/EditorTemplates/User.cshtml", v.RelativePath.ToString());
                    Assert.Equal("EditorTemplates", v.TemplateKind);
                }
            );

        }

        [Theory]
        [InlineData(@"C:\Project")]
        [InlineData(@"D:\")]
        [InlineData(@"D:\Project\Views")]
        [InlineData(@"D:\Project\Areas")]
        public void WrongProjectPaths(string path)
        {
            var locator = new DefaultRazorViewLocator(VirtualFileLocator.Default);
            Assert.Empty(locator.Find(path));
        }
    }
}
