using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace R4Mvc.Services
{
    public interface IControllerRewriterService
    {
        void RewriteControllers(IEnumerable<ClassDeclarationSyntax> controllers);
    }
}