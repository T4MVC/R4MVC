using Microsoft.CodeAnalysis;

namespace R4Mvc.Services
{
    public interface IStaticFileGeneratorService
    {
        SyntaxNode GerateStaticFiles(SyntaxNode syntaxNode);
    }
}