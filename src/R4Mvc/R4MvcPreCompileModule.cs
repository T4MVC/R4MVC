namespace R4Mvc
{
	using Microsoft.Framework.Runtime;

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