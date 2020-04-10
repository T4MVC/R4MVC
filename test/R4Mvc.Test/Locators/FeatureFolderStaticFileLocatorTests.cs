using R4Mvc.Tools;
using R4Mvc.Tools.Locators;
using Xunit;

namespace R4Mvc.Test.Locators
{
    public class FeatureFolderStaticFileLocatorTests
    {
        Settings _settings;
        public FeatureFolderStaticFileLocatorTests()
        {
            _settings = new Settings();
            _settings.FeatureFolders = new Settings.FeatureFoldersClass
            {
                Enabled = true,
                FeatureOnlyAreas = new[] { "Admin" },
                IncludedStaticFileExtensions = new[] { ".js", ".css" },
                StaticFileAccess = true,
            };
        }
        [Fact]
        public void StaticFileLocator()
        {
            var locator = new FeatureFolderStaticFileLocator(VirtualFileLocator.Default, _settings);
            Assert.Collection(locator.Find(@"D:\Project", @"D:\Project\wwwroot"),
                    f =>
                    {
                        Assert.Equal("Index.js", f.FileName);
                        Assert.Equal("Areas/Admin/Features/Home/Index.js", f.RelativePath.ToString());
                        Assert.Equal("Areas/Admin/Features/Home", f.Container);
                    },
                    f =>
                    {
                        Assert.Equal("Index.css", f.FileName);
                        Assert.Equal("Areas/Admin/Features/Home/Index.css", f.RelativePath.ToString());
                        Assert.Equal("Areas/Admin/Features/Home", f.Container);
                    }
                );
        }

        [Fact]
        public void StaticFileLocator_Exclusions()
        {
            _settings.ExcludedStaticFileExtensions = new[] {".css" };
            var locator = new FeatureFolderStaticFileLocator(VirtualFileLocator.Default, _settings);
            Assert.Collection(locator.Find(@"D:\Project", @"D:\Project\wwwroot"),
                f => Assert.Equal("Areas/Admin/Features/Home/Index.js", f.RelativePath.ToString())
            );
        }
    }
}
