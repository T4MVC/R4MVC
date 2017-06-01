using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace R4Mvc.TagHelpers
{
    [HtmlTargetElement("a", Attributes = ActionAttribute)]
    public class AnchorTagHelper : TagHelper
    {
        private const string ActionAttribute = "mvc-action";

        private readonly IUrlHelperFactory _urlHelperFactory;
        public AnchorTagHelper(IUrlHelperFactory urlHelperFactory)
        {
            _urlHelperFactory = urlHelperFactory;
        }

        [ViewContext]
        public ViewContext ViewContext { get; set; }
        [HtmlAttributeName(ActionAttribute)]
        public object ObjectAction { get; set; }
        [HtmlAttributeName(ActionAttribute)]
        public IActionResult Action { get; set; }
        [HtmlAttributeName(ActionAttribute)]
        public Task<IActionResult> TaskAction { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.RemoveAll(ActionAttribute);
            var urlHelper = _urlHelperFactory.GetUrlHelper(ViewContext);

            string url = null;
            switch (ObjectAction)
            {
                case IR4MvcActionResult t4mvcActionResult:
                    url = urlHelper.RouteUrl(t4mvcActionResult.RouteValueDictionary);
                    break;
                case Task<IActionResult> taskActionResult when taskActionResult.Result is IR4MvcActionResult t4mvcActionResult:
                    url = urlHelper.RouteUrl(t4mvcActionResult.RouteValueDictionary);
                    break;
            }

            if (url != null)
                output.Attributes.SetAttribute("href", url);
        }
    }
}
