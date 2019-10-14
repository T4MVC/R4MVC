namespace R4Mvc.Tools
{
    public class Settings
    {
        public string _generatedByVersion { get; set; }
        public bool ShouldSerialize_generatedByVersion() => UpdateGeneratedByVersion;
        public bool UpdateGeneratedByVersion { get; set; } = true;
        public string HelpersPrefix { get; set; } = "MVC";
        public string PageHelpersPrefix { get; set; } = "MVCPages";
        public string R4MvcNamespace { get; set; } = "R4Mvc";
        public string LinksNamespace { get; set; } = "Links";
        public bool SplitIntoMultipleFiles { get; set; } = true;
        public bool SplitViewOnlyPagesIntoMultipleFiles { get; set; } = true;
        public string StaticFilesPath { get; set; } = "wwwroot";
        public string[] ExcludedStaticFileExtensions { get; set; }
        public string[] ReferencedNamespaces { get; set; }
        public string[] PragmaCodes { get; set; }

        // Don't include the page ViewsClass by default, and hide the option unless it's enabled
        // Not sure if we'd even need that, but leaving it in for the time being
        public bool GeneratePageViewsClass { get; set; } = false;
        public bool ShouldSerializeGeneratePageViewsClass() => GeneratePageViewsClass;

        public FeatureFoldersClass FeatureFolders { get; set; } = new FeatureFoldersClass();
        public class FeatureFoldersClass
        {
            public bool Enabled { get; set; }
            public string FeaturesPath { get; set; } = "Features";
            public bool StaticFileAccess { get; set; }
            public string[] FeatureOnlyAreas { get; set; }
        }
    }
}
