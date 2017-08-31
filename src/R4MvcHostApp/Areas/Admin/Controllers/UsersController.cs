using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using R4MvcHostApp.Areas.Admin.Models;

namespace R4MvcHostApp.Areas.Admin.Controllers
{
    [AdminArea]
    public partial class UsersController : Controller
    {

        static UsersController()
        {
           
        }

        public virtual IActionResult Index()
        {
            return View();
        }

        public virtual IActionResult Index2(Index2ViewModel model)
        {
            model = new Index2ViewModel { Id = "hello", Value = 10 };

            return View(model);
        }
    }


}