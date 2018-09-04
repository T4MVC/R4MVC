using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Build.Locator;
using Microsoft.Extensions.Configuration;

namespace R4Mvc.Tools.Commands
{
    [Description(Description)]
    public class VSInstancesCommand : ICommand
    {
        public const string Summary = "List the available VS/MSBuild instances";
        private const string Description = Summary + @"
Usage: vsinstances [showPath]
showpath:
    Should this tool output the path for the VS/MSBuild instance. Defaults to false";

        public Task Run(string projectPath, IConfiguration configuration)
        {
            var showPath = configuration.GetValue<bool?>("showPath") ?? false;

            var instances = MSBuildLocator.QueryVisualStudioInstances().ToArray();
            if (instances.Length == 0)
            {
                Console.WriteLine("No Visual Studio / MSBuild instances found.");
                return Task.CompletedTask;
            }

            Console.WriteLine("Available Visual Studio / MSBuild intances:");
            var index = 1;
            foreach (var instance in instances)
            {
                Console.WriteLine($"  - {index++}: {instance.Name} - {instance.Version}");
                if (showPath)
                {
                    Console.WriteLine($"       {instance.MSBuildPath}");
                    Console.WriteLine();
                }
            }
            return Task.CompletedTask;
        }
    }
}
