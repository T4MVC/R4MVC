using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using R4Mvc.Tools.CodeGen;
using R4Mvc.Tools.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static R4Mvc.Tools.Extensions.SyntaxNodeHelpers;

namespace R4Mvc.Tools.Services
{
    public class ControllerGeneratorService : IControllerGeneratorService
    {
        private readonly Settings _settings;

        public ControllerGeneratorService(Settings settings)
        {
            _settings = settings;
        }

        public string GetControllerArea(INamedTypeSymbol controllerSymbol)
        {
            AttributeData areaAttribute = null;
            var typeSymbol = controllerSymbol;
            while (typeSymbol != null && areaAttribute == null)
            {
                areaAttribute = typeSymbol.GetAttributes().FirstOrDefault(a => a.AttributeClass.InheritsFrom<AreaAttribute>());
                typeSymbol = typeSymbol.BaseType;
            }
            if (areaAttribute == null)
                return string.Empty;

            if (areaAttribute.AttributeClass.ToDisplayString() == typeof(AreaAttribute).FullName)
                return areaAttribute.ConstructorArguments[0].Value?.ToString();

            // parse the constructor to get the area name from derived types
            if (areaAttribute.AttributeClass.BaseType.ToDisplayString() == typeof(AreaAttribute).FullName)
            {
                // direct descendant. Reading the area name from the constructor
                var constructorInit = areaAttribute.AttributeConstructor.DeclaringSyntaxReferences
                    .SelectMany(s => s.SyntaxTree.GetRoot().DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>().Where(c => c.Identifier.Text == areaAttribute.AttributeClass.Name))
                    .SelectMany(s => s.DescendantNodesAndSelf().OfType<ConstructorInitializerSyntax>())
                    .First();
                if (constructorInit.ArgumentList.Arguments.Count > 0)
                {
                    var arg = constructorInit.ArgumentList.Arguments[0];
                    if (arg.Expression is LiteralExpressionSyntax litExp)
                    {
                        return litExp.Token.ValueText;
                    }
                }
            }
            return string.Empty;
        }

        public ClassDeclarationSyntax GeneratePartialController(ControllerDefinition controller)
        {
            // build controller partial class node 
            var genControllerClass = new ClassBuilder(controller.Symbol.Name)               // public partial {controllerClass}
                .WithModifiers(SyntaxKind.PublicKeyword, SyntaxKind.PartialKeyword)
                .WithTypeParameters(controller.Symbol.TypeParameters.Select(tp => tp.Name).ToArray()); // optional <T1, T2, …>

            // add a default constructor if there are some but none are zero length
            var gotCustomConstructors = controller.Symbol.Constructors
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
            AddParameterlessMethods(genControllerClass, controller.Symbol);

            var actionsExpression = controller.AreaKey != null
                ? SyntaxNodeHelpers.MemberAccess(_settings.HelpersPrefix + "." + controller.AreaKey, controller.Name)
                : SyntaxNodeHelpers.MemberAccess(_settings.HelpersPrefix, controller.Name);
            var controllerMethodNames = SyntaxNodeHelpers.GetPublicNonGeneratedMethods(controller.Symbol).Select(m => m.Name).Distinct().ToArray();
            genControllerClass
                .WithExpressionProperty("Actions", controller.Symbol.Name, actionsExpression, SyntaxKind.PublicKeyword)
                .WithStringField("Area", controller.Area, true, SyntaxKind.PublicKeyword, SyntaxKind.ReadOnlyKeyword)
                .WithStringField("Name", controller.Name, true, SyntaxKind.PublicKeyword, SyntaxKind.ReadOnlyKeyword)
                .WithStringField("NameConst", controller.Name, true, SyntaxKind.PublicKeyword, SyntaxKind.ConstKeyword)
                .WithStaticFieldBackedProperty("ActionNames", "ActionNamesClass", SyntaxKind.PublicKeyword)
                /* [GeneratedCode, DebuggerNonUserCode]
                 * public class ActionNamesClass
                 * {
                 *  public readonly string {action} = "{action}";
                 * }
                 */
                .WithChildClass("ActionNamesClass", ac => ac
                    .WithModifiers(SyntaxKind.PublicKeyword)
                    .WithGeneratedNonUserCodeAttributes()
                    .ForEach(controllerMethodNames, (c, m) => c
                        .WithStringField(m, m, false, SyntaxKind.PublicKeyword, SyntaxKind.ReadOnlyKeyword)))
                /* [GeneratedCode, DebuggerNonUserCode]
                 * public class ActionNameConstants
                 * {
                 *  public const string {action} = "{action}";
                 * }
                 */
                .WithChildClass("ActionNameConstants", ac => ac
                    .WithModifiers(SyntaxKind.PublicKeyword)
                    .WithGeneratedNonUserCodeAttributes()
                    .ForEach(controllerMethodNames, (c, m) => c
                        .WithStringField(m, m, false, SyntaxKind.PublicKeyword, SyntaxKind.ConstKeyword)))
                .WithViewsClass(controller.Views);

            return genControllerClass.Build();
        }

        public ClassDeclarationSyntax GenerateR4Controller(ControllerDefinition controller)
        {
            var className = GetR4MVCControllerClassName(controller.Symbol);
            controller.FullyQualifiedR4ClassName = $"{controller.Namespace}.{className}";

            /* [GeneratedCode, DebuggerNonUserCode]
             * public partial class R4MVC_{Controller} : {Controller}
             * {
             *  public R4MVC_{Controller}() : base(Dummy.Instance) {}
             * }
             */
            var r4ControllerClass = new ClassBuilder(className)
                .WithModifiers(SyntaxKind.PublicKeyword, SyntaxKind.PartialKeyword)
                .WithGeneratedNonUserCodeAttributes()
                .WithBaseTypes(controller.Symbol.ToQualifiedName())
                .WithConstructor(c => c
                    .WithBaseConstructorCall(m => IdentifierName(Constants.DummyClass + "." + Constants.DummyClassInstance))
                    .WithModifiers(SyntaxKind.PublicKeyword));
            AddMethodOverrides(r4ControllerClass, controller.Symbol);
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

        private void AddParameterlessMethods(ClassBuilder genControllerClass, ITypeSymbol mvcSymbol)
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
                            .ReturnNewObject(Constants.ActionResultClass, "Area", "Name", "ActionNames." + method.Key)));
        }

        private void AddMethodOverrides(ClassBuilder classBuilder, ITypeSymbol mvcSymbol)
        {
            const string overrideMethodSuffix = "Override";
            foreach (var method in mvcSymbol.GetPublicNonGeneratedMethods())
            {
                var callInfoType = Constants.ActionResultClass;
                if (method.ReturnType.InheritsFrom<JsonResult>())
                    callInfoType = Constants.JsonResultClass;
                else if (method.ReturnType.InheritsFrom<ContentResult>())
                    callInfoType = Constants.ContentResultClass;
                else if (method.ReturnType.InheritsFrom<RedirectResult>())
                    callInfoType = Constants.RedirectResultClass;
                else if (method.ReturnType.InheritsFrom<RedirectToActionResult>())
                    callInfoType = Constants.RedirectToActionResultClass;
                else if (method.ReturnType.InheritsFrom<RedirectToRouteResult>())
                    callInfoType = Constants.RedirectToRouteResultClass;


                Action<BodyBuilder> returnStatement;
                var returnType = method.ReturnType as INamedTypeSymbol;
                if (returnType.InheritsFrom<Task>() == true)
                {
                    var result = returnType.TypeArguments.Length > 0
                        ? "callInfo as " + returnType.TypeArguments[0]
                        : "callInfo";
                    // return Task.FromResult(callInfo as TResult);
                    returnStatement = b => b.ReturnMethodCall(typeof(Task).FullName, "FromResult", result);
                }
                else
                {
                    // return callInfo;
                    returnStatement = b => b.ReturnVariable("callInfo");
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
                            .VariableFromNewObject("callInfo", callInfoType, "Area", "Name", "ActionNames." + method.Name)
                            .ForEach(method.Parameters, (cb, p) => cb
                                .MethodCall("ModelUnbinderHelpers", "AddRouteValues", "callInfo.RouteValueDictionary", ParameterSource.Instance.String(p.Name), p.Name))
                            .MethodCall(null, method.Name + overrideMethodSuffix, new[] { "callInfo" }.Concat(method.Parameters.Select(p => p.Name)).ToArray())
                            .Statement(returnStatement)
                        ));
            }
        }

        internal static string GetR4MVCControllerClassName(INamedTypeSymbol controllerClass)
        {
            return string.Format("R4MVC_{0}", controllerClass.Name);
        }
    }
}
