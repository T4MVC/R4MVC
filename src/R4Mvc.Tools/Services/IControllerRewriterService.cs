using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace R4Mvc.Tools.Services
{
	public interface IControllerRewriterService
	{
		ImmutableArray<ClassDeclarationSyntax> RewriteControllers(CSharpCompilation compiler, string outputFileName);
	}
}
