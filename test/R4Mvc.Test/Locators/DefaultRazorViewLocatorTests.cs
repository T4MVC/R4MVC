using R4Mvc.Tools.Locators;
using Xunit;

namespace R4Mvc.Test.Locators
{
    public class DefaultRazorViewLocatorTests
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
        public void RazorViewLocator()
        {
            var locator = new DefaultRazorViewLocator(_locator);
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
        public void RazorViewLocator_AreaProjectPath()
        {
            var locator = new DefaultRazorViewLocator(_locator);
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

        [Fact]
        public void RazorViewLocator_WrongProjectPaths()
        {
            var locator = new DefaultRazorViewLocator(_locator);
            Assert.Empty(locator.Find(@"C:\Project"));
            Assert.Empty(locator.Find(@"D:\"));
            Assert.Empty(locator.Find(@"D:\Project\Views"));
            Assert.Empty(locator.Find(@"D:\Project\Areas"));
        }
    }
}
