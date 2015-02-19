using System;
using Microsoft.Framework.Runtime;

namespace R4Mvc
{
    public class R4MvcPreCompileModule : ICompileModule
    {
        private readonly IServiceProvider _serviceProvider;

        public R4MvcPreCompileModule(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void BeforeCompile(IBeforeCompileContext context)
        {
            
        }

        public void AfterCompile(IAfterCompileContext context)
        {

        }
    }
}