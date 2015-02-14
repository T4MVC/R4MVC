using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace R4Mvc
{
	/// <summary>
	/// A collection of helper and fluent extension methods to help manipulate SyntaxNodes
	/// </summary>
	public static class SyntaxHelpers
	{
		public const string R4MvcFileName = "R4MVC.generated.cs";

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

		public static SyntaxToken CreateVirtualToken()
		{
			return SyntaxFactory.Token(
				SyntaxFactory.TriviaList(),
				SyntaxKind.VirtualKeyword,
				SyntaxFactory.TriviaList(SyntaxFactory.Space));
		}

		private static AttributeListSyntax[] CreateGeneratedAttribute()
		{
			// TODO Figure out how to construct attribute args in the SyntaxNode
			var argumentList = SyntaxFactory.ParseAttributeArgumentList("(tool: \"R4MVC\", version: \"1.0.0.0\")");
			//var attributeNode = SyntaxFactory.Attribute(SyntaxFactory.IdentifierName("GeneratedCode"), argumentList);
			//var attribute = SyntaxFactory.List<AttributeListSyntax>(attributeNode);
			//return SyntaxFactory.AttributeList(attribute);
			return null;
		}

		private static ConstructorDeclarationSyntax CreateDefaultConstructor(ISymbol mvcSymbol)
		{
			return
				SyntaxFactory.ConstructorDeclaration(mvcSymbol.Name)
					.AddModifiers(CreatePublicToken())
					.WithBody(SyntaxFactory.Block());
		}

		public static IEnumerable<MemberDeclarationSyntax> CreateMethods(this ITypeSymbol mvcSymbol)
		{
			foreach (
				var mvcControllerMethod in
					mvcSymbol.GetMembers()
						.OfType<IMethodSymbol>()
						.Where(x => x.DeclaredAccessibility == Accessibility.Public && x.MethodKind == MethodKind.Ordinary))
			{
				yield return CreateMethod(mvcControllerMethod);
			}
		}

		private static MemberDeclarationSyntax CreateMethod(IMethodSymbol methodSymbol)
		{
			// TODO decide whether to output full qualified name of return types to avoid issues with add usings
			// TODO add return type typeparameters
			// TODO add method body, currently returns null
			var returnType = SyntaxFactory.ParseTypeName(methodSymbol.ReturnType.Name);
			var methodNode =
				SyntaxFactory.MethodDeclaration(returnType, methodSymbol.Name)
					.AddModifiers(CreatePublicToken())
					.WithBody(
						SyntaxFactory.Block(
							SyntaxFactory.SingletonList<StatementSyntax>(
								SyntaxFactory.ReturnStatement(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)))));

			return methodNode.WithGeneratedAttributes();
		}

		public static T WithPragmaCodes<T>(this T node, bool enable, params int[] codes) where T : SyntaxNode
		{
			// BUG prama is not put on newline with normalizewhitespace as expected
			var trivia = enable ? node.GetTrailingTrivia() : node.GetLeadingTrivia();
			var pramaStatus = enable ? SyntaxKind.RestoreKeyword : SyntaxKind.DisableKeyword;
			var pramaExpressions = codes.Select(x => SyntaxFactory.ParseExpression(x.ToString())).ToArray();
			var prama =
				trivia.Add(
					SyntaxFactory.Trivia(
						SyntaxFactory.PragmaWarningDirectiveTrivia(SyntaxFactory.Token(pramaStatus), true).AddErrorCodes(pramaExpressions)))
					.NormalizeWhitespace();

			return enable ? node.WithTrailingTrivia(prama) : node.WithLeadingTrivia(prama);
		}

		public static CompilationUnitSyntax WithUsings(this CompilationUnitSyntax node, params string[] namespaces)
		{
			var collection = namespaces.Select(ns => SyntaxFactory.ParseName(ns)).Select(SyntaxFactory.UsingDirective);
			var usings = SyntaxFactory.List(collection);
			return node.WithUsings(usings);
		}

		public static ClassDeclarationSyntax WithClass(this NamespaceDeclarationSyntax node, string className, TypeParameterSyntax[] typeParams)
		{
			var classSyntax = SyntaxFactory.ClassDeclaration(className).WithPublicModifier().WithPartialModifier();

			if(typeParams != null)
				classSyntax = classSyntax
					.AddTypeParameterListParameters(typeParams);

			return classSyntax;
		}

		public static T WithHeader<T>(this T node, string headerText) where T : SyntaxNode
		{
			var leadingTrivia =
				node.GetLeadingTrivia()
				.Add(SyntaxFactory.Comment(headerText))
				.Add(SyntaxFactory.CarriageReturnLineFeed);
			return node.WithLeadingTrivia(leadingTrivia);
		}

		public static MemberDeclarationSyntax WithGeneratedAttributes(this MemberDeclarationSyntax node)
		{
			var attributes = CreateGeneratedAttribute();
			return node;
			// TODO fix adding attributes for gencode and debugskip
			//return node.AddAttributeLists(attributes);
		}

		public static ClassDeclarationSyntax WithDefaultConstructor(this ClassDeclarationSyntax node, ITypeSymbol mvcSymbol)
		{
			return node.AddMembers(CreateDefaultConstructor(mvcSymbol));
		}

		public static ClassDeclarationSyntax WithMethods(this ClassDeclarationSyntax node, ITypeSymbol mvcSymbol)
		{
			return node;
			// TODO fix member generation
			return node.AddMembers(mvcSymbol.CreateMethods().ToArray());
		}

		public static ClassDeclarationSyntax WithActionNameClass(this ClassDeclarationSyntax node, ITypeSymbol mvcSymbol)
		{
			// TODO create ActionNames sub class using symbol method names
			return node;
		}

		public static ClassDeclarationSyntax WithActionConstantsClass(this ClassDeclarationSyntax node, ITypeSymbol mvcSymbol)
		{
			// TODO create ActionConstants sub class
			return node;
		}

		public static ClassDeclarationSyntax WithPartialModifier(this ClassDeclarationSyntax node)
		{
			return node.AddModifiers(CreatePartialToken());
		}

		public static ClassDeclarationSyntax WithPublicModifier(this ClassDeclarationSyntax node)
		{
			return node.AddModifiers(CreatePublicToken());
		}

		public static MethodDeclarationSyntax WithVirtualModifier(this MethodDeclarationSyntax node)
		{
			return node.AddModifiers(CreateVirtualToken());
		}

		public static void WriteFile(this SyntaxNode fileTree, string generatedFilePath, bool resetWhitespace)
		{
			using (var textWriter = new StreamWriter(new FileStream(generatedFilePath, FileMode.Create)))
			{
				if (resetWhitespace)
				{
					fileTree = fileTree.NormalizeWhitespace();
				}
				fileTree.WriteTo(textWriter);
			}
		}
	}
}