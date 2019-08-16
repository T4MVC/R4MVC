﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace R4Mvc.Tools.CodeGen
{
    public class CodeFileBuilder
    {
        private static readonly string[] _pragmaCodes = { "1591", "3008", "3009", "0108" };
        private const string _headerText =
@"// <auto-generated />
// This file was generated by R4Mvc.
// Don't change it directly as your change would get overwritten.  Instead, make changes
// to the r4mvc.json file (i.e. the settings file), save it and run the generator tool again.

// Make sure the compiler doesn't complain about missing Xml comments and CLS compliance
// 0108: suppress ""Foo hides inherited member Foo.Use the new keyword if hiding was intended."" when a controller and its abstract parent are both processed";


        private CompilationUnitSyntax _compilationUnit;
        private readonly bool _autoGenerated;

        public CodeFileBuilder(Settings settings, bool autoGenerated)
        {
            _autoGenerated = autoGenerated;
            var usings = new List<string>
            {
                "System",
                "System.CodeDom.Compiler",
                "System.Diagnostics",
                "System.Threading.Tasks",
                "Microsoft.AspNetCore.Mvc",
                "Microsoft.AspNetCore.Routing",
                settings.R4MvcNamespace,
            };
            if (settings.ReferencedNamespaces != null)
                usings.AddRange(settings.ReferencedNamespaces);
            _compilationUnit = CompilationUnit()
                .WithUsings(List(usings.Select(u => UsingDirective(ParseName(u)))));

            if (_autoGenerated)
            {
                var headTrivia = _compilationUnit.GetLeadingTrivia()
                    .Add(Comment(_headerText))
                    .Add(CarriageReturnLineFeed)
                    .Add(GetPragmaCodes(SyntaxKind.DisableKeyword));
                _compilationUnit = _compilationUnit.WithLeadingTrivia(headTrivia);
            }
        }

        private SyntaxTrivia GetPragmaCodes(SyntaxKind syntaxKind)
            => Trivia(
                PragmaWarningDirectiveTrivia(
                    Token(syntaxKind),
                    SeparatedList(_pragmaCodes.Select(p => ParseExpression(p))),
                    true));

        public CodeFileBuilder WithNamespace(NamespaceDeclarationSyntax @namespace)
        {
            _compilationUnit = _compilationUnit.AddMembers(@namespace);
            return this;
        }

        public CodeFileBuilder WithNamespaces(IEnumerable<NamespaceDeclarationSyntax> namespaces)
        {
            foreach (var ns in namespaces)
                _compilationUnit = _compilationUnit.AddMembers(ns);
            return this;
        }

        public CodeFileBuilder WithMembers(params MemberDeclarationSyntax[] members)
        {
            _compilationUnit = _compilationUnit.AddMembers(members);
            return this;
        }

        public CodeFileBuilder WithMembers(bool condition, params MemberDeclarationSyntax[] members)
        {
            if (condition)
                return WithMembers(members);
            return this;
        }

        public CompilationUnitSyntax Build()
        {
            if (_autoGenerated)
            {
                var endTrivia = _compilationUnit.GetTrailingTrivia()
                    .Add(ElasticCarriageReturnLineFeed)
                    .Add(GetPragmaCodes(SyntaxKind.RestoreKeyword));
                _compilationUnit = _compilationUnit.WithTrailingTrivia(endTrivia);
            }

            return _compilationUnit
                .NormalizeWhitespace();
        }
    }
}
