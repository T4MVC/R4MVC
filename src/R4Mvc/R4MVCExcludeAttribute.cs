using System;

namespace Microsoft.AspNetCore.Mvc
{
    /// <summary>
    /// Force exclusion of a class for R4MVC code generation
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class R4MvcExcludeAttribute : Attribute
    {
    }
}
