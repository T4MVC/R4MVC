using System;
using Microsoft.Framework.Runtime;
using R4Mvc;

namespace R4MvcHostApp.Compiler.Preprocess
{
    public class R4MvcPreCompileModule : ICompileModule
    {
        public void BeforeCompile(IBeforeCompileContext context)
        {
            R4MvcGenerator.Generate(context);
        }

        public void AfterCompile(IAfterCompileContext context)
        {

        }
    }
}