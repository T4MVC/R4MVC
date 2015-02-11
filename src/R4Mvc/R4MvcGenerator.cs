using System;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using Microsoft.Framework.Runtime;
using Microsoft.Framework.Runtime.Roslyn;

namespace R4Mvc
{
    public class R4MvcGenerator
    {
        public static void Generate(IBeforeCompileContext context)
        {
            var compilationUnit = CompilationUnit();

            NameSyntax name = GetRootNamespace(context);
            name = name.WithLeadingTrivia(Space).WithTrailingTrivia(ElasticCarriageReturn);

            var mvcClass = ClassDeclaration(Identifier(TriviaList(Space), "MVC", TriviaList(ElasticCarriageReturn, ElasticTab)));
            mvcClass = mvcClass.WithModifiers(TokenList(Token(TriviaList(ElasticCarriageReturn, ElasticTab), SyntaxKind.PublicKeyword, TriviaList(Space))));
            mvcClass = mvcClass.WithTrailingTrivia(ElasticCarriageReturn);

            var @namespace = NamespaceDeclaration(name).WithLeadingTrivia(Comment("// Generated code"), ElasticCarriageReturn, ElasticCarriageReturn)
                                                       .AddMembers(mvcClass);

            compilationUnit = compilationUnit.AddMembers(@namespace);

            var tree = SyntaxTree(compilationUnit, path: GetGeneratedCsPath(context));

            context.CSharpCompilation.AddSyntaxTrees(tree);
            
#if !ASPNETCORE50
            // TODO: Fix writing out generated files in both frameworks
            File.WriteAllText(tree.FilePath, tree.ToString());
#endif
        }

        private static NameSyntax GetRootNamespace(IBeforeCompileContext context)
        {
            // TODO: Get this from the complication object
            return ParseName("R4MvcHostApp");
        }

        private static string GetGeneratedCsPath(IBeforeCompileContext context)
        {
            var project = ((CompilationContext)(context)).Project;

            return Path.Combine(project.ProjectDirectory, "R4Mvc.generated.cs");
        }
    }
}