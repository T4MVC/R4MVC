using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace R4MvcHostApp.Features.Calculator
{
    public partial class CalculatorController : Controller
    {
        public virtual IActionResult Index()
        {
            return View();
        }
    }
}
