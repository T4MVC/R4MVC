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
		private readonly SemanticModel _model;

		private readonly List<ClassDeclarationSyntax> _mvcControllerClassNodes = new List<ClassDeclarationSyntax>();

		public ControllerRewriter(SemanticModel model)
		{
			_model = model;
		}

		public ClassDeclarationSyntax[] MvcControllerClassNodes => this._mvcControllerClassNodes.ToArray();

		public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
		{
			var symbol = _model.GetDeclaredSymbol(node);
			if (symbol.InheritsFrom<Controller>())
			{
				_mvcControllerClassNodes.Add(node);

				// if controller is not partial, make it so and create new partial class/file and return
				Debug.WriteLine("{0} inherits from {1}", symbol.ToString(), typeof(Controller).FullName);
				if (!node.Modifiers.Any(SyntaxKind.PartialKeyword))
				{
					// Mark class partial
					Debug.WriteLine("Marking {0} class a partial", symbol);
					node = R4MvcHelpers.AddPartialModifier(node);
				}
			}

			return base.VisitClassDeclaration(node);
		}

		public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
		{
			// first, if !controller class return
			//var symbol = _model.GetDeclaredSymbol(node);
			//if (symbol.InheritsFrom<Controller>() && symbol.IsVirtual)
			//{
			//	// NOTE Cannot use original semantic model after node has been modified
			//	//var symbol = _model.GetDeclaredSymbol(node);
			//	//Debug.WriteLine("visiting controller method {0} from {1}", symbol.ToString(), node.SyntaxTree.FilePath);

			//	if (node.Modifiers.Any(x => x.IsKind(SyntaxKind.PublicKeyword)))
			//	{
			//		// only public methods

			//		// generated class = true
			//		// is method virtual
			//		//    if true, find corresponding method name + arg count in controller class
			//		//    if false, remove method

			//		// generated class = false
			//		// is method virtual
			//		//   if true, return
			//		//   if false, mark as virtual
			//	}
			//}

			return base.VisitMethodDeclaration(node);
		}
	}
}