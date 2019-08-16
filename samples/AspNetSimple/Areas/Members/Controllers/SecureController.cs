using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetSimple.Areas.Members.Controllers
{
    [Authorize]
    [RequireHttps]
    [MembersArea]
    public partial class SecureController : Controller
    {
        public virtual IActionResult Index() => throw new NotImplementedException();
    }
}
