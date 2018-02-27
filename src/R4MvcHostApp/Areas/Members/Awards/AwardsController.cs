using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace R4MvcHostApp.Areas.Members.Awards
{
    [MembersArea]
    public partial class AwardsController : Controller
    {
        public virtual IActionResult Index()
        {
            return View();
        }
    }
}
