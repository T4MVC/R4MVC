namespace R4Mvc.Tools
{
    internal static class Constants
    {
        internal const string ProjectName = "R4Mvc";
        internal const string Version = "1.0";

        internal const string R4MvcFileName = "R4Mvc.cs";
        internal const string R4MvcGeneratedFileName = "R4Mvc.generated.cs";
        internal const string R4MvcSettingsFileName = "r4mvc.json";

        internal const string DummyClass = "Dummy";
        internal const string DummyClassInstance = "Instance";

        internal const string R4MvcHelpersClass = "R4MvcHelpers";
        internal const string R4MvcHelpers_ProcessVirtualPath = "ProcessVirtualPath";

        private const string ActionResultNamespace = "_Microsoft_AspNetCore_Mvc_";
        internal const string ActionResultClass = ProjectName + ActionResultNamespace + "ActionResult";
        internal const string JsonResultClass = ProjectName + ActionResultNamespace + "JsonResult";
        internal const string ContentResultClass = ProjectName + ActionResultNamespace + "ContentResult";
        internal const string FileResultClass = ProjectName + ActionResultNamespace + "FileResult";
        internal const string RedirectResultClass = ProjectName + ActionResultNamespace + "RedirectResult";
        internal const string RedirectToActionResultClass = ProjectName + ActionResultNamespace + "RedirectToActionResult";
        internal const string RedirectToRouteResultClass = ProjectName + ActionResultNamespace + "RedirectToRouteResult";

        internal static class ConfigKeys
        {
            internal const string HelpersPrefix = "helpersPrefix";
            internal const string R4MvcNamespace = "r4mvcNamespace";
            internal const string LinksNamespace = "linksNamespace";
        }
    }
}
