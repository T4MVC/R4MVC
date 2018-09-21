using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;

namespace R4Mvc.Tools.Services
{
    public interface IPageRewriterService
    {
        IList<ControllerDefinition> RewritePages(CSharpCompilation compiler);
    }
}
