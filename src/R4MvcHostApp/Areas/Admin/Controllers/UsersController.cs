using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace R4MvcHostApp.Areas.Admin.Controllers
{
    public partial class UsersController : AdminController
    {
        IUrlHelper _urlHelper;
        public UsersController(IUrlHelper urlHelper)
        {
            _urlHelper = urlHelper;

        }

        public virtual IActionResult Index()
        {
            return View();
        }

        public virtual IActionResult Edit()
        {
            var url = Url.Action(Actions.Index());

            return RedirectToAction(Actions.Index());
        }

        [NonAction]
        public IActionResult ActionExcluded()
        {
            return View();
        }
    }
}