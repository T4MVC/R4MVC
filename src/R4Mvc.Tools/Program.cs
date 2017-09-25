using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using R4Mvc.Tools.Commands;
using R4Mvc.Tools.Locators;
using R4Mvc.Tools.Services;

namespace R4Mvc.Tools
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Looking for project...");
                var projFiles = Directory.GetFiles(Environment.CurrentDirectory, "*.csproj");
                switch (projFiles.Length)
                {
                    case 1:
                        args = projFiles;
                        break;

                    case 0:
                        Console.WriteLine("Project path not found. Pass one as a parameter or run this within the project directory.");
                        return;
                    default:
                        Console.WriteLine("More than one project file found. Aborting, just to be safe!");
                        return;
                }
            }

            var projectPath = Path.IsPathRooted(args[0]) ? args[0] : Path.Combine(Environment.CurrentDirectory, args[0]);
            Console.WriteLine($"Project path: {projectPath}");

            if (!File.Exists(projectPath))
            {
                Console.WriteLine("Project file doesn't exist");
                return;
            }

            var configuration = LoadConfiguration(args.Skip(1).ToArray(), projectPath);

            var services = new ServiceCollection();
            ConfigureServices(services, configuration);

            var serviceProvider = services.BuildServiceProvider();

            Run(projectPath, serviceProvider).Wait();
        }

        static Task Run(string projectPath, IServiceProvider serviceProvider)
        {
            var command = serviceProvider.GetService<GenerateCommand>();
            return command.Run(projectPath);
        }

        static IConfigurationRoot LoadConfiguration(string[] args, string projectPath)
        {
            return new ConfigurationBuilder()
                .AddJsonFile(Path.Combine(Path.GetDirectoryName(projectPath), "r4mvc.json"), optional: true)
                .AddCommandLine(args)
                .Build();
        }

        static void ConfigureServices(IServiceCollection services, IConfigurationRoot configuration)
        {
            services.AddOptions();
            services.Configure<Settings>(configuration);
            services.AddTransient(sc => sc.GetService<IOptions<Settings>>().Value);

            services.AddTransient<GenerateCommand, GenerateCommand>();

            services.AddTransient<IViewLocator, DefaultRazorViewLocator>();
            services.AddTransient<IStaticFileLocator, DefaultStaticFileLocator>();
            services.AddTransient<IFileLocator, PhysicalFileLocator>();
            services.AddTransient<IStaticFileGeneratorService, StaticFileGeneratorService>();
            services.AddTransient<IControllerRewriterService, ControllerRewriterService>();
            services.AddTransient<IControllerGeneratorService, ControllerGeneratorService>();
            services.AddTransient<IFilePersistService, FilePersistService>();
            services.AddTransient<R4MvcGeneratorService, R4MvcGeneratorService>();
        }
    }
}
