using Microsoft.CodeAnalysis;

namespace R4Mvc.Tools.Services
{
    public interface IFilePersistService
    {
        void WriteFile(SyntaxNode fileTree, string filePath);
    }
}
