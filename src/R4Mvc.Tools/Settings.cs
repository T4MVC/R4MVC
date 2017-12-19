using System.Linq;

namespace R4Mvc.Tools
{
    public class Settings
    {
        public string HelpersPrefix { get; set; } = "MVC";
        public string R4MvcNamespace { get; set; } = "R4Mvc";
        public string LinksNamespace { get; set; } = "Links";
        public bool SplitIntoMultipleFiles { get; set; } = true;
        public string wwwroot { get; set; } = "wwwroot";
        public string[] wwwrootInclude { get; set; }

        internal bool IncludeContainer(string container)
        {
            return wwwrootInclude == null || wwwrootInclude.Any(i => Match(i, container));
        }

        bool Match(string current, string container)
        {
            return container.StartsWith(current, System.StringComparison.InvariantCultureIgnoreCase);
        }

    }
}
