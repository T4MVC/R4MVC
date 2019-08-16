using System;

namespace Microsoft.AspNetCore.Mvc
{
    /// <summary>
    /// Force exclusion of a class or action for R4MVC code generation
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class R4MvcExcludeAttribute : Attribute
    {
    }
}
