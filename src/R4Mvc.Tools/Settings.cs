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
        public string StaticFilesPath { get; set; } = "wwwroot";
        public string[] ExcludedStaticFileExtensions { get; set; }
        public string[] ReferencedNamespaces { get; set; }

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
