using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace R4MvcHostApp.Areas.Admin.MisplacedFeature
{
    [AdminArea]
    public partial class MisplacedFeatureController : Controller
    {
        public virtual IActionResult Index()
        {
            return View();
        }
    }
}
