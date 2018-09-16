using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using R4Mvc.Tools.Commands.Core;
using R4Mvc.Tools.Extensions;
using R4Mvc.Tools.Services;

namespace R4Mvc.Tools.Commands
{
    public class RemoveCommand : ICommand
    {
        public bool IsGlobal => false;
        public byte Order => 5;
        public string Key => "remove";
        public string Summary => "Remove R4Mvc generated files from the selected project";
        public string Description => Summary + @"
Usage: remove [-p project-path]
project-path:
    Path to the project's .cshtml file";

        public Type GetCommandType() => typeof(Runner);

        public class Runner : ICommandRunner
        {
            private readonly IGeneratedFileTesterService _generatedFileTesterService;
            public Runner(IGeneratedFileTesterService generatedFileTesterService)
            {
                _generatedFileTesterService = generatedFileTesterService;
            }

            public async Task Run(string projectPath, IConfiguration configuration, string[] args)
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
}
