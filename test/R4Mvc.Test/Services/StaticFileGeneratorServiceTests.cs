using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using R4Mvc.Test.Locators;
using R4Mvc.Tools.CodeGen;
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
            var settings = new Tools.Settings();
            var staticFileLocator = new DefaultStaticFileLocator(VirtualFileLocator.Default, settings);
            var staticFiles = staticFileLocator.Find(VirtualFileLocator.ProjectRoot_wwwroot);
            var staticFileGeneratorService = new StaticFileGeneratorService(new[] { staticFileLocator }, new Tools.Settings());

            var c = new ClassBuilder("Test")
                .WithGeneratedNonUserCodeAttributes();
            staticFileGeneratorService.AddStaticFiles(VirtualFileLocator.ProjectRoot_wwwroot, c, string.Empty, staticFiles);

            Assert.Collection(c.Build().Members,
                m =>
                {
                    var pathClass = m.AssertIsClass("css");
                    Assert.Collection(pathClass.Members,
                        m2 => AssertUrlPathConst(m2, "~/css"),
                        AssertUrlMethod,
                        AssertUrlMethod,
                        m2 => m2.AssertIsSingleField("site_css"));
                },
                m =>
                {
                    var pathClass = m.AssertIsClass("js");
                    Assert.Collection(pathClass.Members,
                        m2 => AssertUrlPathConst(m2, "~/js"),
                        AssertUrlMethod,
                        AssertUrlMethod,
                        m2 => m2.AssertIsSingleField("site_js"));
                },
                m =>
                {
                    var pathClass = m.AssertIsClass("lib");
                    Assert.Collection(pathClass.Members,
                        m2 => AssertUrlPathConst(m2, "~/lib"),
                        AssertUrlMethod,
                        AssertUrlMethod,
                        m2 =>
                        {
                            var pathClass2 = m2.AssertIsClass("jslib");
                            Assert.Collection(pathClass2.Members,
                                m3 => AssertUrlPathConst(m3, "~/lib/jslib"),
                                AssertUrlMethod,
                                AssertUrlMethod,
                                m3 => m3.AssertIsSingleField("core_js"));
                        });
                },
                m => m.AssertIsSingleField("favicon_ico")
            );
        }

        private void AssertUrlPathConst(MemberDeclarationSyntax member, string value)
        {
            var field = Assert.IsType<FieldDeclarationSyntax>(member);
            Assert.Equal($"public const stringUrlPath=\"{value}\";", field.ToString());
        }

        private void AssertUrlMethod(MemberDeclarationSyntax member)
        {
            var method = Assert.IsType<MethodDeclarationSyntax>(member);
            Assert.Equal("Url", method.Identifier.ValueText);
        }
    }
}
