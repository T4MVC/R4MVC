using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using static R4Mvc.Tools.Extensions.SyntaxNodeHelpers;
using static System.String;

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
                .GroupBy(v => v.TemplateKind);
            if (allControllerViews.Count() == 0)
                return node;

            // create subclass called ViewsClass
            // create ViewNames get property returning static instance of _ViewNamesClass subclass
            //	create subclass in ViewsClass called _ViewNamesClass 
            //		create string field per view
            const string viewNamesClass = "_ViewNamesClass";
            var viewClassNode =
                CreateClass("ViewsClass", null, SyntaxKind.PublicKeyword)
                    .WithAttributes(CreateGeneratedCodeAttribute(), CreateDebugNonUserCodeAttribute())
                    .WithField("s_ViewNames", viewNamesClass, SyntaxKind.StaticKeyword, SyntaxKind.ReadOnlyKeyword);

            var viewNamesClassNode = CreateClass(viewNamesClass, null, SyntaxKind.PublicKeyword);
            var controllerViews = allControllerViews.Where(x => string.IsNullOrEmpty(x.Key)).SelectMany(x => x).ToImmutableArray();
            var viewNameFields =
                controllerViews.Select(
                    x => CreateStringFieldDeclaration(x.ViewName, x.ViewName, SyntaxKind.PublicKeyword, SyntaxKind.ReadOnlyKeyword))
                    .Cast<MemberDeclarationSyntax>()
                    .ToArray();
            viewNamesClassNode = viewNamesClassNode.AddMembers(viewNameFields);

            viewClassNode = viewClassNode.AddMembers(viewNamesClassNode);
            var viewFields =
                controllerViews.Select(
                    x => CreateStringFieldDeclaration(x.ViewName, x.RelativePath.ToString(), SyntaxKind.PublicKeyword))
                    .Cast<MemberDeclarationSyntax>()
                    .ToArray();
            viewClassNode = viewClassNode.AddMembers(viewFields);

            foreach (var templateKind in allControllerViews.Where(x => !string.IsNullOrEmpty(x.Key)))
            {
                var className = $"_{templateKind.Key}Class";
                var templateClass = CreateClass(className, null, SyntaxKind.PublicKeyword, SyntaxKind.PartialKeyword)
                    .WithAttributes(CreateGeneratedCodeAttribute(), CreateDebugNonUserCodeAttribute());
                templateClass = templateClass.AddMembers(
                    templateKind.Select(t => CreateStringFieldDeclaration(t.ViewName, t.ViewName, SyntaxKind.PublicKeyword, SyntaxKind.ReadOnlyKeyword)).ToArray());

                node = node
                    .WithField("s_" + templateKind.Key, className, SyntaxKind.StaticKeyword, SyntaxKind.ReadOnlyKeyword)
                    .WithProperty(templateKind.Key, className, SyntaxFactory.IdentifierName("s_" + templateKind.Key), SyntaxKind.PublicKeyword)
                    .AddMembers(templateClass);
            }

            return node.AddMembers(viewClassNode);
        }

        public static ClassDeclarationSyntax WithStaticFieldsForFiles(this ClassDeclarationSyntax node, IEnumerable<StaticFile> staticFiles)
        {
            var staticNodes =
                staticFiles.Select(
                    x => CreateStringFieldDeclaration(x.FileName, x.RelativePath.ToString(), SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword))
                    .Cast<MemberDeclarationSyntax>()
                    .ToArray();
            return node.AddMembers(staticNodes);
        }

        public static ClassDeclarationSyntax WithUrlMethods(this ClassDeclarationSyntax node)
        {
            // TODO add url methods that call delegated virtual path provider
            return node;
        }

        public static NamespaceDeclarationSyntax WithDummyClass(this NamespaceDeclarationSyntax node)
        {
            var dummyClass =
                CreateClass(Constants.DummyClass)
                    .WithModifiers(SyntaxKind.PublicKeyword)
                    .WithAttributes(CreateGeneratedCodeAttribute(), CreateDebugNonUserCodeAttribute())
                    .WithDefaultConstructor(false, SyntaxKind.PrivateKeyword)
                    .WithField(Constants.DummyClassInstance, Constants.DummyClass, SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword);

            return node.AddMembers(dummyClass);
        }

        public static string Replace(this string s, char[] separators, string newVal)
        {
            var temp = s.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            return Join(newVal, temp);
        }
    }
}
