// BaseController.cs
/// <summary>
/// Tests SyntaxRewriters ability to find controllers
/// </summary>
namespace R4Mvc.Test.Site.Controllers
{
	using Microsoft.AspNet.Mvc;

	public abstract class BaseController : Controller
	{
	}

	public class CustomerController1 : BaseController
	{
	}

	public class CustomController2 : CustomerController1
	{
	}
}