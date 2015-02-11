namespace R4MvcHostApp.Compiler.Preprocess
{
	using Microsoft.Framework.Runtime;
	using R4Mvc;

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