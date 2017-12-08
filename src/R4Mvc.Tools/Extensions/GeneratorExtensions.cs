using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using static R4Mvc.Tools.Extensions.SyntaxNodeHelpers;

namespace R4Mvc.Tools.Extensions
{
    public static class GeneratorExtensions
    {
        public static ClassDeclarationSyntax WithActionNameClass(
            this ClassDeclarationSyntax node,
            ITypeSymbol controllerClass)
        {
            // create ActionNames sub class using symbol method names
            return node.WithSubClassMembersAsStrings(
                controllerClass,
                "ActionNamesClass",
                SyntaxKind.PublicKeyword,
                SyntaxKind.ReadOnlyKeyword);
        }

        public static ClassDeclarationSyntax WithActionConstantsClass(
            this ClassDeclarationSyntax node,
            ITypeSymbol controllerClass)
        {
            // create ActionConstants sub class
            return node.WithSubClassMembersAsStrings(
                controllerClass,
                "ActionNameConstants",
                SyntaxKind.PublicKeyword,
                SyntaxKind.ConstKeyword);
        }

        public static ClassDeclarationSyntax WithViewsClass(this ClassDeclarationSyntax node, string controllerName, string areaName, IEnumerable<View> viewFiles)
        {
            var allControllerViews = viewFiles
                .Where(x => string.Equals(x.ControllerName, controllerName, StringComparison.CurrentCultureIgnoreCase) && string.Equals(x.AreaName, areaName, StringComparison.OrdinalIgnoreCase))
                .GroupBy(v => v.TemplateKind)
                .ToList();

            // create subclass called ViewsClass
            // create ViewNames get property returning static instance of _ViewNamesClass subclass
            //	create subclass in ViewsClass called _ViewNamesClass 
            //		create string field per view
            const string viewNamesClass = "_ViewNamesClass";
            var viewClassNode = SyntaxFactory.ClassDeclaration("ViewsClass")
                .WithModifiers(SyntaxKind.PublicKeyword)
                .WithGeneratedNonUserCodeAttributes()
                .WithStaticFieldBackedProperty("ViewNames", viewNamesClass, SyntaxKind.PublicKeyword);

            var viewNamesClassNode = SyntaxFactory.ClassDeclaration(viewNamesClass).WithModifiers(SyntaxKind.PublicKeyword);
            var controllerViews = allControllerViews.Where(x => string.IsNullOrEmpty(x.Key)).SelectMany(x => x).ToImmutableArray();
            var viewNameFields =
                controllerViews.Select(
                    x => CreateStringFieldDeclaration(x.ViewName.SanitiseFieldName(), x.ViewName, SyntaxKind.PublicKeyword, SyntaxKind.ReadOnlyKeyword))
                    .ToArray<MemberDeclarationSyntax>();
            viewNamesClassNode = viewNamesClassNode.AddMembers(viewNameFields);

            viewClassNode = viewClassNode.AddMembers(viewNamesClassNode);
            var viewFields =
                controllerViews.Select(
                    x => CreateStringFieldDeclaration(x.ViewName.SanitiseFieldName(), x.RelativePath.ToString(), SyntaxKind.PublicKeyword))
                    .ToArray<MemberDeclarationSyntax>();
            viewClassNode = viewClassNode.AddMembers(viewFields);

            foreach (var templateKind in allControllerViews.Where(x => !string.IsNullOrEmpty(x.Key)))
            {
                viewClassNode = viewClassNode
                    .WithSubViewsClass(controllerName, areaName, templateKind, templateKind.Key);
            }

            return node
                .AddMembers(viewClassNode)
                .WithStaticFieldBackedProperty("Views", "ViewsClass", SyntaxKind.PublicKeyword);
        }

        public static ClassDeclarationSyntax WithSubViewsClass(this ClassDeclarationSyntax node, string controllerName, string areaName, IEnumerable<View> viewFiles, string templateKind = null)
        {
            const string viewNamesClass = "_ViewNamesClass";
            
            var viewNamesClassNode = SyntaxFactory.ClassDeclaration(viewNamesClass).WithModifiers(SyntaxKind.PublicKeyword);
            var controllerViews = viewFiles.ToImmutableArray();
            var viewNameFields =
                controllerViews.Select(
                    x => CreateStringFieldDeclaration(x.ViewName, x.ViewName, SyntaxKind.PublicKeyword, SyntaxKind.ReadOnlyKeyword))
                    .ToArray<MemberDeclarationSyntax>();
            viewNamesClassNode = viewNamesClassNode.AddMembers(viewNameFields);
            
            var className = $"_{templateKind}Class";
            var templateClass = SyntaxFactory.ClassDeclaration(className)
                .WithModifiers(SyntaxKind.PublicKeyword, SyntaxKind.PartialKeyword)
                .WithGeneratedNonUserCodeAttributes()
                .WithStaticFieldBackedProperty("ViewNames", viewNamesClass, SyntaxKind.PublicKeyword);

            templateClass = templateClass.AddMembers(viewNamesClassNode);
            var isAspNetTemplateDirectory = IsAspNetTemplateDirectory(templateKind);
            var viewFields =
                controllerViews.Select(
                        x => CreateStringFieldDeclaration(x.ViewName, isAspNetTemplateDirectory ? x.ViewName : x.RelativePath.ToString(), SyntaxKind.PublicKeyword))
                    .ToArray<MemberDeclarationSyntax>();
            templateClass = templateClass.AddMembers(viewFields);

            node = node
                .WithStaticFieldBackedProperty(templateKind, className, SyntaxKind.PublicKeyword)
                .AddMembers(templateClass);

            return node;
        }

        private static bool IsAspNetTemplateDirectory(string templateKind)
        {
            return templateKind == "DisplayTemplates" || templateKind == "EditorTemplates";
        }

        public static ClassDeclarationSyntax WithStaticFieldsForFiles(this ClassDeclarationSyntax node, IEnumerable<StaticFile> staticFiles)
        {
            var staticNodes =
                staticFiles.Select(
                    x => CreateStringFieldDeclaration(x.FileName, x.RelativePath.ToString(), SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword))
                    .ToArray<MemberDeclarationSyntax>();
            return node.AddMembers(staticNodes);
        }

        public static ClassDeclarationSyntax WithUrlMethods(this ClassDeclarationSyntax node)
        {
            // TODO add url methods that call delegated virtual path provider
            return node;
        }

        public static NamespaceDeclarationSyntax WithDummyClass(this NamespaceDeclarationSyntax node)
        {
            var dummyClass = SyntaxFactory.ClassDeclaration(Constants.DummyClass)
                    .WithModifiers(SyntaxKind.PublicKeyword)
                    .WithGeneratedNonUserCodeAttributes()
                    .WithDefaultConstructor(false, SyntaxKind.PrivateKeyword)
                    .WithField(Constants.DummyClassInstance, Constants.DummyClass, SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword);

            return node.AddMembers(dummyClass);
        }
    }
}
