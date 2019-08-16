using Microsoft.AspNetCore.Mvc;

namespace AspNetFeatureFolders.Features.Calculator
{
    public partial class CalculatorController : Controller
    {
        public virtual IActionResult Index() => View();
    }
}
