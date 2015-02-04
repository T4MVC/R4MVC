// ControllerRewriter.cs
namespace R4Mvc
{
	using System.CodeDom.Compiler;
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
					Debug.WriteLine("Marking {0} class a partial", symbol);
					var syntaxToken = Helpers.CreatePartialToken();
					var newNode = node.AddModifiers(syntaxToken);
					node = newNode;
				}

				if (!string.IsNullOrWhiteSpace(node.SyntaxTree.FilePath))
				{
					// does generated file exist?
					var references = symbol.DeclaringSyntaxReferences;
					if (references.Length == 1)
					{
						// Create new generate file/partial class for this class
						var fileName = symbol.Name;
						if (symbol.IsGenericType)
						{
							fileName += "`" + symbol.TypeParameters.Length;
						}

						var generatedFilePath = GetGeneratedFilePath(Path.GetDirectoryName(node.SyntaxTree.FilePath), fileName);

						var fileTree = Helpers.CreateNamespace(symbol.ContainingNamespace.ToString());
						fileTree = fileTree.WithUsings(new[] { "System.CodeDom.Compiler" });
						fileTree = fileTree.WithClass(symbol.Name, node.TypeParameterList?.Parameters.ToArray());
						fileTree.SyntaxTree.WithFilePath(generatedFilePath);
						fileTree = fileTree.NormalizeWhitespace();
						
						using (var textWriter = new StreamWriter(generatedFilePath))
						{
							fileTree.WriteTo(textWriter);
						}					
					}
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
		
		private string GetGeneratedFilePath(string directory, string name)
		{
			var path = Path.Combine(directory, "R4MVC");
			if (directory != null && !Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}

			var fileName =  string.Format("{0}.R4MVC.generated.cs", name);
			return Path.Combine(path, fileName);
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

	public static class Helpers
	{
		public static NamespaceDeclarationSyntax CreateNamespace(string namespaceText)
		{
			var nameSyntax = SyntaxFactory.ParseName(namespaceText);
			var declaration = SyntaxFactory.NamespaceDeclaration(nameSyntax);
			return declaration;
		}

		public static SyntaxNode CreateClass(this SyntaxNode node, string className)
		{
			return node;
		}

		public static SyntaxToken CreatePartialToken()
		{
			return SyntaxFactory.Token(
				SyntaxFactory.TriviaList(),
				SyntaxKind.PartialKeyword,
				SyntaxFactory.TriviaList(SyntaxFactory.Space));
		}
		public static SyntaxToken CreatePublicToken()
		{
			return SyntaxFactory.Token(
				SyntaxFactory.TriviaList(),
				SyntaxKind.PublicKeyword,
				SyntaxFactory.TriviaList(SyntaxFactory.Space));
		}

		public static NamespaceDeclarationSyntax WithUsings(this NamespaceDeclarationSyntax node, string[] namespaces)
		{
			var collection = namespaces.Select(ns => SyntaxFactory.ParseName(ns)).Select(SyntaxFactory.UsingDirective);
			var usings = SyntaxFactory.List(collection);
			return node.WithUsings(usings);
		}

		public static NamespaceDeclarationSyntax WithClass(this NamespaceDeclarationSyntax node, string className, TypeParameterSyntax[] typeParams)
		{
			var classSyntax =
				SyntaxFactory.ClassDeclaration(className)
					.AddModifiers(CreatePublicToken())
					.AddModifiers(CreatePartialToken());
					//.AddAttributeLists(CreateGeneratedAttribute());

			if(typeParams != null)
				classSyntax = classSyntax
					.AddTypeParameterListParameters(typeParams);

			node = node.AddMembers(classSyntax);
			return node;
		}

		//private static AttributeListSyntax[] CreateGeneratedAttribute()
		//{
		//	//var argumentList = SyntaxFactory.ParseAttributeArgumentList("[GeneratedCode(\"R4MVC\", \"1.0.0.0\")]");
		//	//var attribute = SyntaxFactory.AttributeList(SyntaxFactory.Attribute(SyntaxFactory.IdentifierName("GeneratedCode"), argumentList);
		//	//return SyntaxFactory.AttributeList(attribute);
		//}
	}
}