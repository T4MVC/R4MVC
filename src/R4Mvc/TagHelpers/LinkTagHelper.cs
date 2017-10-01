using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace R4Mvc.TagHelpers
{
    [HtmlTargetElement("link", Attributes = HrefAttribute, TagStructure = TagStructure.WithoutEndTag)]
    public class LinkTagHelper : UrlResolutionTagHelper
    {
        private const string HrefAttribute = "mvc-href";

        public LinkTagHelper(IUrlHelperFactory urlHelperFactory, HtmlEncoder htmlEncoder)
            : base(urlHelperFactory, htmlEncoder)
        { }

        [HtmlAttributeName(HrefAttribute)]
        public string Source { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.SetAttribute("href", Source);
            ProcessUrlAttribute("href", output);
        }
    }
}
