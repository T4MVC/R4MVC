using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#if CORE2
using Microsoft.AspNetCore.Mvc.Infrastructure;
#endif

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

#if CORE2
            if (ObjectAction is IConvertToActionResult convertToActionResult)
                ObjectAction = convertToActionResult.Convert();
#endif
            RouteValueDictionary routeValues = null;
            switch (ObjectAction)
            {
                case IR4ActionResult t4ActionResult:
                    routeValues = t4ActionResult.RouteValueDictionary;
                    break;
                case Task task:
                    var result = task.GetActionResult();
                    if (result is IR4ActionResult taskActionResult)
                        routeValues = taskActionResult.RouteValueDictionary;
                    break;
            }

            if (routeValues != null)
            {
                foreach (var set in RouteValues)
                    routeValues[set.Key] = set.Value;

                var url = urlHelper.RouteUrl(routeValues);
                output.Attributes.SetAttribute("action", url);

                var methodAttr = context.AllAttributes["method"];
                if (string.IsNullOrEmpty(methodAttr?.Value.ToString()))
                    output.Attributes.SetAttribute("method", "post");
            }
        }
    }
}
