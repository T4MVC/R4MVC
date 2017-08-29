using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace R4MvcHostApp.Areas.Admin
{
    [AdminArea]
    public partial class AdminController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
        }


        protected virtual void NonActionMethodVoidReturn()
        {

        }

        protected virtual int NonActionMethodWithReturnValue()
        {
            return 1;
        }

        public virtual void PublicNonActionMethodVoidReturn()
        {

        }

        [NonAction]
        public virtual int PublicNonActionMethodMustBeExcludedWithNonAction()
        {
            return 1;
        }
    }
}
