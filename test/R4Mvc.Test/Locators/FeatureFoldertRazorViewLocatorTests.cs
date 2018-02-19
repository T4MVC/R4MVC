using System;
using R4Mvc.Tools;
using R4Mvc.Tools.Locators;
using Xunit;

namespace R4Mvc.Test.Locators
{
    public class FeatureFoldertRazorViewLocatorTests
    {
        ControllerDefinition[] _controllers;

        public FeatureFoldertRazorViewLocatorTests()
        {
            _controllers = new[]
            {
                new ControllerDefinition
                {
                    Area = "Features",
                    FilePaths = new []{ @"D:\Project\Areas\Features\Home\HomeController.cs" },
                },
                new ControllerDefinition
                {
                    Area = "AreaWithFeatures",
                    FilePaths = new []{ @"D:\Project\AreaWithFeatures\Home\HomeController.cs"},
                }
            };
        }

        [Fact]
        public void BasicProject()
        {
            var locator = new FeatureFoldersViewLocator(VirtualFileLocator.FeatureFolders);
            var views = locator.Find(@"D:\Project", _controllers);
            Assert.Collection(views,
                v =>
                {
                    Assert.Equal("Features", v.AreaName);
                    Assert.Equal("Home", v.ControllerName);
                    Assert.Equal("Index", v.ViewName);
                    Assert.Equal(@"~\Areas\Features\Home\Index.cshtml", v.RelativePath.ToString());
                    Assert.Null(v.TemplateKind);
                },
                v =>
                {
                    Assert.Equal("AreaWithFeatures", v.AreaName);
                    Assert.Equal("Home", v.ControllerName);
                    Assert.Equal("Index", v.ViewName);
                    Assert.Equal(@"~\AreaWithFeatures\Home\Index.cshtml", v.RelativePath.ToString());
                    Assert.Null(v.TemplateKind);
                }
            );
        }

    }
}
