using Microsoft.AspNetCore.Mvc;

namespace AspNetFeatureFolders.Areas.Members.Features.Manage
{
    [MembersArea]
    public partial class ManageController : Controller
    {
        public virtual IActionResult Index() => View();
    }
}
