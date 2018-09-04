using System;
using System.ComponentModel;
using System.Linq;

namespace R4Mvc.Tools.Commands
{
    public static class CommandResolver
    {
        public static Type GetCommand(ref string[] args)
        {
            if (!(args?.Length > 0))
                return null;

            switch (args[0].ToLowerInvariant())
            {
                case "vsinstances":
                    args = args.Skip(1).ToArray();
                    return typeof(VSInstancesCommand);

                case "generate":
                    args = args.Skip(1).ToArray();
                    return typeof(GenerateCommand);

                case "remove":
                    args = args.Skip(1).ToArray();
                    return typeof(RemoveCommand);

                default:
                    return null;
            }
        }

        public static void DisplayHelp()
        {
            Console.WriteLine("Usage: dotnet r4mvc {command} [arguments]");
            Console.WriteLine("Available commands:");
            Console.WriteLine("  generate     " + GenerateCommand.Summary);
            Console.WriteLine("  remove       " + RemoveCommand.Summary);
            Console.WriteLine("  vsinstances  " + VSInstancesCommand.Summary);
        }

        public static void DisplayHelp(Type commandType)
        {
            var descAttr = commandType.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (descAttr?.Length > 0)
                Console.WriteLine(((DescriptionAttribute)descAttr[0]).Description);
            else
                Console.WriteLine("Command type: " + commandType);
        }
    }
}
