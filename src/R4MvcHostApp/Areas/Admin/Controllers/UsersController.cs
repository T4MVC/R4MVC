using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace R4MvcHostApp.Areas.Admin.Controllers
{
    [AdminArea]
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
    }
}