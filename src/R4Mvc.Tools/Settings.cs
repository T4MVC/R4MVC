﻿namespace R4Mvc.Tools
{
    public class Settings
    {
        public string HelpersPrefix { get; set; } = "MVC";
        public string R4MvcNamespace { get; set; } = "R4Mvc";
        public string LinksNamespace { get; set; } = "Links";
        public bool SplitIntoMultipleFiles { get; set; } = true;
        public string StaticFilesPath { get; set; } = "wwwroot";
        public string[] AbsoluteStaticFilesPaths { get; set; } =  new string[0];
        public string[] ExcludedStaticFileExtensions { get; set; }
    }
}
