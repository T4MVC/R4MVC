//using Microsoft.Framework.ConfigurationModel;
using System;

namespace R4Mvc.Tools.Services
{
    public class Settings : ISettings
    {
        //private readonly IConfiguration _configuration;

        public const string SettingsFileName = "r4mvc.json";

        public Settings(string projectDirectory) : this(projectDirectory, SettingsFileName) { }

        internal Settings(string projectDirectory, string settingsFile)
        {
            //var configuration = new Configuration();
            //configuration.AddJsonFile(Path.Combine(projectDirectory, settingsFile));

            //_configuration = configuration;
        }

        public string HelpersPrefix
        {
            get { return GetStringValue(Constants.ConfigKeys.HelpersPrefix, "MVC"); }
        }

        public string R4MvcNamespace
        {
            get { return GetStringValue(Constants.ConfigKeys.R4MvcNamespace, "R4Mvc"); }
        }

        public string LinksNamespace
        {
            get { return GetStringValue(Constants.ConfigKeys.LinksNamespace, "Links"); }
        }

        private string GetStringValue(string key, string defaultValue)
        {
            return defaultValue;
            string value;
            //_configuration.TryGet(key, out value);

            //return value ?? defaultValue;
            throw new NotImplementedException();
        }
    }
}
