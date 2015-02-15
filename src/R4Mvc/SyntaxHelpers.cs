using System;
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

		private static SyntaxToken CreateModifier(SyntaxKind kind)
		{
			return SyntaxFactory.Token(
				SyntaxFactory.TriviaList(),
				kind,
				SyntaxFactory.TriviaList(SyntaxFactory.Space));
		}

		public static SyntaxToken[] CreateModifiers(params SyntaxKind[] kinds)
		{
			return kinds.Select(CreateModifier).ToArray();
		}

		public static SyntaxToken CreatePublicToken()
		{
			return SyntaxFactory.Token(
				SyntaxFactory.TriviaList(),
				SyntaxKind.PublicKeyword,
				SyntaxFactory.TriviaList(SyntaxFactory.Space));
		}

		private static SyntaxToken CreatePrivateToken()
		{
			return SyntaxFactory.Token(
				SyntaxFactory.TriviaList(),
				SyntaxKind.PrivateKeyword,
				SyntaxFactory.TriviaList(SyntaxFactory.Space));
		}

		private static SyntaxToken CreateStaticToken()
		{
			return SyntaxFactory.Token(
				SyntaxFactory.TriviaList(),
				SyntaxKind.StaticKeyword,
				SyntaxFactory.TriviaList(SyntaxFactory.Space));
		}

		public static SyntaxToken CreateVirtualToken()
		{
			return SyntaxFactory.Token(
				SyntaxFactory.TriviaList(),
				SyntaxKind.VirtualKeyword,
				SyntaxFactory.TriviaList(SyntaxFactory.Space));
		}

		public static ClassDeclarationSyntax CreateClass(string className, TypeParameterSyntax[] typeParams = null, params SyntaxKind[] modifiers)
		{
			var classSyntax = SyntaxFactory.ClassDeclaration(className).WithModifiers(modifiers);

			if (typeParams != null)
				classSyntax = classSyntax
					.AddTypeParameterListParameters(typeParams);

			return classSyntax;
		}

		public static AttributeSyntax CreateDebugNonUserCodeAttribute()
		{
			return SyntaxFactory.Attribute(SyntaxFactory.IdentifierName(@"DebuggerNonUserCode"));
		}

		public static AttributeSyntax CreateNonActionAttribute()
		{
			return SyntaxFactory.Attribute(SyntaxFactory.IdentifierName(@"NonAction"));
		}

		public static AttributeSyntax CreateGeneratedCodeAttribute()
		{
			var arguments =
				SyntaxFactory.AttributeArgumentList(
					SyntaxFactory.SeparatedList(
						new[]
							{
								SyntaxFactory.AttributeArgument(
									SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal("R4MVC"))),
									SyntaxFactory.AttributeArgument(
									SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal("1.0")))
							}));
			return SyntaxFactory.Attribute(SyntaxFactory.IdentifierName("GeneratedCode"), arguments);
		}

		private static ConstructorDeclarationSyntax CreateDefaultConstructor(string className)
		{
			return
				SyntaxFactory.ConstructorDeclaration(className)
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
			var returnType = methodSymbol.ReturnType;
			//var typeParameters = returnType.ContainingType.TypeParameters;

			var returnTypeSyntax = SyntaxFactory.ParseTypeName(returnType.ToDisplayString());
			
			var methodNode =
				SyntaxFactory.MethodDeclaration(returnTypeSyntax, methodSymbol.Name)
					.WithAttributes(CreateGeneratedCodeAttribute(), CreateDebugNonUserCodeAttribute(), CreateNonActionAttribute())
					.WithModifiers(SyntaxKind.PublicKeyword, SyntaxKind.VirtualKeyword)
					.WithBody(
						SyntaxFactory.Block(
							SyntaxFactory.SingletonList<StatementSyntax>(
								SyntaxFactory.ReturnStatement(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)))));
			return methodNode;
		}

		private static FieldDeclarationSyntax CreateStringFieldDeclaration(string fieldName, string fieldValue, params SyntaxKind[] modifiers)
		{
			return
				SyntaxFactory.FieldDeclaration(
					SyntaxFactory.VariableDeclaration(
						SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword)),
						SyntaxFactory.SingletonSeparatedList(
							SyntaxFactory.VariableDeclarator(fieldName)
								.WithInitializer(
									SyntaxFactory.EqualsValueClause(
										SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(fieldValue)))))))
					.WithModifiers(modifiers);
		}

		public static MethodDeclarationSyntax WithAttributes(this MethodDeclarationSyntax node, params AttributeSyntax[] attributes)
		{
			return node.AddAttributeLists(SyntaxFactory.AttributeList(SyntaxFactory.SeparatedList(attributes)));
		}

		public static ConstructorDeclarationSyntax WithAttributes(this ConstructorDeclarationSyntax node, params AttributeSyntax[] attributes)
		{
			return node.AddAttributeLists(SyntaxFactory.AttributeList(SyntaxFactory.SeparatedList(attributes)));
		}

		public static ClassDeclarationSyntax WithAttributes(this ClassDeclarationSyntax node, params AttributeSyntax[] attributes)
		{
			return node.AddAttributeLists(SyntaxFactory.AttributeList(SyntaxFactory.SeparatedList(attributes)));
		}

		public static FieldDeclarationSyntax WithAttributes(this FieldDeclarationSyntax node, params AttributeSyntax[] attributes)
		{
			return node.AddAttributeLists(SyntaxFactory.AttributeList(SyntaxFactory.SeparatedList(attributes)));
		}

		public static T WithPragmaCodes<T>(this T node, bool enable, params int[] codes) where T : SyntaxNode
		{
			// BUG prama is not put on newline with normalizewhitespace as expected
			var trivia = enable ? node.GetTrailingTrivia() : node.GetLeadingTrivia();
			var pramaStatus = enable ? SyntaxKind.RestoreKeyword : SyntaxKind.DisableKeyword;
			var pramaExpressions = SyntaxFactory.SeparatedList(codes.Select(x => SyntaxFactory.ParseExpression(x.ToString())));
			var prama =
				trivia.Add(SyntaxFactory.ElasticCarriageReturnLineFeed)
					.Add(
						SyntaxFactory.Trivia(
							SyntaxFactory.PragmaWarningDirectiveTrivia(SyntaxFactory.Token(pramaStatus), pramaExpressions, true)
								.NormalizeWhitespace()))
					.Add(SyntaxFactory.ElasticCarriageReturnLineFeed);

			return enable ? node.WithTrailingTrivia(prama) : node.WithLeadingTrivia(prama);
		}

		public static CompilationUnitSyntax WithUsings(this CompilationUnitSyntax node, params string[] namespaces)
		{
			var collection = namespaces.Select(ns => SyntaxFactory.ParseName(ns)).Select(SyntaxFactory.UsingDirective);
			var usings = SyntaxFactory.List(collection);
			return node.WithUsings(usings);
		}

		public static T WithHeader<T>(this T node, string headerText) where T : SyntaxNode
		{
			var leadingTrivia =
				node.GetLeadingTrivia()
				.Add(SyntaxFactory.Comment(headerText))
				.Add(SyntaxFactory.CarriageReturnLineFeed);
			return node.WithLeadingTrivia(leadingTrivia);
		}

		public static ClassDeclarationSyntax WithDefaultConstructor(this ClassDeclarationSyntax node, string className, params SyntaxKind[] modifiers)
		{
			return
				node.AddMembers(CreateDefaultConstructor(className).WithModifiers(modifiers).WithAttributes(CreateGeneratedCodeAttribute(), CreateDebugNonUserCodeAttribute()));
		}

		public static ClassDeclarationSyntax WithMethods(this ClassDeclarationSyntax node, ITypeSymbol mvcSymbol)
		{
			return node;
			// TODO fix member generation
			return node.AddMembers(mvcSymbol.CreateMethods().ToArray());
		}

		public static ClassDeclarationSyntax WithActionNameClass(this ClassDeclarationSyntax node, ClassDeclarationSyntax controllerNode)
		{
			// create ActionNames sub class using symbol method names
			return node.WithSubClassMembersAsStrings(
				controllerNode,
				"ActionNamesClass",
				SyntaxKind.PublicKeyword,
				SyntaxKind.ReadOnlyKeyword);
		}

		public static ClassDeclarationSyntax WithActionConstantsClass(this ClassDeclarationSyntax node, ClassDeclarationSyntax controllerNode)
		{
			// create ActionConstants sub class
			return node.WithSubClassMembersAsStrings(
				controllerNode,
				"ActionNameConstants",
				SyntaxKind.PublicKeyword,
				SyntaxKind.ConstKeyword);
		}

		public static ClassDeclarationSyntax WithSubClassMembersAsStrings(this ClassDeclarationSyntax node, ClassDeclarationSyntax controllerNode, string className, params SyntaxKind[] modifiers)
		{
			// create ActionConstants sub class
			var actionNameClass = CreateClass(className, null, SyntaxKind.PublicKeyword).WithAttributes(CreateGeneratedCodeAttribute());
			foreach (var action in controllerNode.Members.OfType<MethodDeclarationSyntax>().Where(x => x.Modifiers.Any(SyntaxKind.PublicKeyword)).DistinctBy(x => x.Identifier.ToString()))
			{
				actionNameClass = actionNameClass.WithStringField(action.Identifier.ToString(), action.Identifier.ToString(), false, modifiers);
			}
			return node.AddMembers(actionNameClass);
		}

		public static NamespaceDeclarationSyntax WithDummyClass(this NamespaceDeclarationSyntax node)
		{
			const string dummyClassName = "Dummy";
			var dummyClass =
				CreateClass(dummyClassName)
					.WithModifiers(SyntaxKind.PublicKeyword)
					.WithAttributes(CreateGeneratedCodeAttribute(), CreateDebugNonUserCodeAttribute())
					.WithDefaultConstructor(dummyClassName, SyntaxKind.PrivateKeyword)
					.WithField("Instance", dummyClassName, SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword);

			return node.AddMembers(dummyClass);
		}

		public static ClassDeclarationSyntax WithField(this ClassDeclarationSyntax node, string fieldName, string typeName, params SyntaxKind[] modifiers)
		{
			var field =
				SyntaxFactory.FieldDeclaration(
					SyntaxFactory.VariableDeclaration(SyntaxFactory.ParseTypeName(typeName))
						.WithVariables(
							SyntaxFactory.SingletonSeparatedList(
								SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier(fieldName))
									.WithInitializer(
										SyntaxFactory.EqualsValueClause(
											SyntaxFactory.ObjectCreationExpression(SyntaxFactory.IdentifierName(typeName))
												.WithArgumentList(SyntaxFactory.ArgumentList())))))).WithModifiers(modifiers);
			return node.AddMembers(field);
		}

		public static ClassDeclarationSyntax WithStringField(this ClassDeclarationSyntax node, string name, string value, bool includeGeneratedAttribute = true, params SyntaxKind[] modifiers)
		{
			var fieldDeclaration = CreateStringFieldDeclaration(name, value, modifiers);
			if (includeGeneratedAttribute) fieldDeclaration = fieldDeclaration.WithAttributes(CreateGeneratedCodeAttribute());
			return node.AddMembers(fieldDeclaration);
		}

		public static ClassDeclarationSyntax WithModifiers(this ClassDeclarationSyntax node, params SyntaxKind[] modifiers)
		{
			return node.AddModifiers(CreateModifiers(modifiers));
		}

		public static ConstructorDeclarationSyntax WithModifiers(this ConstructorDeclarationSyntax node, params SyntaxKind[] modifiers)
		{
			return node.AddModifiers(CreateModifiers(modifiers));
		}

		public static MethodDeclarationSyntax WithModifiers(this MethodDeclarationSyntax node, params SyntaxKind[] modifiers)
		{
			return node.AddModifiers(CreateModifiers(modifiers));
		}

		public static FieldDeclarationSyntax WithModifiers(this FieldDeclarationSyntax node, params SyntaxKind[] modifiers)
		{
			return node.AddModifiers(CreateModifiers(modifiers));
		}

		public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			var seenKeys = new HashSet<TKey>();
			return source.Where(element => seenKeys.Add(keySelector(element)));
		}

		public static void WriteFile(this SyntaxNode fileTree, string generatedFilePath)
		{
			using (var textWriter = new StreamWriter(new FileStream(generatedFilePath, FileMode.Create)))
			{
				fileTree.WriteTo(textWriter);
			}
		}
	}
}