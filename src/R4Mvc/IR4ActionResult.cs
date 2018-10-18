using Microsoft.AspNetCore.Routing;

namespace Microsoft.AspNetCore.Mvc
{
    public interface IR4ActionResult
    {
        string Protocol { get; }
        RouteValueDictionary RouteValueDictionary { get; }
    }
}
