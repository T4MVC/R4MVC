using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace R4MvcHostApp.Areas.Admin.Controllers
{
    [AdminArea]
    public partial class UsersController : Controller
    {

        static UsersController()
        {
            ModelUnbinderHelpers.ModelUnbinders.Add(typeof(Index2ViewModel), new R4Mvc.ModelUnbinders.SimplePropertyModelUnbinder());
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

    public class Index2ViewModel
    {
        public string Id { get; set; }
        public int Value { get; set; }

        public string UnbindedUrl { get; set; }
    }
}