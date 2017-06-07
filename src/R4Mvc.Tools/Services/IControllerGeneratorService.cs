using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace R4Mvc.Tools.Services
{
    public interface IControllerGeneratorService
    {
        string GetControllerArea(INamedTypeSymbol controllerSymbol);
        ClassDeclarationSyntax GeneratePartialController(INamedTypeSymbol controllerSymbol, string areaKey, string areaName, string controllerName, string projectRoot);
        ClassDeclarationSyntax GenerateR4Controller(INamedTypeSymbol controllerSymbol);
    }
}
