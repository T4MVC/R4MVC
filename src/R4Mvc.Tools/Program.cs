using System;
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
            var assembly = typeof(Program).Assembly;
            var version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
            if (version == null)
                version = assembly.GetName().Version.ToString();
            Console.WriteLine($"  R4Mvc Generator Tool v{version}");
            Console.WriteLine();

            var commands = CommandResolver.GetCommands();
            ICommand command = null;
            if (args.Length > 0)
            {
                command = commands.FirstOrDefault(c => c.Key.Equals(args[0], StringComparison.OrdinalIgnoreCase));
                if (command != null)
                    args = args.Skip(1).ToArray();
            }
            if (command == null)
                command = commands.OfType<HelpCommand>().First();

            string projectPath = null;
            if (!command.IsGlobal)
            {
                projectPath = GetProjectPath(ref args);
            }

            var configuration = LoadConfiguration(args, projectPath);

            var services = new ServiceCollection();
            ConfigureServices(services, configuration);

            var serviceProvider = services.BuildServiceProvider();

            var commandRunner = serviceProvider.GetService(command.GetCommandType()) as ICommandRunner;
            await commandRunner.Run(projectPath, configuration, args);
        }

        static string GetProjectPath(ref string[] args)
        {
            string projectPath = null;
            if (args.Length > 0 && !args[0].StartsWith("-"))
            {
                projectPath = args[0];
                args = args.Skip(1).ToArray();
            }
            if (!string.IsNullOrWhiteSpace(projectPath))
            {
                projectPath = Path.IsPathRooted(projectPath) ? projectPath : Path.Combine(Environment.CurrentDirectory, projectPath);
                if (File.Exists(projectPath) && projectPath.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase))
                    return projectPath;

                Console.WriteLine("Invalid project path.");
            }

            Console.WriteLine("Project path not passed. Searching current path...");
            var projFiles = Directory.GetFiles(Environment.CurrentDirectory, "*.csproj");
            switch (projFiles.Length)
            {
                case 1:
                    return projFiles[0];

                case 0:
                    Console.WriteLine("Project path not found. Pass one as a parameter or run this within the project directory.");
                    break;
                default:
                    Console.WriteLine("More than one project file found. Aborting, just to be safe!");
                    break;
            }
            return null;
        }

        static IConfigurationRoot LoadConfiguration(string[] args, string projectPath)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder();
            if (projectPath != null)
                builder = builder
                .AddJsonFile(Path.Combine(Path.GetDirectoryName(projectPath), Constants.R4MvcSettingsFileName), optional: true);
            return builder
                .AddCommandLine(args)
                .Build();
        }

        static void ConfigureServices(IServiceCollection services, IConfigurationRoot configuration)
        {
            services.AddOptions();
            services.Configure<Settings>(configuration);
            services.AddTransient(sc => sc.GetService<IOptions<Settings>>().Value);

            services.AddTransient<IViewLocator, FeatureFolderRazorViewLocator>();
            services.AddTransient<IViewLocator, DefaultRazorViewLocator>();
            services.AddTransient<IStaticFileLocator, DefaultStaticFileLocator>();
            services.AddTransient<IFileLocator, PhysicalFileLocator>();
            services.AddTransient<IGeneratedFileTesterService, GeneratedFileTesterService>();
            services.AddTransient<IStaticFileGeneratorService, StaticFileGeneratorService>();
            services.AddTransient<IControllerRewriterService, ControllerRewriterService>();
            services.AddTransient<IControllerGeneratorService, ControllerGeneratorService>();
            services.AddTransient<IFilePersistService, FilePersistService>();
            services.AddTransient<R4MvcGeneratorService>();

            foreach (var runnerType in CommandResolver.GetCommandRunnerTypes())
                services.AddTransient(runnerType);
        }
    }
}
