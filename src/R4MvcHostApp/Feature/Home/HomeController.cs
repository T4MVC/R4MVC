using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using R4MvcHostApp.Areas.Admin.Models;

namespace R4MvcHostApp.Feature.Home
{
    [Area("Feature")]
    public partial class HomeController : Controller
    {
        public virtual IActionResult Index()
        {
            return View();
        }

        public virtual IActionResult Index(IndexViewModel model)
        {
            model = new IndexViewModel { Id = "hello", Value = 10 };

            return View(model);
        }
    }
}
