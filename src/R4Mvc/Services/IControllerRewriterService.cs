using System.Collections.Generic;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace R4Mvc.Services
{
	public interface IControllerRewriterService
	{
		IEnumerable<ClassDeclarationSyntax> RewriteControllers(CSharpCompilation compiler, string outputFileName);
	}
}