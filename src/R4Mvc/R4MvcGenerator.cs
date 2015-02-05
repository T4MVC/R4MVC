using System;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace R4Mvc
{
    public class R4MvcGenerator
    {
        public static void Generate(CSharpCompilation complication)
        {
            var compilationUnit = CompilationUnit();

            NameSyntax name = GetRootNamespace(complication);
            name = name.WithLeadingTrivia(Space).WithTrailingTrivia(ElasticCarriageReturn);

            var mvcClass = ClassDeclaration(Identifier(TriviaList(Space), "MVC", TriviaList(ElasticCarriageReturn, ElasticTab)));
            mvcClass = mvcClass.WithModifiers(TokenList(Token(TriviaList(ElasticCarriageReturn, ElasticTab), SyntaxKind.PublicKeyword, TriviaList(Space))));
            mvcClass = mvcClass.WithTrailingTrivia(ElasticCarriageReturn);

            var @namespace = NamespaceDeclaration(name).WithLeadingTrivia(Comment("// Generated code"), ElasticCarriageReturn, ElasticCarriageReturn)
                                                       .AddMembers(mvcClass);

            compilationUnit = compilationUnit.AddMembers(@namespace);

            var tree = SyntaxTree(compilationUnit, path: GetGeneratedCsPath(complication));

            complication.AddSyntaxTrees(tree);
            
#if !ASPNETCORE50
            // TODO: Fix writing out generated files in both frameworks
            File.WriteAllText(tree.FilePath, tree.ToString());
#endif
        }

        private static NameSyntax GetRootNamespace(CSharpCompilation complication)
        {
            // TODO: Get this from the complication object
            return ParseName("R4MvcHostApp");
        }

        private static string GetGeneratedCsPath(CSharpCompilation complication)
        {
            // TODO: Get this from the complication object
            return @"C:\Projects\R4MVC\src\R4MvcHostApp\R4Mvc.generated.cs";
        }
    }
}