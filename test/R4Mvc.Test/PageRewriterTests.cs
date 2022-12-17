using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Moq;
using Moq.Protected;
using R4Mvc.Tools;
using Xunit;

namespace R4Mvc.Test
{
    public class PageRewriterTests
    {
        #region Mocks

        private INamedTypeSymbol GetClass(string name, INamedTypeSymbol baseClass = null)
        {
            var mockSymbol = new Mock<INamedTypeSymbol>();
            mockSymbol.Setup(s => s.ToString()).Returns(name);
            mockSymbol.SetupGet(s => s.TypeKind).Returns(TypeKind.Class);
            mockSymbol.SetupGet(s => s.BaseType).Returns(baseClass);
            return mockSymbol.Object;
        }

        private INamedTypeSymbol GetPageModelClass() => GetClass("Microsoft.AspNetCore.Mvc.RazorPages.PageModel");

        private INamedTypeSymbol GetExcludedAttribute() => GetClass("Microsoft.AspNetCore.Mvc.R4MvcExcludeAttribute");

        private AttributeData GetAttributeData(INamedTypeSymbol attribute)
        {
            var excludedAttributeData = new Mock<AttributeData>();
            excludedAttributeData.Protected().SetupGet<INamedTypeSymbol>("CommonAttributeClass").Returns(attribute);
            return excludedAttributeData.Object;
        }

        private Mock<INamedTypeSymbol> GetPage(bool isPublic = true, bool isAbstract = false, INamedTypeSymbol baseClass = null, bool noBaseClass = false, INamedTypeSymbol attribute = null)
        {
            var symbol = new Mock<INamedTypeSymbol>();
            symbol.Setup(s => s.ToString()).Returns("TestPage");
            symbol.SetupGet(s => s.TypeKind).Returns(TypeKind.Class);
            symbol.SetupGet(s => s.DeclaredAccessibility).Returns(isPublic ? Accessibility.Public : Accessibility.Internal);
            symbol.SetupGet(s => s.IsAbstract).Returns(isAbstract);
            symbol.SetupGet(s => s.BaseType).Returns(!noBaseClass ? baseClass ?? GetPageModelClass() : null);

            symbol.Setup(s => s.GetAttributes()).Returns(attribute != null
                ? new AttributeData[] { GetAttributeData(attribute) }.ToImmutableArray()
                : new AttributeData[0].ToImmutableArray());
            return symbol;
        }

        #endregion

        [Fact]
        public void ProcessPages()
        {
            var pageSymbolMock = GetPage();
            var pageSymbol = pageSymbolMock.Object;
            Assert.True(PageRewriter.PageShouldBeProcessed(pageSymbol));
        }

        [Fact]
        public void ProcessPages_PageSubtype()
        {
            var pageSymbolMock = GetPage(baseClass: GetClass("PageBase", GetPageModelClass()));
            var pageSymbol = pageSymbolMock.Object;
            Assert.True(PageRewriter.PageShouldBeProcessed(pageSymbol));
        }

        [Fact]
        public void ProcessPages_RandomAttribute()
        {
            var pageSymbolMock = GetPage(attribute: GetClass("CustomAttribute"));
            var pageSymbol = pageSymbolMock.Object;
            Assert.True(PageRewriter.PageShouldBeProcessed(pageSymbol));
        }

        [Fact]
        public void ProcessPages_NotPublic()
        {
            var pageSymbolMock = GetPage(isPublic: false);
            var pageSymbol = pageSymbolMock.Object;
            Assert.False(PageRewriter.PageShouldBeProcessed(pageSymbol));
        }

        [Fact]
        public void ProcessPages_Abstract()
        {
            var pageSymbolMock = GetPage(isAbstract: true);
            var pageSymbol = pageSymbolMock.Object;
            Assert.False(PageRewriter.PageShouldBeProcessed(pageSymbol));
        }

        [Fact]
        public void ProcessPages_NoBaseType()
        {
            var pageSymbolMock = GetPage(noBaseClass: true);
            var pageSymbol = pageSymbolMock.Object;
            Assert.False(PageRewriter.PageShouldBeProcessed(pageSymbol));
        }

        [Fact]
        public void ProcessPages_AttributeExcluded()
        {
            var pageSymbolMock = GetPage(attribute: GetExcludedAttribute());
            var pageSymbol = pageSymbolMock.Object;
            Assert.False(PageRewriter.PageShouldBeProcessed(pageSymbol));
        }

        [Fact]
        public void ProcessPages_NestedAttributeExcluded()
        {
            var pageSymbolMock = GetPage(attribute: GetClass("CustomAttribute", GetExcludedAttribute()));
            var pageSymbol = pageSymbolMock.Object;
            Assert.False(PageRewriter.PageShouldBeProcessed(pageSymbol));
        }
    }
}
