using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace R4Mvc.Tools
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Expecting project path as parameter");
                return;
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

            new Program().Run(projectPath, serviceProvider).Wait();
        }

        public async Task Run(string projectPath, IServiceProvider serviceProvider)
        {
            var workspace = MSBuildWorkspace.Create();
            var generator = serviceProvider.GetService<R4MvcGenerator>();
            var settings = serviceProvider.GetService<IOptions<Services.Settings>>().Value;

            var project = await workspace.OpenProjectAsync(projectPath);
            if (workspace.Diagnostics.Count > 0)
            {
                foreach (var diag in workspace.Diagnostics)
                    Console.WriteLine($"  {diag.Kind}: {diag.Message}");
                return;
            }

            var compilation = await project.GetCompilationAsync() as CSharpCompilation;
            var node = generator.Generate(compilation, settings);
            Extensions.SyntaxNodeHelpers.WriteFile(node, Path.Combine(Path.GetDirectoryName(project.FilePath), R4MvcGenerator.R4MvcFileName));
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
            Ioc.IocConfig.RegisterServices(services);
            services.AddOptions();
            services.Configure<Services.Settings>(configuration);
        }
    }
}
