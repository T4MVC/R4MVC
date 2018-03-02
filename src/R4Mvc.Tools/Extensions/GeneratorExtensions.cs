using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using R4Mvc.Tools.CodeGen;

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
            /* [GeneratedCode, DebuggerNonUserCode]
             * public class ViewsClass
             * {
             * [...] */
            var viewsClass = new ClassBuilder("ViewsClass")
                .WithModifiers(SyntaxKind.PublicKeyword)
                .WithGeneratedNonUserCodeAttributes()
                // static readonly _ViewNamesClass s_ViewNames = new _ViewNamesClass();
                // public _ViewNamesClass ViewNames => s_ViewNames;
                .WithStaticFieldBackedProperty("ViewNames", ViewNamesClassName, SyntaxKind.PublicKeyword)
                /* public class _ViewNamesClass
                 * {
                 *  public readonly string {view} = "{view}";
                 * }
                 */
                .WithChildClass(ViewNamesClassName, vnc => vnc
                    .WithModifiers(SyntaxKind.PublicKeyword)
                    .ForEach(viewFiles.Where(c => c.TemplateKind == null), (vc, v) => vc
                        .WithStringField(v.ViewName.SanitiseFieldName(), v.ViewName, false, SyntaxKind.PublicKeyword, SyntaxKind.ReadOnlyKeyword)))
                .ForEach(viewFiles.Where(c => c.TemplateKind == null), (c, v) => c
                    // public readonly string {view} = "~/Views/{controller}/{view}.cshtml";
                    .WithStringField(v.ViewName.SanitiseFieldName(), v.RelativePath.ToString(), false, SyntaxKind.PublicKeyword, SyntaxKind.ReadOnlyKeyword))
                .ForEach(viewEditorTemplates.GroupBy(v => v.TemplateKind), (c, g) => c
                    // static readonly _DisplayTemplatesClass s_DisplayTemplates = new _DisplayTemplatesClass();
                    // public _DisplayTemplatesClass DisplayTemplates => s_DisplayTemplates;
                    .WithStaticFieldBackedProperty(g.Key, $"_{g.Key}Class", SyntaxKind.PublicKeyword)
                    /* [GeneratedCode, DebuggerNonUserCode]
                     * public partial _DisplayTemplatesClass
                     * {
                     *  public readonly string {view} = "{view}";
                     * }
                     */
                    .WithChildClass($"_{g.Key}Class", tc => tc
                        .WithModifiers(SyntaxKind.PublicKeyword, SyntaxKind.PartialKeyword)
                        .WithGeneratedNonUserCodeAttributes()
                        .ForEach(g, (tcc, v) => tcc
                            .WithStringField(v.ViewName.SanitiseFieldName(), v.ViewName, false, SyntaxKind.PublicKeyword, SyntaxKind.ReadOnlyKeyword))))
                .ForEach(subpathViews.GroupBy(v => v.TemplateKind), (c, g) => c
                    // static readonly _{viewFolder}Class s_{viewFolder} = new _{viewFolder}Class();
                    // public _{viewFolder}Class {viewFolder} => s_{viewFolder};
                    .WithStaticFieldBackedProperty(g.Key, $"_{g.Key}Class", SyntaxKind.PublicKeyword)
                    /* [GeneratedCode, DebuggerNonUserCode]
                     * public class _{viewFolder}Class
                     * {
                     * [...] */
                    .WithChildClass($"_{g.Key}Class", tc => tc
                        .WithModifiers(SyntaxKind.PublicKeyword, SyntaxKind.PartialKeyword)
                        .WithGeneratedNonUserCodeAttributes()
                        // static readonly _ViewNamesClass s_ViewNames = new _ViewNamesClass();
                        // public _ViewNamesClass ViewNames => s_ViewNames;
                        .WithStaticFieldBackedProperty("ViewNames", ViewNamesClassName, SyntaxKind.PublicKeyword)
                        /* public class _ViewNamesClass
                         * {
                         *  public readonly string {view} = "{view}";
                         * }
                         */
                        .WithChildClass(ViewNamesClassName, vnc => vnc
                            .WithModifiers(SyntaxKind.PublicKeyword)
                            .ForEach(g, (vc, v) => vc
                                .WithStringField(v.ViewName.SanitiseFieldName(), v.ViewName, false, SyntaxKind.PublicKeyword, SyntaxKind.ReadOnlyKeyword)))
                        .ForEach(g, (vc, v) => vc
                            // public string {view} = "~/Views/{controller}/{viewFolder}/{view}.cshtml";
                            .WithStringField(v.ViewName.SanitiseFieldName(), v.RelativePath.ToString(), false, SyntaxKind.PublicKeyword, SyntaxKind.ReadOnlyKeyword))));

            return classBuilder
                .WithMember(viewsClass.Build())
                .WithStaticFieldBackedProperty("Views", viewsClass.Name, SyntaxKind.PublicKeyword);
        }
    }
}
