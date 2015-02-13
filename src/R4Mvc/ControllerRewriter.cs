// ControllerRewriter.cs
namespace R4Mvc
{
	using System.Collections.Generic;
	using System.Diagnostics;

	using Microsoft.AspNet.Mvc;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	/// <summary>
	/// Handles changes to non-generated mvc controller inheriting classes
	/// </summary>
	public class ControllerRewriter : CSharpSyntaxRewriter
	{
		private readonly CSharpCompilation _compiler;

		private readonly List<ClassDeclarationSyntax> _mvcControllerClassNodes = new List<ClassDeclarationSyntax>();

		public ControllerRewriter(CSharpCompilation compiler)
		{
			this._compiler = compiler;
		}

		public ClassDeclarationSyntax[] MvcControllerClassNodes => this._mvcControllerClassNodes.ToArray();

		public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
		{
			var symbol = _compiler.GetSemanticModel(node.SyntaxTree).GetDeclaredSymbol(node);
			if (symbol.InheritsFrom<Controller>())
			{
				// hold a list of all controller classes to use later for the generator
				_mvcControllerClassNodes.Add(node);

				if (!node.Modifiers.Any(SyntaxKind.PartialKeyword))
				{
					// Mark class partial
					Debug.WriteLine("R4MVC - Marking {0} class a partial", symbol);
					node = node.WithPartialModifier();
				}
			}

			return base.VisitClassDeclaration(node);
		}

		public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
		{
			// the symbol wont be found if we've modified class before this point
			// the modification would cause the compiler to run again which would then pick up
			// the symbol second time around. More efficient to manually create a compilation unit for
			// this node first time and make all modifications in one pass
			// TODO how to update out of sync node, new CompilationUnit?

			//var model = _compiler.GetSemanticModel(node.SyntaxTree);
			//var symbol = model.GetDeclaredSymbol(node);
			//if (symbol.InheritsFrom<Controller>() && !symbol.IsVirtual)
			//{
			//	Debug.WriteLine("visiting controller method {0} from {1}", symbol.ToString(), node.SyntaxTree.FilePath);
			//	node.WithVirtualModifier();
			//}

			return base.VisitMethodDeclaration(node);
		}
	}
}