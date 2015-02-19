using Microsoft.CodeAnalysis;

namespace R4Mvc.Services
{
    public interface IControllerGeneratorService
    {
        SyntaxNode GenerateControllers(object controllers);
    }
}