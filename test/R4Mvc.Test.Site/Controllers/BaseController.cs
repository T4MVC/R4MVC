/// <summary>
/// Tests SyntaxRewriters ability to find controllers
/// </summary>
namespace R4Mvc.Test.Site.Controllers
{
	using System.CodeDom.Compiler;

	using Microsoft.AspNet.Mvc;

	public abstract class BaseController : Controller
	{
		ActionResult TestMethod()
		{
			return new EmptyResult();;
		}

		ActionResult TestMethod(string arg1)
		{
			return new EmptyResult();
		}
	}

	public abstract class BaseController<T> : BaseController
		where T : class
	{
	}

	public class CustomGenericController : BaseController<Dummy>
	{
	}

	public class CustomController1 : BaseController
	{
	}

	public class CustomController2 : CustomController1
	{
	}

	public partial class CustomPartialController : BaseController
	{
	}

	[GeneratedCode("R4MVC", "1.0.0.0")]
	public partial class CustomPartialController
	{
	}

	public partial class Dummy {}
}