using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using R4Mvc.Tools.CodeGen;
using static R4Mvc.Tools.Extensions.SyntaxNodeHelpers;

namespace R4Mvc.Tools.Extensions
{
    public static class GeneratorExtensions
    {
        private const string ViewNamesClassName = "_ViewNamesClass";

        public static ClassBuilder WithViewsClass(this ClassBuilder classBuilder, IEnumerable<View> viewFiles)
        {
            var viewEditorTemplates = viewFiles.Where(c => c.TemplateKind == "EditorTemplates" || c.TemplateKind == "DisplayTemplates");
            var subpathViews = viewFiles.Where(c => c.TemplateKind != null && c.TemplateKind != "EditorTemplates" && c.TemplateKind != "DisplayTemplates")
                .OrderBy(v => v.TemplateKind);
            var viewsClass = new ClassBuilder("ViewsClass")
                .WithModifiers(SyntaxKind.PublicKeyword)
                .WithGeneratedNonUserCodeAttributes()
                .WithStaticFieldBackedProperty("ViewNames", ViewNamesClassName, SyntaxKind.PublicKeyword)
                .WithChildClass(ViewNamesClassName, vnc => vnc
                    .WithModifiers(SyntaxKind.PublicKeyword)
                    .ForMany(viewFiles.Where(c => c.TemplateKind == null), (vc, v) => vc
                        .WithStringField(v.ViewName.SanitiseFieldName(), v.ViewName, false, SyntaxKind.PublicKeyword, SyntaxKind.ReadOnlyKeyword)))
                .ForMany(viewFiles.Where(c => c.TemplateKind == null), (c, v) => c
                    .WithStringField(v.ViewName.SanitiseFieldName(), v.RelativePath.ToString(), false, SyntaxKind.PublicKeyword))
                .ForMany(viewEditorTemplates.GroupBy(v => v.TemplateKind), (c, g) => c
                    .WithChildClass($"_{g.Key}Class", tc => tc
                        .WithModifiers(SyntaxKind.PublicKeyword, SyntaxKind.PartialKeyword)
                        .WithGeneratedNonUserCodeAttributes()
                        .ForMany(g, (tcc, v) => tcc
                            .WithStringField(v.ViewName.SanitiseFieldName(), v.ViewName, false, SyntaxKind.PublicKeyword, SyntaxKind.ReadOnlyKeyword)))
                    .WithStaticFieldBackedProperty(g.Key, $"_{g.Key}Class", SyntaxKind.PublicKeyword))
                .ForMany(subpathViews.GroupBy(v => v.TemplateKind), (c, g) => c
                    .WithChildClass($"_{g.Key}Class", tc => tc
                        .WithModifiers(SyntaxKind.PublicKeyword, SyntaxKind.PartialKeyword)
                        .WithGeneratedNonUserCodeAttributes()
                        .WithChildClass(ViewNamesClassName, vnc => vnc
                            .WithModifiers(SyntaxKind.PublicKeyword)
                            .ForMany(g, (vc, v) => vc
                                .WithStringField(v.ViewName.SanitiseFieldName(), v.ViewName, false, SyntaxKind.PublicKeyword, SyntaxKind.ReadOnlyKeyword)))
                        .WithStaticFieldBackedProperty("ViewNames", ViewNamesClassName, SyntaxKind.PublicKeyword)
                        .ForMany(g, (vc, v) => vc
                            .WithStringField(v.ViewName.SanitiseFieldName(), v.RelativePath.ToString(), false, SyntaxKind.PublicKeyword, SyntaxKind.ReadOnlyKeyword))
                                )
                    .WithStaticFieldBackedProperty(g.Key, $"_{g.Key}Class", SyntaxKind.PublicKeyword));

            return classBuilder
                .WithMember(viewsClass.Build())
                .WithStaticFieldBackedProperty("Views", viewsClass.Name, SyntaxKind.PublicKeyword);
        }

        public static NamespaceDeclarationSyntax WithDummyClass(this NamespaceDeclarationSyntax node)
        {
            var dummyClass = new ClassBuilder(Constants.DummyClass)
                .WithModifiers(SyntaxKind.PublicKeyword)
                .WithGeneratedNonUserCodeAttributes()
                .WithConstructor(c => c
                    .WithModifiers(SyntaxKind.PrivateKeyword))
                .WithField(Constants.DummyClassInstance, Constants.DummyClass, SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword);

            return node.AddMembers(dummyClass.Build());
        }
    }
}
