using System;
using R4Mvc.Tools;
using Xunit;

namespace R4Mvc.Test
{
    public class ControllerDefinitionTests
    {
        /// <summary>
        /// Want to make sure that GetFilePath never returns null or empty string!
        /// </summary>
        [Fact]
        public void GetFilePath_NoPaths()
        {
            Assert.Throws<InvalidOperationException>(delegate
            {
                new ControllerDefinition().GetFilePath();
            });
        }

        [Fact]
        public void GetFilePath()
        {
            var controller = new ControllerDefinition
            {
                FilePaths =
                {
                    "/Controllers/Controller.cs",
                },
            };

            Assert.Equal("/Controllers/Controller.cs", controller.GetFilePath());
        }

        [Fact]
        public void GetFilePath_Sort_WrongPath()
        {
            var controller = new ControllerDefinition
            {
                FilePaths =
                {
                    "/AFirstController.cs",
                    "/XFirstController.cs",
                    "/Controllers/Controller.cs",
                    "/ALastController.cs",
                    "/XLastController.cs",
                },
            };

            Assert.Equal("/Controllers/Controller.cs", controller.GetFilePath());
        }

        [Fact]
        public void GetFilePath_Sort_Generated()
        {
            var controller = new ControllerDefinition
            {
                FilePaths =
                {
                    "/Controllers/AController.generated.cs",
                    "/Controllers/XController.generated.cs",
                    "/Controllers/Controller.cs",
                    "/Controllers/AController.generated.cs",
                    "/Controllers/XController.generated.cs",
                },
            };

            Assert.Equal("/Controllers/Controller.cs", controller.GetFilePath());
        }

        [Fact]
        public void FullyQualifiedName_Default()
        {
            var controller = new ControllerDefinition
            {
                Namespace = "Project.Root",
                Name = "Home",
            };
            Assert.Equal($"{controller.Namespace}.{controller.Name}Controller", controller.FullyQualifiedGeneratedName);
        }

        [Fact]
        public void FullyQualifiedName_Custom()
        {
            var customName = "Project.Root.Controllers.HomeController";
            var controller = new ControllerDefinition
            {
                Namespace = "Project.Root",
                Name = "Home",
                FullyQualifiedGeneratedName = customName,
            };
            Assert.Equal(customName, controller.FullyQualifiedGeneratedName);
        }

        [Fact]
        public void Views_NotNull() => Assert.NotNull(new ControllerDefinition().Views);

        [Fact]
        public void FilePaths_NotNull() => Assert.NotNull(new ControllerDefinition().FilePaths);
    }
}
