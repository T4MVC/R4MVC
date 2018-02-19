using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace R4MvcHostApp.Areas.Features.Home
{
    [Area("Features")]
    public partial class HomeController : Controller
    {
        public HomeController()
        {

        }

        public virtual IActionResult Index()
        {
            return View(Views.Index);
        }
    }
}
