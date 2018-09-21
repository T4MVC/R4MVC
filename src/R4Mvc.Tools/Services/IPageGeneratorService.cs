using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using R4Mvc.Tools.CodeGen;

namespace R4Mvc.Tools.Services
{
    public interface IPageGeneratorService
    {
        ClassDeclarationSyntax GeneratePartialPage(PageDefinition page, bool supportsPages);
        ClassDeclarationSyntax GenerateR4Page(PageDefinition page);
        ClassBuilder WithViewsClass(ClassBuilder classBuilder, IEnumerable<PageView> viewFiles);
    }
}
