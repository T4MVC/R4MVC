using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        [ViewContext, HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }
        /// <summary>
        /// The MVC action call (use R4MVC syntax i.e. `MVC.Home.Index()`)
        /// </summary>
        [HtmlAttributeName(ActionAttribute)]
        public object ObjectAction { get; set; }
        [HtmlAttributeName(ActionAttribute)]
        public IActionResult Action { get; set; }
        [HtmlAttributeName(ActionAttribute)]
        public Task<IActionResult> TaskAction { get; set; }
        [HtmlAttributeName("asp-all-route-data", DictionaryAttributePrefix = "asp-route-")]
        public IDictionary<string, string> RouteValues { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.RemoveAll(ActionAttribute);
            var urlHelper = _urlHelperFactory.GetUrlHelper(ViewContext);

            RouteValueDictionary routeValues = null;
            switch (ObjectAction)
            {
                case IR4MvcActionResult t4mvcActionResult:
                    routeValues = t4mvcActionResult.RouteValueDictionary;
                    break;
                case Task<IActionResult> taskActionResult when taskActionResult.Result is IR4MvcActionResult t4mvcActionResult:
                    routeValues = t4mvcActionResult.RouteValueDictionary;
                    break;
            }

            if (routeValues != null)
            {
                foreach (var set in RouteValues)
                    routeValues[set.Key] = set.Value;

                var url = urlHelper.RouteUrl(routeValues);
                output.Attributes.SetAttribute("action", url);
            }
        }
    }
}
