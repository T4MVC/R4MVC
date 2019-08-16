using System;
using Microsoft.AspNetCore.Mvc;

namespace AspNetSimple.Areas.Members.Controllers
{
    [Area("Members")]
    public partial class ManualAreaController : Controller
    {
        public virtual IActionResult Index() => throw new NotImplementedException();
    }
}
