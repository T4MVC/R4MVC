using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using R4Mvc.Tools.Extensions;
using R4Mvc.Tools.Services;

namespace R4Mvc.Tools.Commands
{
    [Description(Description)]
    public class RemoveCommand : ICommand
    {
        public const string Summary = "Remove R4Mvc generated files from the selected project";
        private const string Description = Summary + @"
Usage: remove [project-path]
project-path:
    Path to the project's .cshtml file";

        private readonly IGeneratedFileTesterService _generatedFileTesterService;
        public RemoveCommand(IGeneratedFileTesterService generatedFileTesterService)
        {
            _generatedFileTesterService = generatedFileTesterService;
        }

        public async Task Run(string projectPath, IConfiguration configuration)
        {
            var projectRoot = Path.GetDirectoryName(projectPath);
            var generatedFiles = Directory.GetFiles(projectRoot, "*.generated.cs", SearchOption.AllDirectories);
            foreach (var file in generatedFiles)
            {
                using (var fileStream = File.OpenRead(file))
                {
                    if (await _generatedFileTesterService.IsGenerated(fileStream))
                    {
                        Console.WriteLine("Deleting " + file.GetRelativePath(projectRoot));
                        File.Delete(file);
                    }
                }
            }
        }
    }
}
