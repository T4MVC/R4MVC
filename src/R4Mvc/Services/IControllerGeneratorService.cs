using System.Collections.Generic;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace R4Mvc.Services
{
	public interface IControllerGeneratorService
	{
		IEnumerable<NamespaceDeclarationSyntax> GenerateControllers(CSharpCompilation compiler, IEnumerable<ClassDeclarationSyntax> controllers);
	}
}