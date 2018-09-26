namespace Microsoft.AspNetCore.Mvc
{
    public interface IR4PageActionResult : IR4ActionResult
    {
        string PageName { get; set; }
        string PageHandler { get; set; }
        string Protocol { get; set; }
    }
}
