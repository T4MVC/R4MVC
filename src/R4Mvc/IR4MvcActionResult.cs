using Microsoft.AspNetCore.Routing;

namespace Microsoft.AspNetCore.Mvc
{
    public interface IR4MvcActionResult : IR4ActionResult
    {
        string Controller { get; set; }
        string Action { get; set; }
        new string Protocol { get; set; }
        new RouteValueDictionary RouteValueDictionary { get; set; }
    }
}
