using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace R4Mvc.TagHelpers
{
    [HtmlTargetElement("form", Attributes = ActionAttribute)]
    public class FormTagHelper : TagHelper
    {
        private const string ActionAttribute = "mvc-action";

        private readonly IUrlHelperFactory _urlHelperFactory;
        public FormTagHelper(IUrlHelperFactory urlHelperFactory)
        {
            _urlHelperFactory = urlHelperFactory;
        }

        [ViewContext]
        public ViewContext ViewContext { get; set; }
        [HtmlAttributeName(ActionAttribute)]
        public IActionResult Action { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.RemoveAll(ActionAttribute);
            var urlHelper = _urlHelperFactory.GetUrlHelper(ViewContext);

            if (Action is IR4MvcActionResult t4mvcActionResult)
            {
                var url = urlHelper.RouteUrl(t4mvcActionResult.RouteValueDictionary);
                output.Attributes.SetAttribute("action", url);
            }
        }
    }
}
