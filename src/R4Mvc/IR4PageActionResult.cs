using Microsoft.AspNetCore.Routing;

namespace Microsoft.AspNetCore.Mvc
{
    public interface IR4PageActionResult : IR4ActionResult
    {
        string PageName { get; set; }
        string PageHandler { get; set; }
        new string Protocol { get; set; }
        new RouteValueDictionary RouteValueDictionary { get; set; }
    }
}
