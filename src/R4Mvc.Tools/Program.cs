using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using R4Mvc.Tools.Commands;
using R4Mvc.Tools.Commands.Core;
using R4Mvc.Tools.Locators;
using R4Mvc.Tools.Services;

namespace R4Mvc.Tools
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine($"  R4Mvc Generator Tool v{GetVersion()}");
            Console.WriteLine();

            var commandLineConfig = BuildCommandLineConfig(ref args);

            var commands = CommandResolver.GetCommands();
            ICommand command = null;
            if (args.Length > 0)
            {
                command = commands.FirstOrDefault(c => c.Key.Equals(args[0], StringComparison.OrdinalIgnoreCase));
                if (command != null)
                    RemoveItem(ref args, 0);
            }
            if (command == null)
                command = commands.OfType<HelpCommand>().First();

            string projectPath = null;
            if (!command.IsGlobal)
            {
                projectPath = GetProjectPath(commandLineConfig);
                if (projectPath == null)
                    return;
            }
            var configuration = LoadConfiguration(projectPath, commandLineConfig);

            var services = new ServiceCollection();
            ConfigureServices(services, configuration);

            var serviceProvider = services.BuildServiceProvider();

            var commandRunner = serviceProvider.GetService(command.GetCommandType()) as ICommandRunner;
            await commandRunner.Run(projectPath, configuration, args);
        }

        internal static string GetVersion()
        {
            var assembly = typeof(Program).Assembly;
            var version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
            if (version == null)
                version = assembly.GetName().Version.ToString();
            return version;
        }

        static IConfigurationSource BuildCommandLineConfig(ref string[] args)
        {
            if (args.Length == 0)
                return null;

            var triggers = new[] { "-", "--", "/" };
            var commandArgs = new List<string>();
            int i = 0;
            bool usingSpace = !args.Any(a => triggers.Any(s => a.StartsWith(s)) && a.Contains("="));
            while (i < args.Length)
            {
                var arg = args[i];
                var trigger = triggers.FirstOrDefault(s => arg.StartsWith(s));
                if (trigger == null)
                {
                    i++;
                    continue;
                }

                commandArgs.Add(args[i]);
                RemoveItem(ref args, i);

                if (usingSpace)
                {
                    commandArgs.Add(args[i]);
                    RemoveItem(ref args, i);
                }
            }

            return new ConfigurationBuilder()
                .AddCommandLine(commandArgs.ToArray(), new Dictionary<string, string>
                {
                    ["-p"] = "ProjectPath",
                    ["-vsi"] = "vsinstance",
                })
                .Sources[0];
        }

        static void RemoveItem(ref string[] args, int index)
        {
            for (int i = index; i + 1 < args.Length; i++)
                args[i] = args[i + 1];
            Array.Resize(ref args, args.Length - 1);
        }

        static string GetProjectPath(IConfigurationSource configuration)
        {
            var config = new ConfigurationBuilder().Add(configuration).Build();
            var projectPath = config["ProjectPath"];
            if (string.IsNullOrWhiteSpace(projectPath))
                projectPath = Environment.CurrentDirectory;
            projectPath = Path.GetFullPath(projectPath);

            if (projectPath.EndsWith(".csproj") && File.Exists(projectPath))
                return projectPath;

            // Not pointing to a project file
            if (File.Exists(projectPath))
                projectPath = Path.GetDirectoryName(projectPath);

            if (!Directory.Exists(projectPath))
            {
                Console.Error.WriteLine("Project path doesn't exist");
                return null;
            }

            var projFiles = Directory.GetFiles(projectPath, "*.csproj");
            switch (projFiles.Length)
            {
                case 1:
                    return projFiles[0];

                case 0:
                    Console.Error.WriteLine("Project path not found. Pass one as a parameter or run this within the project directory.");
                    return null;

                default:
                    Console.Error.WriteLine("More than one project file found. Aborting, just to be safe!");
                    return null;
            }
        }

        static IConfigurationRoot LoadConfiguration(string projectPath, IConfigurationSource commandLineConfig)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder();
            if (projectPath != null)
                builder = builder.AddJsonFile(Path.Combine(Path.GetDirectoryName(projectPath), Constants.R4MvcSettingsFileName), optional: true);
            if (commandLineConfig != null)
                builder = builder.Add(commandLineConfig);

            return builder.Build();
        }

        static void ConfigureServices(IServiceCollection services, IConfigurationRoot configuration)
        {
            services.AddOptions();
            services.Configure<Settings>(configuration);
            services.AddTransient(sc => sc.GetService<IOptions<Settings>>().Value);

            services.AddTransient<IViewLocator, FeatureFolderRazorViewLocator>();
            services.AddTransient<IViewLocator, DefaultRazorViewLocator>();
            services.AddTransient<IPageViewLocator, DefaultRazorPageViewLocator>();
            services.AddTransient<IStaticFileLocator, DefaultStaticFileLocator>();
            services.AddTransient<IFileLocator, PhysicalFileLocator>();
            services.AddTransient<IGeneratedFileTesterService, GeneratedFileTesterService>();
            services.AddTransient<IStaticFileGeneratorService, StaticFileGeneratorService>();
            services.AddTransient<IControllerRewriterService, ControllerRewriterService>();
            services.AddTransient<IControllerGeneratorService, ControllerGeneratorService>();
            services.AddTransient<IPageRewriterService, PageRewriterService>();
            services.AddTransient<IPageGeneratorService, PageGeneratorService>();
            services.AddTransient<IFilePersistService, FilePersistService>();
            services.AddTransient<R4MvcGeneratorService>();

            foreach (var runnerType in CommandResolver.GetCommandRunnerTypes())
                services.AddTransient(runnerType);
        }
    }
}
