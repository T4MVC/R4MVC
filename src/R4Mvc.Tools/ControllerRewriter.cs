using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using R4Mvc.Tools.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace R4Mvc.Tools
{
    /// <summary>
    /// Handles changes to non-generated mvc controller inheriting classes
    /// </summary>
    public class ControllerRewriter : CSharpSyntaxRewriter
    {
        private readonly CSharpCompilation _compiler;

        private readonly List<ClassDeclarationSyntax> _mvcControllerClassNodes = new List<ClassDeclarationSyntax>();

        public ControllerRewriter(CSharpCompilation compiler)
        {
            _compiler = compiler;
        }

        public ClassDeclarationSyntax[] MvcControllerClassNodes => this._mvcControllerClassNodes.ToArray();

        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            // grab the symbol first and pass to other visitors first
            var symbol = _compiler.GetSemanticModel(node.SyntaxTree).GetDeclaredSymbol(node);
            var newNode = (ClassDeclarationSyntax)base.VisitClassDeclaration(node);
            if (ControllerShouldBeProcessed(symbol))
            {
                Console.WriteLine($"Processing controller {symbol.ContainingNamespace}.{symbol.Name}");

                // hold a list of all controller classes to use later for the generator
                _mvcControllerClassNodes.Add(node);

                if (!newNode.Modifiers.Any(SyntaxKind.PartialKeyword))
                {
                    Debug.WriteLine("R4MVC - Marking {0} class a partial", symbol);
                    newNode = newNode.WithModifiers(SyntaxKind.PartialKeyword);
                }
            }

            return newNode;
        }

        internal static bool ControllerShouldBeProcessed(INamedTypeSymbol symbol)
            => symbol.DeclaredAccessibility == Accessibility.Public &&
                !symbol.IsAbstract &&
                symbol.InheritsFrom<ControllerBase>() &&
                !symbol.GetAttributes().Any(a => a.AttributeClass.InheritsFrom<R4MvcExcludeAttribute>());

        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            node = (MethodDeclarationSyntax)base.VisitMethodDeclaration(node);

            // only public methods not marked as virtual
            if (node.Modifiers.Any(SyntaxKind.PublicKeyword) && !node.Modifiers.Any(m => m.IsKind(SyntaxKind.VirtualKeyword) || m.IsKind(SyntaxKind.OverrideKeyword)))
            {
                var symbol = _compiler.GetSemanticModel(node.SyntaxTree).GetDeclaredSymbol(node);
                if (ControllerShouldBeProcessed(symbol.ContainingType) && symbol.IsMvcAction() && symbol.IsNotR4MvcExcluded())
                {
                    Debug.WriteLine(
                        "R4MVC - Marking controller method {0} as virtual from {1}",
                        symbol.ToString(),
                        symbol.ContainingType?.ToString());
                    node = node.WithModifiers(SyntaxKind.VirtualKeyword);
                }
            }

            return node;
        }
    }
}
