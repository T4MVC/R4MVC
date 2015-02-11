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
				_mvcControllerClassNodes.Add(node);

				// if controller is not partial, make it so
				Debug.WriteLine("{0} inherits from {1}", symbol.ToString(), typeof(Controller).FullName);
				if (!node.Modifiers.Any(SyntaxKind.PartialKeyword))
				{
					// Mark class partial
					Debug.WriteLine("Marking {0} class a partial", symbol);
					node = node.WithPartialModifier();
				}
			}

			return base.VisitClassDeclaration(node);
		}

		public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
		{
			// symbol wont be found if we've modified class
			// TODO how to update out of sync node?
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