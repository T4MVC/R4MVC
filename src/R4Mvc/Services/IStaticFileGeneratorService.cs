
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace R4Mvc.Services
{
	public interface IStaticFileGeneratorService
	{
		MemberDeclarationSyntax GenerateStaticFiles(ISettings settings);
	}
}