using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace R4Mvc.Tools.Services
{
    public interface IControllerGeneratorService
    {
        string GetControllerArea(INamedTypeSymbol controllerSymbol);
        ClassDeclarationSyntax GeneratePartialController(ControllerDefinition controller);
        ClassDeclarationSyntax GenerateR4Controller(ControllerDefinition controller);
    }
}
