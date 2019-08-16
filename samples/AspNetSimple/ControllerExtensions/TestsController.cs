using System;
using Microsoft.AspNetCore.Mvc;

namespace AspNetSimple.Controllers
{
    public partial class TestsController
    {
        public virtual IActionResult ExtendedPage() => throw new NotImplementedException();
    }
}
