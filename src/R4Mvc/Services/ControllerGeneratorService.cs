using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using R4Mvc.Extensions;

namespace R4Mvc.Services
{
	public class ControllerGeneratorService : IControllerGeneratorService
	{
		private readonly IViewLocatorService _viewLocator;

		public ControllerGeneratorService(IViewLocatorService viewLocator)
		{
			_viewLocator = viewLocator;
		}

		public IEnumerable<NamespaceDeclarationSyntax> GenerateControllers(
			CSharpCompilation compiler,
			IEnumerable<ClassDeclarationSyntax> controllerNodes)
		{
			// controllers might be in different namespaces so should group by namespace 
			var namespaceGroups =
				controllerNodes.GroupBy(x => x.Ancestors().OfType<NamespaceDeclarationSyntax>().First().Name.ToFullString());
			foreach (var namespaceControllers in namespaceGroups)
			{
				// create the namespace for the controllers
				var namespaceNode = SyntaxNodeHelpers.CreateNamespace(namespaceControllers.Key);

				// loop through the controllers and create a partial node for each
				foreach (var mvcControllerNode in namespaceControllers)
				{
					var model = compiler.GetSemanticModel(mvcControllerNode.SyntaxTree);
					var mvcSymbol = model.GetDeclaredSymbol(mvcControllerNode);

					// build controller partial class node 
					// add a default constructor if there are some but none are zero length
					var genControllerClass = SyntaxNodeHelpers.CreateClass(
						mvcSymbol.Name,
						mvcControllerNode.TypeParameterList?.Parameters.ToArray(),
						SyntaxKind.PublicKeyword,
						SyntaxKind.PartialKeyword);

					if (!mvcSymbol.Constructors.IsEmpty || !mvcSymbol.Constructors.Any(x => x.Parameters.Length == 0))
					{
						genControllerClass = genControllerClass.WithDefaultConstructor(true, SyntaxKind.PublicKeyword);
					}

					// add all method stubs, TODO criteria for this: only public virtual actionresults?
					// add subclasses, fields, properties, constants for action names
					genControllerClass =
						genControllerClass.WithMethods(mvcSymbol)
							.WithStringField(
								"Name",
								mvcControllerNode.Identifier.ToString(),
								true,
								SyntaxKind.PublicKeyword,
								SyntaxKind.ReadOnlyKeyword)
							.WithStringField(
								"NameConst",
								mvcControllerNode.Identifier.ToString(),
								true,
								SyntaxKind.PublicKeyword,
								SyntaxKind.ConstKeyword)
							.WithStringField(
								"Area",
								mvcControllerNode.Identifier.ToString(),
								true,
								SyntaxKind.PublicKeyword,
								SyntaxKind.ReadOnlyKeyword)
							.WithField("s_actions", "ActionNamesClass", SyntaxKind.StaticKeyword, SyntaxKind.ReadOnlyKeyword)
							.WithActionNameClass(mvcControllerNode)
							.WithActionConstantsClass(mvcControllerNode)
							.WithField("s_views", "ViewsClass", SyntaxKind.StaticKeyword, SyntaxKind.ReadOnlyKeyword)
							.WithViewsClass(_viewLocator.FindViews());

					// create R4MVC_[Controller] class inheriting from partial
					// TODO chain base constructor call : base(Dummy.Instance)
					// TODO create [method]overrides(T4MVC_System_Web_Mvc_ActionResult callInfo)
					// TODO create method overrides that call above
					var r4ControllerClass =
						SyntaxNodeHelpers.CreateClass(
							GetR4MVCControllerClassName(genControllerClass),
							null,
							SyntaxKind.PublicKeyword,
							SyntaxKind.PartialKeyword)
							.WithAttributes(SyntaxNodeHelpers.CreateGeneratedCodeAttribute(), SyntaxNodeHelpers.CreateDebugNonUserCodeAttribute())
							.WithBaseTypes(mvcControllerNode.ToQualifiedName())
							.WithDefaultConstructor(false, SyntaxKind.PublicKeyword);

					namespaceNode = namespaceNode.AddMembers(genControllerClass).AddMembers(r4ControllerClass);
				}
				yield return namespaceNode;
			}
		}

		private static string GetR4MVCControllerClassName(ClassDeclarationSyntax genControllerClass)
		{
			return string.Format("R4MVC_{0}", genControllerClass.Identifier);
		}
	}
}
