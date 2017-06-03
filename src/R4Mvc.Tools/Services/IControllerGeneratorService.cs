using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace R4Mvc.Tools.Services
{
    public interface IControllerGeneratorService
    {
        IEnumerable<NamespaceDeclarationSyntax> GenerateControllers(CSharpCompilation compiler, IEnumerable<ClassDeclarationSyntax> controllerNodes, ref ClassDeclarationSyntax mvcStaticClass);
    }
}
