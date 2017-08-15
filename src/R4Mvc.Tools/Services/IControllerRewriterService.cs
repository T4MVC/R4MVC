using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;

namespace R4Mvc.Tools.Services
{
    public interface IControllerRewriterService
    {
        IReadOnlyCollection<ControllerDefinition> RewriteControllers(CSharpCompilation compiler);
    }
}
