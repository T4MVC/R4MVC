using System;
using System.IO;
//using Microsoft.Framework.ConfigurationModel;
using R4Mvc.Constants;

namespace R4Mvc.Services
{
    public class Settings : ISettings
    {
        //private readonly IConfiguration _configuration;
        
        public const string SettingsFileName = "r4mvc.json";

        public Settings(string projectDirectory) : this (projectDirectory, SettingsFileName) { }

        internal Settings(string projectDirectory, string settingsFile)
        {
            //var configuration = new Configuration();
            //configuration.AddJsonFile(Path.Combine(projectDirectory, settingsFile));

            //_configuration = configuration;
        }

        public string HelpersPrefix
        {
            get { return GetStringValue(ConfigKeys.HelpersPrefix, "MVC"); }
        }

        public string R4MvcNamespace
        {
            get { return GetStringValue(ConfigKeys.R4MvcNamespace, "R4Mvc"); }
        }

        public string LinksNamespace
        {
            get { return GetStringValue(ConfigKeys.LinksNamespace, "Links"); }
        }

        private string GetStringValue(string key, string defaultValue)
        {
            string value;
            //_configuration.TryGet(key, out value);

            //return value ?? defaultValue;
            throw new NotImplementedException();
        }
    }
}
