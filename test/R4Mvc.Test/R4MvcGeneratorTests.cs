using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Moq;
using R4Mvc.Tools;
using R4Mvc.Tools.Services;
using Xunit;

namespace R4Mvc.Test
{
    public class R4MvcGeneratorTests
    {
        private IFilePersistService DummyPersistService => new Mock<IFilePersistService>().Object;
        private R4MvcGeneratorService GetGeneratorService(
            IControllerRewriterService controllerRewriter = null,
            IControllerGeneratorService controllerGenerator = null,
            IStaticFileGeneratorService staticFileGenerator = null,
            IFilePersistService filePersistService = null,
            IViewLocatorService viewLocator = null,
            Settings settings = null)
            => new R4MvcGeneratorService(controllerRewriter, controllerGenerator, staticFileGenerator, filePersistService ?? DummyPersistService, viewLocator, settings ?? new Settings());

        [Fact]
        public void ActionResultClass()
        {
            var service = GetGeneratorService();
            var actionClass = service.ActionResultClass()
                .AssertIs(SyntaxKind.InternalKeyword, SyntaxKind.PartialKeyword)
                .AssertName(Constants.ActionResultClass);
            Assert.Collection(actionClass.BaseList.Types,
                t => Assert.Equal("ActionResult", (t.Type as IdentifierNameSyntax).Identifier.Value),
                t => Assert.Equal("IR4MvcActionResult", (t.Type as IdentifierNameSyntax).Identifier.Value));
            Assert.Contains(actionClass.Members,
                m =>
                {
                    var constructor = Assert.IsType<ConstructorDeclarationSyntax>(m).AssertIsPublic();
                    Assert.Equal(4, constructor.ParameterList.Parameters.Count);
                    return true;
                });
        }

        [Fact]
        public void JsonResultClass()
        {
            var service = GetGeneratorService();
            var actionClass = service.JsonResultClass()
                .AssertIs(SyntaxKind.InternalKeyword, SyntaxKind.PartialKeyword)
                .AssertName(Constants.JsonResultClass);
            Assert.Collection(actionClass.BaseList.Types,
                t => Assert.Equal("JsonResult", (t.Type as IdentifierNameSyntax).Identifier.Value),
                t => Assert.Equal("IR4MvcActionResult", (t.Type as IdentifierNameSyntax).Identifier.Value));
            Assert.Contains(actionClass.Members,
                m =>
                {
                    var constructor = Assert.IsType<ConstructorDeclarationSyntax>(m).AssertIsPublic();
                    Assert.Equal(4, constructor.ParameterList.Parameters.Count);
                    return true;
                });
        }
    }
}
