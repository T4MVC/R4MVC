using System;

namespace Microsoft.AspNet.Mvc
{
	/// <summary>
	/// Force exclusion of a class for R4MVC code generation
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class R4MVCExcludeAttribute : Attribute
	{
	}
}