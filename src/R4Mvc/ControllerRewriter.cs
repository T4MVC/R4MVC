// ControllerRewriter.cs
namespace R4Mvc
{
	using System.Diagnostics;
	using System.IO;
	using System.Linq;

	using Microsoft.AspNet.Mvc;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	public class ControllerRewriter : CSharpSyntaxRewriter
	{
		private readonly SemanticModel _model;

		private bool _inheritsController;

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
				if (!node.Modifiers.Any(SyntaxKind.PartialKeyword))
				{
					// Mark class partial
					Debug.WriteLine("Marking {1} class a partial", symbol.ToString());
					var syntaxToken = SyntaxFactory.Token(
						SyntaxFactory.TriviaList(),
						SyntaxKind.PartialKeyword,
						SyntaxFactory.TriviaList(SyntaxFactory.Space));
					var newNode = node.AddModifiers(syntaxToken);
					
					// Create new generate file/partial class for this class
					// TODO should put generated files in own folder
					var generatedText = string.Format("namespace {0} {{ public partial class {1} {{}} }}", symbol.ContainingNamespace.ToString(), symbol.Name);
					var generatedFilePath = System.Text.RegularExpressions.Regex.Replace(node.SyntaxTree.FilePath, ".cs$", ".R4MVC.generated.cs");
					var generatedTree = CSharpSyntaxTree.ParseText(generatedText);
					
					// TODO how to have reliable updated semanticmodel to pass to visitor for newly generate file
					// var generatedAfterVisit = new ControllerRewriter(_model).Visit(generatedTree.GetRoot());
					File.WriteAllText(generatedFilePath, generatedTree.GetCompilationUnitRoot().GetText().ToString());

					node = newNode;
				}
			}

			return base.VisitClassDeclaration(node);
		}

		public override SyntaxNode VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
		{
			// first, if !controller class return
			if (_inheritsController)
			{
				// NOTE Cannot use original semantic model after node has been modified
				//var symbol = _model.GetDeclaredSymbol(node);
				//Debug.WriteLine("visiting controller constructor {0} from {1}", symbol.ToString(), node.SyntaxTree.FilePath);
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
			if (_inheritsController)
			{
				// NOTE Cannot use original semantic model after node has been modified
				//var symbol = _model.GetDeclaredSymbol(node);
				//Debug.WriteLine("visiting controller method {0} from {1}", symbol.ToString(), node.SyntaxTree.FilePath);

				if (node.Modifiers.Any(x => x.IsKind(SyntaxKind.PublicKeyword)))
				{
					// only public methods

					// generated class = true
					// is method virtual
					//    if true, find corresponding method name + arg count in controller class
					//    if false, remove method

					// generated class = false
					// is method virtual
					//   if true, return
					//   if false, mark as virtual
				}
			}

			return base.VisitMethodDeclaration(node);
		}

		private bool InheritsFrom<T>(INamedTypeSymbol symbol)
		{
			while (true)
			{
				if (symbol.ToString() == typeof(T).FullName)
				{
					_inheritsController = true;
					return _inheritsController;
				}
				if (symbol.BaseType != null)
				{
					symbol = symbol.BaseType;
					continue;
				}
				break;
			}
			return _inheritsController;
		}
	}
}