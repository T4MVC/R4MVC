using System;

using R4Mvc;

namespace R4MvcHostApp.Compiler.Preprocess
{
	public class R4MvcCompiler : R4MVCCompilerModule
	{
		public R4MvcCompiler(IServiceProvider serviceProvider) : base(serviceProvider)
		{
			// Uncomment this method to register custom locators
			//public override void RegisterCustomLocators()
			//{
			//    ControllerLocators.Clear();
			//    ControllerLocators.Add(new CustomControllerLocator());
			//}
		}
	}
}