using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using R4Mvc.Tools.CodeGen;
using R4Mvc.Tools.Extensions;
using R4Mvc.Tools.Locators;
using R4Mvc.Tools.Services;

namespace R4Mvc.Tools.Commands
{
    [Description(Description)]
    public class GenerateCommand : ICommand
    {
        public const string Summary = "Run the R4Mvc generator against the selected project";
        private const string Description = Summary + @"
Usage: generate [project-path] [options]
project-path:
    Path to the project's .cshtml file";

        private readonly IControllerRewriterService _controllerRewriter;
        private readonly IEnumerable<IViewLocator> _viewLocators;
        private readonly R4MvcGeneratorService _generatorService;
        private readonly Settings _settings;
        private readonly IGeneratedFileTesterService _generatedFileTesterService;
        private readonly IFilePersistService _filePersistService;
        private bool _debugMsBuild = false;
        public GenerateCommand(IControllerRewriterService controllerRewriter, IEnumerable<IViewLocator> viewLocators, R4MvcGeneratorService generatorService, Settings settings, IGeneratedFileTesterService generatedFileTesterService, IFilePersistService filePersistService)
        {
            _controllerRewriter = controllerRewriter;
            _viewLocators = viewLocators;
            _generatorService = generatorService;
            _settings = settings;
            _generatedFileTesterService = generatedFileTesterService;
            _filePersistService = filePersistService;
        }

        public async Task Run(string projectPath, IConfiguration configuration)
        {
            if (configuration["debugmsbuild"] != null)
                _debugMsBuild = true;
            var projectRoot = Path.GetDirectoryName(projectPath);

            InitialiseMSBuild(configuration);

            // Load the project and check for compilation errors
            var workspace = MSBuildWorkspace.Create();
            DumpMsBuildAssemblies("clean workspace");

            var project = await workspace.OpenProjectAsync(projectPath);
            DumpMsBuildAssemblies("project loaded");
            if (workspace.Diagnostics.Count > 0)
            {
                var foundErrors = false;
                foreach (var diag in workspace.Diagnostics)
                {
                    Console.Error.WriteLine($"  {diag.Kind}: {diag.Message}");
                    if (diag.Kind == Microsoft.CodeAnalysis.WorkspaceDiagnosticKind.Failure)
                        foundErrors = true;
                }
                if (foundErrors)
                    return;
            }

            // Prep the project Compilation object, and process the Controller public methods list
            var compilation = await project.GetCompilationAsync() as CSharpCompilation;
            SyntaxNodeHelpers.PopulateControllerClassMethodNames(compilation);

            // Analyse the controllers in the project (updating them to be partial), as well as locate all the view files
            var controllers = _controllerRewriter.RewriteControllers(compilation);
            var allViewFiles = _viewLocators.SelectMany(x => x.Find(projectRoot));

            // Assign view files to controllers
            foreach (var views in allViewFiles.GroupBy(v => new { v.AreaName, v.ControllerName }))
            {
                var controller = controllers
                    .Where(c => string.Equals(c.Name, views.Key.ControllerName, StringComparison.OrdinalIgnoreCase))
                    .Where(c => string.Equals(c.Area, views.Key.AreaName, StringComparison.OrdinalIgnoreCase))
                    .FirstOrDefault();
                if (controller == null)
                    controllers.Add(controller = new ControllerDefinition
                    {
                        Area = views.Key.AreaName,
                        Name = views.Key.ControllerName,
                    });
                foreach (var view in views)
                    controller.Views.Add(view);
            }

            // Generate mappings for area names, to avoid clashes with controller names
            var areaMap = GenerateAreaMap(controllers);
            foreach (var controller in controllers.Where(a => !string.IsNullOrEmpty(a.Area)))
                controller.AreaKey = areaMap[controller.Area];

            // Generate the R4Mvc.generated.cs file
            _generatorService.Generate(projectRoot, controllers);

            // updating the r4mvc.json settings file
            var r4MvcJsonFile = Path.Combine(projectRoot, Constants.R4MvcSettingsFileName);
            File.WriteAllText(r4MvcJsonFile, JsonConvert.SerializeObject(_settings, Formatting.Indented));

            // Ensuring a user customisable r4mvc.cs code file exists
            var r4MvcFile = Path.Combine(projectRoot, Constants.R4MvcFileName);
            if (!File.Exists(r4MvcFile))
                CreateR4MvcUserFile(r4MvcFile);

            // Cleanup old generated files
            var generatedFiles = Directory.GetFiles(projectRoot, "*.generated.cs", SearchOption.AllDirectories);
            foreach (var file in generatedFiles)
            {
                if (File.Exists(file.Replace(".generated.cs", ".cs")) ||
                    string.Equals(Constants.R4MvcGeneratedFileName, Path.GetFileName(file)))
                    continue;

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

        private void InitialiseMSBuild(IConfiguration configuration)
        {
            var instances = MSBuildLocator.QueryVisualStudioInstances().ToArray();
            if (!instances.Any())
                Console.WriteLine("No Visual Studio instances found.");

            Console.WriteLine("Available Visual Studio intances:");
            for (int i = 0; i < instances.Length; i++)
            {
                Console.WriteLine($"  - {i}: {instances[i].Name} - {instances[i].Version}");
                Console.WriteLine($"       {instances[i].MSBuildPath}");
                Console.WriteLine();
            }

            if (!int.TryParse(configuration["vsinstance"], out var vsInstanceIndex))
            {
                Console.WriteLine("You can manually select an instance by setting the 'vsinstance' parameter");
                vsInstanceIndex = 0;
            }
            if (vsInstanceIndex < 0 || vsInstanceIndex>=instances.Length)
            {
                Console.WriteLine("Invalid VS instance. Falling back to the first one available");
                vsInstanceIndex = 0;
            }

            // We register the first instance that we found. This will cause MSBuildWorkspace to use the MSBuild installed in that instance.
            // Note: This has to be registered *before* creating MSBuildWorkspace. Otherwise, the MEF composition used by MSBuildWorkspace will fail to compose.
            var registeredInstance = instances[vsInstanceIndex];
            MSBuildLocator.RegisterInstance(registeredInstance);

            Console.WriteLine($"Registered: {registeredInstance.Name} - {registeredInstance.Version}");
            Console.WriteLine();
        }

        private void DumpMsBuildAssemblies(string stage)
        {
            if (!_debugMsBuild)
                return;

            var domainAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            var msBuildAssemblies = domainAssemblies.Where(a => a.GetName().Name.StartsWith("Microsoft.Build") || a.GetName().Name.StartsWith("Microsoft.CodeAnalysis")).ToList();
            Console.WriteLine();
            Console.WriteLine($"MSBuild loaded assemblies (stage: {stage}): ");
            foreach (var assembly in msBuildAssemblies)
                Console.WriteLine($"  {assembly.GetName().Name}: {assembly?.GetName().Version} from {assembly?.Location}");
        }

        public IDictionary<string, string> GenerateAreaMap(IEnumerable<ControllerDefinition> controllers)
        {
            var areaMap = controllers.Select(c => c.Area).Where(a => !string.IsNullOrEmpty(a)).Distinct(StringComparer.OrdinalIgnoreCase).ToDictionary(a => a);
            foreach (var area in areaMap.Keys.ToArray())
                if (controllers.Any(c => c.Area == string.Empty && c.Name == area))
                    areaMap[area] = area + "Area";
            return areaMap;
        }

        public void CreateR4MvcUserFile(string filePath)
        {
            var result = new CodeFileBuilder(_settings, false)
                .WithMembers(new ClassBuilder("R4MvcExtensions")
                    .WithModifiers(SyntaxKind.InternalKeyword)
                    .WithComment("// Use this file to add custom extensions and helper methods to R4Mvc in your project")
                    .Build())
                .Build();
            _filePersistService.WriteFile(result, filePath);
        }
    }
}
