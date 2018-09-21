using Microsoft.AspNetCore.Routing;

namespace Microsoft.AspNetCore.Mvc
{
    public interface IR4MvcPageActionResult
    {
        string PageName { get; set; }
        string PageHandler { get; set; }
        string Protocol { get; set; }
        RouteValueDictionary RouteValueDictionary { get; set; }
    }
}
