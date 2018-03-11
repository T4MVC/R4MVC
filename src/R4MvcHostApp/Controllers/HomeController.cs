using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using R4MvcHostApp.Models;

namespace R4MvcHostApp.Controllers
{
    public partial class HomeController : AboutControllerBase
    {
        public virtual IActionResult Index()
        {
            return View();
        }

        public virtual IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public virtual IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public virtual IActionResult Error()
        {
            return View();
        }

        public virtual Task<IActionResult> TaskActionResult()
        {
            return Task.FromResult(View() as IActionResult);
        }

        public virtual Task TaskResult()
        {
            return Task.CompletedTask;
        }

        public virtual Task<JsonResult> TaskJsonResult()
        {
            return Task.FromResult(Json(null));
        }

        public virtual ActionResult ActionMethod(int id)
        {
            return View();
        }

        public virtual JsonResult JsonMethod(int id)
        {
            return Json(new object());
        }

        public virtual ContentResult ContentMethod(int id)
        {
            return Content("Hello World");
        }

        public virtual RedirectResult RedirectMethod(int id)
        {
            return Redirect(" ");
        }

        public virtual RedirectToActionResult RedirectToActionMethod(int id)
        {
            return RedirectToAction(" ");
        }

        public virtual RedirectToRouteResult RedirectToRouteMethod(int id)
        {
            return RedirectToRoute(null);
        }

        public virtual ApplicationUser User()
        {
            return null;
        }

        public virtual ApplicationUser[] Users()
        {
            return null;
        }
    }
}
