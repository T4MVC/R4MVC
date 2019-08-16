using System;
using Microsoft.AspNetCore.Mvc;

namespace AspNetSimple.Controllers
{
    public abstract class DebugControllerBase : Controller
    {
        public IActionResult Debug() => throw new NotImplementedException();
        public virtual IActionResult OverrideMe() => throw new NotImplementedException();
    }
}
