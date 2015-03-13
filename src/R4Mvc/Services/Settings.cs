using System;
using System.IO;
using Microsoft.Framework.ConfigurationModel;
using R4Mvc.Constants;

namespace R4Mvc.Services
{
    public class Settings : ISettings
    {
        private readonly IConfiguration _configuration;
        
        public const string SettingsFileName = "r4mvc.json";

        public Settings(string projectDirectory) : this (projectDirectory, SettingsFileName) { }

        internal Settings(string projectDirectory, string settingsFile)
        {
            var configuration = new Configuration();
            configuration.AddJsonFile(Path.Combine(projectDirectory, settingsFile));

            _configuration = configuration;
        }

        public string HelpersPrefix
        {
            get
            {
                string prefix;
                _configuration.TryGet(ConfigKeys.HelpersPrefix, out prefix);

                return prefix ?? "MVC";
            }
        }
    }
}