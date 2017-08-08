using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace R4Mvc.Tools.Services
{
    public interface IStaticFileGeneratorService
    {
        MemberDeclarationSyntax GenerateStaticFiles(string projectRoot);
    }
}
