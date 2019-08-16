using R4Mvc.Tools;
using Xunit;

namespace R4Mvc.Test.Locators
{
    public abstract class RazorViewLocatorTestsBase
    {
        protected void AssertView(View view, string areaName, string controllerName, string viewName, string templateKind, string viewPath)
        {
            Assert.Equal(areaName, view.AreaName);
            Assert.Equal(controllerName, view.ControllerName);
            Assert.Equal(viewName, view.Name);
            Assert.Equal(viewPath, view.RelativePath.ToString());
            if (templateKind != null)
                Assert.Equal(templateKind, view.TemplateKind);
            else
                Assert.Null(view.TemplateKind);
        }
    }
}
