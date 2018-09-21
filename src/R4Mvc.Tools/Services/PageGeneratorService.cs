using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using R4Mvc.Tools.CodeGen;
using R4Mvc.Tools.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static R4Mvc.Tools.Extensions.SyntaxNodeHelpers;

namespace R4Mvc.Tools.Services
{
    public class PageGeneratorService : IPageGeneratorService
    {
        private const string ViewNamesClassName = "_ViewNamesClass";

        private readonly Settings _settings;

        public PageGeneratorService(Settings settings)
        {
            _settings = settings;
        }

        public ClassDeclarationSyntax GeneratePartialPage(PageDefinition page)
        {
            // build controller partial class node
            var genControllerClass = new ClassBuilder(page.Symbol.Name)               // public partial {controllerClass}
                .WithModifiers(SyntaxKind.PublicKeyword, SyntaxKind.PartialKeyword)
                .WithTypeParameters(page.Symbol.TypeParameters.Select(tp => tp.Name).ToArray()); // optional <T1, T2, …>

            // add a default constructor if there are some but none are zero length
            var gotCustomConstructors = page.Symbol.Constructors
                .Where(c => c.DeclaredAccessibility == Accessibility.Public)
                .Where(SyntaxNodeHelpers.IsNotR4MVCGenerated)
                .Where(c => !c.IsImplicitlyDeclared)
                .Any();
            if (!gotCustomConstructors)
                /* [GeneratedCode, DebuggerNonUserCode]
                 * public ctor() { }
                 */
                genControllerClass.WithConstructor(c => c
                    .WithModifiers(SyntaxKind.PublicKeyword)
                    .WithGeneratedNonUserCodeAttributes());
            /* [GeneratedCode, DebuggerNonUserCode]
             * public ctor(Dummy d) {}
             */
            genControllerClass.WithConstructor(c => c
                .WithModifiers(SyntaxKind.ProtectedKeyword)
                .WithGeneratedNonUserCodeAttributes()
                .WithParameter("d", Constants.DummyClass));

            AddRedirectMethods(genControllerClass);
            AddParameterlessMethods(genControllerClass, page.Symbol, page.IsSecure);

            //var actionsExpression = _settings.HelpersPrefix + "." + page.Name;
            //var controllerMethodNames = SyntaxNodeHelpers.GetPublicNonGeneratedMethods(page.Symbol).Select(m => m.Name).Distinct().ToArray();
            genControllerClass
                //.WithExpressionProperty("Actions", page.Symbol.Name, actionsExpression, SyntaxKind.PublicKeyword)
                .WithStringField("Name", page.Name, true, SyntaxKind.PublicKeyword, SyntaxKind.ReadOnlyKeyword)
                .WithStringField("NameConst", page.Name, true, SyntaxKind.PublicKeyword, SyntaxKind.ConstKeyword);
                //.WithStaticFieldBackedProperty("ActionNames", "ActionNamesClass", true, SyntaxKind.PublicKeyword)
                ///* [GeneratedCode, DebuggerNonUserCode]
                // * public class ActionNamesClass
                // * {
                // *  public readonly string {action} = "{action}";
                // * }
                // */
                //.WithChildClass("ActionNamesClass", ac => ac
                //    .WithModifiers(SyntaxKind.PublicKeyword)
                //    .WithGeneratedNonUserCodeAttributes()
                //    .ForEach(controllerMethodNames, (c, m) => c
                //        .WithStringField(m, m, false, SyntaxKind.PublicKeyword, SyntaxKind.ReadOnlyKeyword)))
                ///* [GeneratedCode, DebuggerNonUserCode]
                // * public class ActionNameConstants
                // * {
                // *  public const string {action} = "{action}";
                // * }
                // */
                //.WithChildClass("ActionNameConstants", ac => ac
                //    .WithModifiers(SyntaxKind.PublicKeyword)
                //    .WithGeneratedNonUserCodeAttributes()
                //    .ForEach(controllerMethodNames, (c, m) => c
                //        .WithStringField(m, m, false, SyntaxKind.PublicKeyword, SyntaxKind.ConstKeyword)));
            WithViewsClass(genControllerClass, page.Views);

            return genControllerClass.Build();
        }

        public ClassDeclarationSyntax GenerateR4Page(PageDefinition page)
        {
            var className = GetR4MVCControllerClassName(page.Symbol);
            page.FullyQualifiedR4ClassName = $"{page.Namespace}.{className}";

            /* [GeneratedCode, DebuggerNonUserCode]
             * public partial class R4MVC_{Controller} : {Controller}
             * {
             *  public R4MVC_{Controller}() : base(Dummy.Instance) {}
             * }
             */
            var r4ControllerClass = new ClassBuilder(className)
                .WithModifiers(SyntaxKind.PublicKeyword, SyntaxKind.PartialKeyword)
                .WithGeneratedNonUserCodeAttributes()
                .WithBaseTypes(page.Symbol.ContainingNamespace + "." + page.Symbol.Name)
                .WithConstructor(c => c
                    .WithBaseConstructorCall(IdentifierName(Constants.DummyClass + "." + Constants.DummyClassInstance))
                    .WithModifiers(SyntaxKind.PublicKeyword));
            AddMethodOverrides(r4ControllerClass, page.Symbol, page.IsSecure);
            return r4ControllerClass.Build();
        }

        private void AddRedirectMethods(ClassBuilder genControllerClass)
        {
            genControllerClass
                /* [GeneratedCode, DebuggerNonUserCode]
                 * protected RedirectToRouteResult RedirectToAction(IActionResult result)
                 * {
                 *  var callInfo = result.GetR4MvcResult();
                 *  return RedirectToRoute(callInfo.RouteValueDictionary);
                 * }
                 */
                .WithMethod("RedirectToAction", "RedirectToRouteResult", m => m
                    .WithModifiers(SyntaxKind.ProtectedKeyword)
                    .WithGeneratedNonUserCodeAttributes()
                    .WithParameter("result", "IActionResult")
                    .WithBody(b => b
                        .VariableFromMethodCall("callInfo", "result", "GetR4MvcResult")
                        .ReturnMethodCall(null, "RedirectToRoute", "callInfo.RouteValueDictionary")))

                /* [GeneratedCode, DebuggerNonUserCode]
                 * protected RedirectToRouteResult RedirectToAction(Task<IActionResult> taskResult)
                 * {
                 *  return RedirectToAction(taskResult.Result);
                 * }
                */
                .WithMethod("RedirectToAction", "RedirectToRouteResult", m => m
                    .WithModifiers(SyntaxKind.ProtectedKeyword)
                    .WithGeneratedNonUserCodeAttributes()
                    .WithParameter("taskResult", "Task<IActionResult>")
                    .WithBody(b => b
                        .ReturnMethodCall(null, "RedirectToAction", "taskResult.Result")))

                /* [GeneratedCode, DebuggerNonUserCode]
                 * protected RedirectToRouteResult RedirectToActionPermanent(IActionResult result)
                 * {
                 *  var callInfo = result.GetR4MvcResult();
                 *  return RedirectToRoutePermanent(callInfo.RouteValueDictionary);
                 * }
                 */
                .WithMethod("RedirectToActionPermanent", "RedirectToRouteResult", m => m
                    .WithModifiers(SyntaxKind.ProtectedKeyword)
                    .WithGeneratedNonUserCodeAttributes()
                    .WithParameter("result", "IActionResult")
                    .WithBody(b => b
                        .VariableFromMethodCall("callInfo", "result", "GetR4MvcResult")
                        .ReturnMethodCall(null, "RedirectToRoutePermanent", "callInfo.RouteValueDictionary")))

                /* [GeneratedCode, DebuggerNonUserCode]
                 * protected RedirectToRouteResult RedirectToActionPermanent(Task<IActionResult> taskResult)
                 * {
                 *  return RedirectToActionPermanent(taskResult.Result);
                 * }
                */
                .WithMethod("RedirectToActionPermanent", "RedirectToRouteResult", m => m
                    .WithModifiers(SyntaxKind.ProtectedKeyword)
                    .WithGeneratedNonUserCodeAttributes()
                    .WithParameter("taskResult", "Task<IActionResult>")
                    .WithBody(b => b
                        .ReturnMethodCall(null, "RedirectToActionPermanent", "taskResult.Result")));
        }

        private void AddParameterlessMethods(ClassBuilder genControllerClass, ITypeSymbol mvcSymbol, bool isControllerSecure)
        {
            var methods = mvcSymbol.GetPublicNonGeneratedMethods()
                .GroupBy(m => m.Name)
                .Where(g => !g.Any(m => m.Parameters.Length == 0));
            foreach (var method in methods)
                genControllerClass
                    /* [GeneratedCode, DebuggerNonUserCode]
                     * public virtual IActionResult {method.Key}()
                     * {
                     *  return new R4Mvc_Microsoft_AspNetCore_Mvc_ActionResult(Area, Name, ActionNames.{Action});
                     * }
                     */
                    .WithMethod(method.Key, "IActionResult", m => m
                        .WithModifiers(SyntaxKind.PublicKeyword, SyntaxKind.VirtualKeyword)
                        .WithNonActionAttribute()
                        .WithGeneratedNonUserCodeAttributes()
                        .WithBody(b => b
                            .ReturnNewObject(Constants.ActionResultClass,
                                isControllerSecure || method.Any(mg => mg.GetAttributes().Any(a => a.AttributeClass.InheritsFrom<RequireHttpsAttribute>()))
                                    ? new object[] { "Area", "Name", "ActionNames." + method.Key, SimpleLiteral.String("https") }
                                    : new object[] { "Area", "Name", "ActionNames." + method.Key }
                            )));
        }

        private void AddMethodOverrides(ClassBuilder classBuilder, ITypeSymbol mvcSymbol, bool isControllerSecure)
        {
            const string overrideMethodSuffix = "Override";
            foreach (var method in mvcSymbol.GetPublicNonGeneratedMethods())
            {
                var methodReturnType = method.ReturnType;
                bool isTaskResult = false, isGenericTaskResult = false;
                if (methodReturnType.InheritsFrom<Task>())
                {
                    isTaskResult = true;
                    var taskReturnType = methodReturnType as INamedTypeSymbol;
                    if (taskReturnType.TypeArguments.Length > 0)
                    {
                        methodReturnType = taskReturnType.TypeArguments[0];
                        isGenericTaskResult = true;
                    }
                }

                var callInfoType = Constants.ActionResultClass;
                if (methodReturnType.InheritsFrom<JsonResult>())
                    callInfoType = Constants.JsonResultClass;
                else if (methodReturnType.InheritsFrom<ContentResult>())
                    callInfoType = Constants.ContentResultClass;
                else if (methodReturnType.InheritsFrom<FileResult>())
                    callInfoType = Constants.FileResultClass;
                else if (methodReturnType.InheritsFrom<RedirectResult>())
                    callInfoType = Constants.RedirectResultClass;
                else if (methodReturnType.InheritsFrom<RedirectToActionResult>())
                    callInfoType = Constants.RedirectToActionResultClass;
                else if (methodReturnType.InheritsFrom<RedirectToRouteResult>())
                    callInfoType = Constants.RedirectToRouteResultClass;
                else if (methodReturnType.InheritsFrom<IConvertToActionResult>())
                    callInfoType = Constants.ActionResultClass;
                else if ((!isTaskResult || isGenericTaskResult) && !methodReturnType.InheritsFrom<IActionResult>())
                {
                    // Not a return type we support right now. Returning
                    continue;
                }

                classBuilder
                    /* [NonAction]
                     * partial void {action}Override({ActionResultType} callInfo, [… params]);
                     */
                    .WithMethod(method.Name + overrideMethodSuffix, null, m => m
                        .WithModifiers(SyntaxKind.PartialKeyword)
                        .WithNonActionAttribute()
                        .WithParameter("callInfo", callInfoType)
                        .ForEach(method.Parameters, (m2, p) => m2
                            .WithParameter(p.Name, p.Type.ToString()))
                        .WithNoBody())
                    /* [NonAction]
                     * public overrive {ActionResultType} {action}([… params])
                     * {
                     *  var callInfo = new R4Mvc_Microsoft_AspNetCore_Mvc_ActionResult(Area, Name, ActionNames.{Action});
                     *  ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "paramName", paramName);
                     *  {Action}Override(callInfo, {parameters});
                     *  return callInfo;
                     * }
                     */
                    .WithMethod(method.Name, method.ReturnType.ToString(), m => m
                        .WithModifiers(SyntaxKind.PublicKeyword, SyntaxKind.OverrideKeyword)
                        .WithNonActionAttribute()
                        .ForEach(method.Parameters, (m2, p) => m2
                            .WithParameter(p.Name, p.Type.ToString()))
                        .WithBody(b => b
                            .VariableFromNewObject("callInfo", callInfoType,
                                isControllerSecure || method.GetAttributes().Any(a => a.AttributeClass.InheritsFrom<RequireHttpsAttribute>())
                                    ? new object[] { "Area", "Name", "ActionNames." + method.Name, SimpleLiteral.String("https") }
                                    : new object[] { "Area", "Name", "ActionNames." + method.Name }
                            )
                            .ForEach(method.Parameters, (cb, p) => cb
                                .MethodCall("ModelUnbinderHelpers", "AddRouteValues", "callInfo.RouteValueDictionary", SimpleLiteral.String(p.Name), p.Name))
                            .MethodCall(null, method.Name + overrideMethodSuffix, new[] { "callInfo" }.Concat(method.Parameters.Select(p => p.Name)).ToArray())
                            .Statement(rb => isTaskResult
                                ? rb.ReturnMethodCall(typeof(Task).FullName, "FromResult" + (isGenericTaskResult ? "<" + methodReturnType + ">" : null), "callInfo")
                                : rb.ReturnVariable("callInfo"))
                        ));
            }
        }

        internal static string GetR4MVCControllerClassName(INamedTypeSymbol controllerClass)
        {
            return string.Format("R4MVC_{0}", controllerClass.Name);
        }

        public ClassBuilder WithViewsClass(ClassBuilder classBuilder, IEnumerable<IView> viewFiles)
        {
            var viewEditorTemplates = viewFiles.Where(c => c.TemplateKind == "EditorTemplates" || c.TemplateKind == "DisplayTemplates");
            var subpathViews = viewFiles.Where(c => c.TemplateKind != null && c.TemplateKind != "EditorTemplates" && c.TemplateKind != "DisplayTemplates")
                .OrderBy(v => v.TemplateKind);
            /* public class ViewsClass
             * {
             * [...] */
            var viewsClass = new ClassBuilder("ViewsClass")
                .WithModifiers(SyntaxKind.PublicKeyword)
                // static readonly _ViewNamesClass s_ViewNames = new _ViewNamesClass();
                // public _ViewNamesClass ViewNames => s_ViewNames;
                .WithStaticFieldBackedProperty("ViewNames", ViewNamesClassName, false, SyntaxKind.PublicKeyword)
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
                    .WithStaticFieldBackedProperty(g.Key, $"_{g.Key}Class", false, SyntaxKind.PublicKeyword)
                    /* public partial _DisplayTemplatesClass
                     * {
                     *  public readonly string {view} = "{view}";
                     * }
                     */
                    .WithChildClass($"_{g.Key}Class", tc => tc
                        .WithModifiers(SyntaxKind.PublicKeyword, SyntaxKind.PartialKeyword)
                        .ForEach(g, (tcc, v) => tcc
                            .WithStringField(v.ViewName.SanitiseFieldName(), v.ViewName, false, SyntaxKind.PublicKeyword, SyntaxKind.ReadOnlyKeyword))))
                .ForEach(subpathViews.GroupBy(v => v.TemplateKind), (c, g) => c
                    // static readonly _{viewFolder}Class s_{viewFolder} = new _{viewFolder}Class();
                    // public _{viewFolder}Class {viewFolder} => s_{viewFolder};
                    .WithStaticFieldBackedProperty(g.Key, $"_{g.Key}Class", false, SyntaxKind.PublicKeyword)
                    /* public class _{viewFolder}Class
                     * {
                     * [...] */
                    .WithChildClass($"_{g.Key}Class", tc => tc
                        .WithModifiers(SyntaxKind.PublicKeyword, SyntaxKind.PartialKeyword)
                        // static readonly _ViewNamesClass s_ViewNames = new _ViewNamesClass();
                        // public _ViewNamesClass ViewNames => s_ViewNames;
                        .WithStaticFieldBackedProperty("ViewNames", ViewNamesClassName, false, SyntaxKind.PublicKeyword)
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

            if (!classBuilder.IsGenerated)
                viewsClass.WithGeneratedNonUserCodeAttributes();

            return classBuilder
                .WithMember(viewsClass.Build())
                .WithStaticFieldBackedProperty("Views", viewsClass.Name, !classBuilder.IsGenerated, SyntaxKind.PublicKeyword);
        }
    }
}
