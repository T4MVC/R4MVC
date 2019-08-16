using Microsoft.AspNetCore.Mvc;

namespace AspNetFeatureFolders.Areas.Products.Search
{
    [ProductsArea]
    public partial class SearchController : Controller
    {
        public virtual IActionResult Index() => View();
    }
}
