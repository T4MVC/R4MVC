using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace R4Mvc.Tools.Services
{
    public interface IControllerGeneratorService
    {
        ClassDeclarationSyntax GeneratePartialController(INamedTypeSymbol controllerSymbol, string areaName, string controllerName);
        ClassDeclarationSyntax GenerateR4Controller(INamedTypeSymbol controllerSymbol);
    }
}
