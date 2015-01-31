// ControllerRewriter.cs
namespace R4Mvc
{
	using System.Diagnostics;
	using System.Linq;

	using Microsoft.AspNet.Mvc;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	public class ControllerRewriter : CSharpSyntaxRewriter
	{
		private readonly SemanticModel _model;

		public ControllerRewriter(SemanticModel model)
		{
			_model = model;
		}

		public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
		{
			var symbol = _model.GetDeclaredSymbol(node);
			if (InheritsFrom<Controller>(symbol))
			{
				// if controller is not partial, make it so and create new partial class/file and return
				Debug.WriteLine("{0} inherits from {1}", symbol.ToString(), typeof(Controller).FullName);
			}

            return base.VisitClassDeclaration(node);
		}

		public override SyntaxNode VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
		{
			// first, if !controller class return
			if (InheritsFrom<Controller>(GetClassSymbolFromSyntaxNode(node)))
			{
				var symbol = _model.GetDeclaredSymbol(node);
				Debug.WriteLine("visiting controller constructor {0} from {1}", symbol.ToString(), node.SyntaxTree.FilePath);
				// second, if !generated partial return
				// third, default constructor exists in non-generated class 
				//	if true, remove this constructor
				//	if false, add a default constructor
			}
			return base.VisitConstructorDeclaration(node);
		}

		public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
		{
			// first, if !controller class return
			if (InheritsFrom<Controller>(GetClassSymbolFromSyntaxNode(node)))
			{
				var symbol = _model.GetDeclaredSymbol(node);
				Debug.WriteLine("visiting controller method {0} from {1}", symbol.ToString(), node.SyntaxTree.FilePath);

				if (node.Modifiers.Any(x => x.IsKind(SyntaxKind.PublicKeyword)))
				{
					// only public methods
				}
			}

			return base.VisitMethodDeclaration(node);
		}

		public INamedTypeSymbol GetClassSymbolFromSyntaxNode(SyntaxNode node)
		{
			if (node == null) return null;
			while (true)
			{
				if (node.CSharpKind() == SyntaxKind.ClassDeclaration)
				{
					return _model.GetDeclaredSymbol(node as ClassDeclarationSyntax);
				}
				node = node.Parent;
			}
		}

		private bool InheritsFrom<T>(INamedTypeSymbol symbol)
		{
			while (true)
			{
				if (symbol.ToString() == typeof(T).FullName)
				{
					return true;
				}
				if (symbol.BaseType != null)
				{
					symbol = symbol.BaseType;
					continue;
				}
				break;
			}
			return false;
		}
	}
}