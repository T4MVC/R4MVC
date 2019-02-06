using System;
using System.Threading.Tasks;
using AspNetSimple.Models;
using Microsoft.AspNetCore.Mvc;

namespace AspNetSimple.Controllers
{
    public partial class WebApiController : ControllerBase
    {
        public virtual Product ApiCall() => throw new NotImplementedException();
        public virtual Product ApiCallWithParams(int id) => throw new NotImplementedException();

        public virtual Task<Product> TaskApiCall() => throw new NotImplementedException();
        public virtual Task<Product> TaskApiCallWithParams(int id) => throw new NotImplementedException();

        public virtual ActionResult<Product> ApiCallTyped() => throw new NotImplementedException();
        public virtual ActionResult<Product> ApiCallTypedWithParams(int id) => throw new NotImplementedException();

        public virtual Task<ActionResult<Product>> TaskApiCallTyped() => throw new NotImplementedException();
        public virtual Task<ActionResult<Product>> TaskApiCallTypedWithParams(int id) => throw new NotImplementedException();
    }
}
