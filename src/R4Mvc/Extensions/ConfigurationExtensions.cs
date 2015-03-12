using System;
using Microsoft.Framework.ConfigurationModel;
using R4Mvc.Constants;

namespace R4Mvc.Extensions
{
    internal static class ConfigurationExtensions
    {
        internal static string GetHelpersPrefix(this IConfiguration configuration)
        {
            string prefix;
            configuration.TryGet(ConfigKeys.HelpersPrefix, out prefix);

            return prefix ?? "MVC";
        }
    }
}