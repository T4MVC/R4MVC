namespace R4Mvc
{
	using System.IO;
	using System.Linq;

	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	public static class R4MvcHelpers
	{
		public static bool InheritsFrom<T>(this ITypeSymbol symbol)
		{
			while (true)
			{
				if (symbol.TypeKind == TypeKind.Class && symbol.ToString() == typeof(T).FullName)
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

		public static bool InheritsFrom<T>(this IMethodSymbol symbol)
		{
			return symbol.ContainingType.InheritsFrom<T>();
		}

		public static NamespaceDeclarationSyntax CreateNamespace(string namespaceText)
		{
			var nameSyntax = SyntaxFactory.ParseName(namespaceText);
			var declaration = SyntaxFactory.NamespaceDeclaration(nameSyntax);
			return declaration;
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

		public static NamespaceDeclarationSyntax WithPragmaCodes(this NamespaceDeclarationSyntax node, bool enable, params int[] codes)
		{
			// TODO Add prama warning enable/disable [codes]
			return node;
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

		public static NamespaceDeclarationSyntax WithHeader(this NamespaceDeclarationSyntax node, string headerText)
		{
			return node.WithLeadingTrivia(SyntaxFactory.Comment(headerText), SyntaxFactory.CarriageReturnLineFeed);
		}

		//private static AttributeListSyntax[] CreateGeneratedAttribute()
		//{
		//	//var argumentList = SyntaxFactory.ParseAttributeArgumentList("[GeneratedCode(\"R4MVC\", \"1.0.0.0\")]");
		//	//var attribute = SyntaxFactory.AttributeList(SyntaxFactory.Attribute(SyntaxFactory.IdentifierName("GeneratedCode"), argumentList);
		//	//return SyntaxFactory.AttributeList(attribute);
		//}

		public static void WriteFile(this SyntaxNode fileTree, string generatedFilePath)
		{
			using (var textWriter = new StreamWriter(new FileStream(generatedFilePath, FileMode.Create)))
			{
				fileTree.NormalizeWhitespace().WriteTo(textWriter);
			}
		}

		public static ClassDeclarationSyntax AddPartialModifier(ClassDeclarationSyntax node)
		{
			var syntaxToken = R4MvcHelpers.CreatePartialToken();
			var newNode = node.AddModifiers(syntaxToken);
			node = newNode;
			return node;
		}

		public const string R4MvcFileName = "R4MVC.generated.cs";
	}
}