using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace R4MvcHostApp.Areas.Admin.Features.TaxCalculator
{
    [AdminArea]
    public partial class TaxCalculatorController : Controller
    {
        // GET: /<controller>/
        public virtual IActionResult Index()
        {
            return View();
        }
    }
}
