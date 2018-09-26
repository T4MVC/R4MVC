using Microsoft.AspNetCore.Routing;

namespace Microsoft.AspNetCore.Mvc
{
    public interface IR4ActionResult
    {
        string Protocol { get; set; }
        RouteValueDictionary RouteValueDictionary { get; set; }
    }
}
