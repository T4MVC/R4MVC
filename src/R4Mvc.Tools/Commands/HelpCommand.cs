using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using R4Mvc.Tools.Commands.Core;

namespace R4Mvc.Tools.Commands
{
    public class HelpCommand : ICommand
    {
        public bool IsGlobal => true;
        public byte Order => 200;
        public string Key => "help";
        public string Summary => "Display this help message";
        public string Description => "Display tool help listing commands and command details";

        public Type GetCommandType() => typeof(Runner);

        public class Runner : ICommandRunner
        {
            public Task Run(string projectPath, IConfiguration configuration, string[] args)
            {
                var commands = CommandResolver.GetCommands();
                ICommand selectedCommand = null;

                if (args.Length > 0)
                    selectedCommand = commands.FirstOrDefault(c => c.Key.Equals(args[0], StringComparison.OrdinalIgnoreCase));

                if (selectedCommand != null)
                    DisplayHelp(selectedCommand);
                else
                    DisplayHelp(commands);

                return Task.CompletedTask;
            }

            public void DisplayHelp(ICommand[] commands)
            {
                Console.WriteLine("Usage: r4mvc [command] [arguments]");
                Console.WriteLine();
                Console.WriteLine("Commands:");
                var maxLength = commands.Max(c => c.Key.Length);
                foreach (var command in commands)
                    Console.WriteLine($"  {command.Key.PadRight(maxLength)}  {command.Summary}");
            }

            public static void DisplayHelp(ICommand command)
            {
                Console.WriteLine($"Usage: r4mvc {command.Key} [arguments]");
                Console.WriteLine();
                Console.WriteLine(command.Description);
            }
        }
    }
}
