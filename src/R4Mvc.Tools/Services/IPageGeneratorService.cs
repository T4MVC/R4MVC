using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using R4Mvc.Tools.CodeGen;

namespace R4Mvc.Tools.Services
{
    public interface IPageGeneratorService
    {
        ClassDeclarationSyntax GeneratePartialPage(PageView pageView);
        ClassDeclarationSyntax GenerateR4Page(PageDefinition page);
        ClassBuilder WithViewsClass(ClassBuilder classBuilder, IEnumerable<PageView> viewFiles);
        void AddR4ActionMethods(ClassBuilder genControllerClass, string pagePath);
    }
}
