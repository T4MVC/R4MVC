using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Moq;
using R4Mvc.Tools;
using R4Mvc.Tools.Services;
using Xunit;

namespace R4Mvc.Test.Services
{
    public class R4MvcGeneratorServiceTests
    {
        private IFilePersistService DummyPersistService => new Mock<IFilePersistService>().Object;
        private R4MvcGeneratorService GetGeneratorService(
            IControllerRewriterService controllerRewriter = null,
            IControllerGeneratorService controllerGenerator = null,
            IPageGeneratorService pageGenerator = null,
            IStaticFileGeneratorService staticFileGenerator = null,
            IFilePersistService filePersistService = null,
            Settings settings = null)
        {
            if (settings == null)
                settings = new Settings();
            if (controllerGenerator == null)
                controllerGenerator = new ControllerGeneratorService(settings);
            return new R4MvcGeneratorService(controllerRewriter, controllerGenerator, pageGenerator, staticFileGenerator, filePersistService ?? DummyPersistService, settings);
        }

        [Fact]
        public void ViewControllers()
        {
            var controllers = new[]
            {
                new ControllerDefinition { Name = "Shared" },                           // Root view only controller
                new ControllerDefinition { Name = "Shared", Area = "Admin" },           // Area view only controller
                new ControllerDefinition { Name = "Shared", Namespace = "Project" },    // Regular controller (should be ignored here)
            };
            var settings = new Settings();
            var service = GetGeneratorService(settings: settings);

            var viewControllers = service.CreateViewOnlyControllerClasses(controllers).ToList();
            Assert.Collection(viewControllers,
                c => Assert.Equal("SharedController", c.Identifier.Value),
                c => Assert.Equal("AdminArea_SharedController", c.Identifier.Value));
            Assert.Collection(controllers,
                c => Assert.StartsWith(settings.R4MvcNamespace, c.FullyQualifiedGeneratedName),
                c => Assert.StartsWith(settings.R4MvcNamespace, c.FullyQualifiedGeneratedName),
                c => Assert.StartsWith("Project", c.FullyQualifiedGeneratedName)); // Don't update this field for regular controllers in this method
        }

        [Fact]
        public void ViewControllers_Sort()
        {
            var controllers = new[]
            {
                new ControllerDefinition { Name = "Shared", Area = "Admin" },
                new ControllerDefinition { Name = "Shared2" },
                new ControllerDefinition { Name = "Shared1" },
            };
            var settings = new Settings();
            var service = GetGeneratorService(settings: settings);

            var viewControllers = service.CreateViewOnlyControllerClasses(controllers).ToList();
            Assert.Collection(viewControllers,
                c => Assert.Equal("Shared1Controller", c.Identifier.Value),
                c => Assert.Equal("Shared2Controller", c.Identifier.Value),
                c => Assert.Equal("AdminArea_SharedController", c.Identifier.Value));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("Project.R4")]
        [InlineData("R4MvcCustom")]
        public void ViewControllers_UseSettingsNamespace(string r4Namespace)
        {
            var controllers = new[]
            {
                new ControllerDefinition { Name = "Shared" },
            };
            var settings = new Settings();
            if (r4Namespace != null)
                settings.R4MvcNamespace = r4Namespace;
            var service = GetGeneratorService(settings: settings);

            var viewControllers = service.CreateViewOnlyControllerClasses(controllers).ToList();
            Assert.Collection(controllers, c => Assert.StartsWith(settings.R4MvcNamespace, c.FullyQualifiedGeneratedName));
        }

        [Fact]
        public void AreaClasses()
        {
            var controllers = new[]
            {
                new ControllerDefinition { Name = "Users", Area = "Admin" },
                new ControllerDefinition { Name = "Shared", Area = "Admin" },
                new ControllerDefinition { Name = "Shared" },
            };
            var areaControllers = controllers.ToLookup(c => c.Area);
            var service = GetGeneratorService();

            var areaClasses = service.CreateAreaClasses(areaControllers).ToList();
            Assert.Collection(areaClasses,
                a => Assert.Equal("AdminAreaClass", a.Identifier.Value));
        }

        [Fact]
        public void AreaClasses_Sort()
        {
            var controllers = new[]
            {
                new ControllerDefinition { Name = "Shared", Area = "Admin2" },
                new ControllerDefinition { Name = "Shared", Area = "Admin1" },
            };
            var areaControllers = controllers.ToLookup(c => c.Area);
            var service = GetGeneratorService();

            var areaClasses = service.CreateAreaClasses(areaControllers).ToList();
            Assert.Collection(areaClasses,
                a => Assert.Equal("Admin1AreaClass", a.Identifier.Value),
                a => Assert.Equal("Admin2AreaClass", a.Identifier.Value));
        }

        private void AssertIActionResultClass(ClassDeclarationSyntax actionClass, string className, string baseClassName)
        {
            actionClass
                .AssertIs(SyntaxKind.InternalKeyword, SyntaxKind.PartialKeyword)
                .AssertName(className);
            Assert.Collection(actionClass.BaseList.Types,
                t => Assert.Equal(baseClassName, (t.Type as IdentifierNameSyntax).Identifier.Value),
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
        public void ActionResultClass()
        {
            var service = GetGeneratorService();
            var actionClass = service.ActionResultClass();
            AssertIActionResultClass(actionClass, Constants.ActionResultClass, "ActionResult");
        }

        [Fact]
        public void JsonResultClass()
        {
            var service = GetGeneratorService();
            var actionClass = service.JsonResultClass();
            AssertIActionResultClass(actionClass, Constants.JsonResultClass, "JsonResult");
        }

        [Fact]
        public void ContentResultClass()
        {
            var service = GetGeneratorService();
            var actionClass = service.ContentResultClass();
            AssertIActionResultClass(actionClass, Constants.ContentResultClass, "ContentResult");
        }

        [Fact]
        public void FileResultClass()
        {
            var service = GetGeneratorService();
            var actionClass = service.FileResultClass();
            AssertIActionResultClass(actionClass, Constants.FileResultClass, "FileResult");
        }

        [Fact]
        public void RedirectResultClass()
        {
            var service = GetGeneratorService();
            var actionClass = service.RedirectResultClass();
            AssertIActionResultClass(actionClass, Constants.RedirectResultClass, "RedirectResult");
        }

        [Fact]
        public void RedirectToActionResultClass()
        {
            var service = GetGeneratorService();
            var actionClass = service.RedirectToActionResultClass();
            AssertIActionResultClass(actionClass, Constants.RedirectToActionResultClass, "RedirectToActionResult");
        }

        [Fact]
        public void RedirectToRouteResultClass()
        {
            var service = GetGeneratorService();
            var actionClass = service.RedirectToRouteResultClass();
            AssertIActionResultClass(actionClass, Constants.RedirectToRouteResultClass, "RedirectToRouteResult");
        }
    }
}
