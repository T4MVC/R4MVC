using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Moq;
using Moq.Protected;
using R4Mvc.Tools;
using Xunit;

namespace R4Mvc.Test
{
    public class ControllerRewriterTests
    {
        #region Mocks

        private INamedTypeSymbol GetClass(string name, INamedTypeSymbol baseClass = null)
        {
            var baseControllerSymbol = new Mock<INamedTypeSymbol>();
            baseControllerSymbol.Setup(s => s.ToString()).Returns(name);
            baseControllerSymbol.SetupGet(s => s.TypeKind).Returns(TypeKind.Class);
            baseControllerSymbol.SetupGet(s => s.BaseType).Returns(baseClass);
            return baseControllerSymbol.Object;
        }

        private INamedTypeSymbol GetMvcControllerClass() => GetClass("Microsoft.AspNetCore.Mvc.Controller", GetClass("Microsoft.AspNetCore.Mvc.ControllerBase"));

        private INamedTypeSymbol GetExcludedAttribute() => GetClass("Microsoft.AspNetCore.Mvc.R4MvcExcludeAttribute");

        private AttributeData GetAttributeData(INamedTypeSymbol attribute)
        {
            var excludedAttributeData = new Mock<AttributeData>();
            excludedAttributeData.Protected().SetupGet<INamedTypeSymbol>("CommonAttributeClass").Returns(attribute);
            return excludedAttributeData.Object;
        }

        private Mock<INamedTypeSymbol> GetController(bool isPublic = true, bool isAbstract = false, INamedTypeSymbol baseClass = null, bool noBaseClass = false, INamedTypeSymbol attribute = null)
        {
            var symbol = new Mock<INamedTypeSymbol>();
            symbol.Setup(s => s.ToString()).Returns("TestController");
            symbol.SetupGet(s => s.TypeKind).Returns(TypeKind.Class);
            symbol.SetupGet(s => s.DeclaredAccessibility).Returns(isPublic ? Accessibility.Public : Accessibility.Internal);
            symbol.SetupGet(s => s.IsAbstract).Returns(isAbstract);
            symbol.SetupGet(s => s.BaseType).Returns(!noBaseClass ? baseClass ?? GetMvcControllerClass() : null);


            symbol.Setup(s => s.GetAttributes()).Returns(attribute != null
                ? new AttributeData[] { GetAttributeData(attribute) }.ToImmutableArray()
                : new AttributeData[0].ToImmutableArray());
            return symbol;
        }

        #endregion

        [Fact]
        public void ProcessControllers()
        {
            var controllerSymbolMock = GetController();
            var controllerSymbol = controllerSymbolMock.Object;
            Assert.True(ControllerRewriter.ControllerShouldBeProcessed(controllerSymbol));
        }

        [Fact]
        public void ProcessControllers_ControllerSubtype()
        {
            var controllerSymbolMock = GetController(baseClass: GetClass("ControllerBase", GetMvcControllerClass()));
            var controllerSymbol = controllerSymbolMock.Object;
            Assert.True(ControllerRewriter.ControllerShouldBeProcessed(controllerSymbol));
        }

        [Fact]
        public void ProcessControllers_RandomAttribute()
        {
            var controllerSymbolMock = GetController(attribute: GetClass("CustomAttribute"));
            var controllerSymbol = controllerSymbolMock.Object;
            Assert.True(ControllerRewriter.ControllerShouldBeProcessed(controllerSymbol));
        }

        [Fact]
        public void ProcessControllers_NotPublic()
        {
            var controllerSymbolMock = GetController(isPublic: false);
            var controllerSymbol = controllerSymbolMock.Object;
            Assert.False(ControllerRewriter.ControllerShouldBeProcessed(controllerSymbol));
        }

        [Fact]
        public void ProcessControllers_Abstract()
        {
            var controllerSymbolMock = GetController(isAbstract: true);
            var controllerSymbol = controllerSymbolMock.Object;
            Assert.False(ControllerRewriter.ControllerShouldBeProcessed(controllerSymbol));
        }

        [Fact]
        public void ProcessControllers_NoBaseType()
        {
            var controllerSymbolMock = GetController(noBaseClass: true);
            var controllerSymbol = controllerSymbolMock.Object;
            Assert.False(ControllerRewriter.ControllerShouldBeProcessed(controllerSymbol));
        }

        [Fact]
        public void ProcessControllers_AttributeExcluded()
        {
            var controllerSymbolMock = GetController(attribute: GetExcludedAttribute());
            var controllerSymbol = controllerSymbolMock.Object;
            Assert.False(ControllerRewriter.ControllerShouldBeProcessed(controllerSymbol));
        }

        [Fact]
        public void ProcessControllers_NestedAttributeExcluded()
        {
            var controllerSymbolMock = GetController(attribute: GetClass("CustomAttribute", GetExcludedAttribute()));
            var controllerSymbol = controllerSymbolMock.Object;
            Assert.False(ControllerRewriter.ControllerShouldBeProcessed(controllerSymbol));
        }
    }
}
