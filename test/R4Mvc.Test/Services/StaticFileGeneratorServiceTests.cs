using Microsoft.CodeAnalysis.CSharp;
using R4Mvc.Test.Locators;
using R4Mvc.Tools.Locators;
using R4Mvc.Tools.Services;
using Xunit;

namespace R4Mvc.Test.Services
{
    public class StaticFileGeneratorServiceTests
    {
        [Fact]
        public void CreateLinks()
        {
            var settings = new Tools.Settings();
            var staticFileGeneratorService = new StaticFileGeneratorService(new IStaticFileLocator[0], settings);
            var result = staticFileGeneratorService.GenerateStaticFiles(VirtualFileLocator.ProjectRoot);
            result.AssertIsClass(settings.LinksNamespace).AssertIs(SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword, SyntaxKind.PartialKeyword);
        }

        [Fact]
        public void AddStaticFiles()
        {
            var staticFileLocator = new DefaultStaticFileLocator(VirtualFileLocator.Default);
            var staticFiles = staticFileLocator.Find(VirtualFileLocator.ProjectRoot_wwwroot);
            var staticFileGeneratorService = new StaticFileGeneratorService(new[] { staticFileLocator }, new Tools.Settings());

            var c = SyntaxFactory.ClassDeclaration("Test");
            c = staticFileGeneratorService.AddStaticFiles(c, string.Empty, staticFiles);

            Assert.Collection(c.Members,
                m =>
                {
                    var pathClass = m.AssertIsClass("css");
                    Assert.Collection(pathClass.Members, m2 => m2.AssertIsSingleField("site_css"));
                },
                m =>
                {
                    var pathClass = m.AssertIsClass("js");
                    Assert.Collection(pathClass.Members, m2 => m2.AssertIsSingleField("site_js"));
                },
                m =>
                {
                    var pathClass = m.AssertIsClass("lib");
                    Assert.Collection(pathClass.Members, m2 =>
                    {
                        var pathClass2 = m2.AssertIsClass("jslib");
                        Assert.Collection(pathClass2.Members, m3 => m3.AssertIsSingleField("core_js"));
                    });
                },
                m => m.AssertIsSingleField("favicon_ico")
            );
        }
    }
}
