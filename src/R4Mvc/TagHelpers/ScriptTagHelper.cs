using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace R4Mvc.TagHelpers
{
    [HtmlTargetElement("script", Attributes = SourceAttribute)]
    public class ScriptTagHelper : UrlResolutionTagHelper
    {
        private const string SourceAttribute = "mvc-src";

        public ScriptTagHelper(IUrlHelperFactory urlHelperFactory, HtmlEncoder htmlEncoder)
            : base(urlHelperFactory, htmlEncoder)
        { }

        [HtmlAttributeName(SourceAttribute)]
        public string Source { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.SetAttribute("src", Source);
            ProcessUrlAttribute("src", output);
        }
    }
}
