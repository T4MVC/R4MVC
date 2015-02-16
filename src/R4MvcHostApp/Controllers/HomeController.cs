using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc;

namespace R4MvcHostApp.Controllers
{
    public partial class HomeController : Controller
    {
        public virtual IActionResult Index()
        {
            return View();
        }

        public virtual IActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public virtual IActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public virtual IActionResult Error()
        {
            return View("~/Views/Shared/Error.cshtml");
        }
    }
}