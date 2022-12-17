using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using R4Mvc.Tools.CodeGen;
using R4Mvc.Tools.Commands.Core;
using R4Mvc.Tools.Extensions;
using R4Mvc.Tools.Locators;
using R4Mvc.Tools.Services;

namespace R4Mvc.Tools.Commands
{
    public class GenerateCommand : ICommand
    {
        public bool IsGlobal => false;
        public byte Order => 1;
        public string Key => "generate";
        public string Summary => "Run the R4Mvc generator against the selected project";
        public string Description => Summary + @"
Usage: generate [options] [-p project-path]
project-path:
    Path to the project's .cshtml file";

        public Type GetCommandType() => typeof(Runner);

        public class Runner : ICommandRunner
        {
            private readonly IControllerRewriterService _controllerRewriter;
            private readonly IPageRewriterService _pageRewriter;
            private readonly IEnumerable<IViewLocator> _viewLocators;
            private readonly IEnumerable<IPageViewLocator> _pageViewLocators;
            private readonly R4MvcGeneratorService _generatorService;
            private readonly Settings _settings;
            private readonly IGeneratedFileTesterService _generatedFileTesterService;
            private readonly IFilePersistService _filePersistService;
            public Runner(IControllerRewriterService controllerRewriter, IPageRewriterService pageRewriter, IEnumerable<IViewLocator> viewLocators,
                IEnumerable<IPageViewLocator> pageViewLocators, R4MvcGeneratorService generatorService, Settings settings,
                IGeneratedFileTesterService generatedFileTesterService, IFilePersistService filePersistService)
            {
                _controllerRewriter = controllerRewriter;
                _pageRewriter = pageRewriter;
                _viewLocators = viewLocators;
                _pageViewLocators = pageViewLocators;
                _generatorService = generatorService;
                _settings = settings;
                _generatedFileTesterService = generatedFileTesterService;
                _filePersistService = filePersistService;
            }

            public async Task Run(string projectPath, IConfiguration configuration, string[] args)
            {
                var sw = Stopwatch.StartNew();

                var vsInstance = InitialiseMSBuild(configuration);
                Console.WriteLine($"Using: {vsInstance.Name} - {vsInstance.Version}");
                Console.WriteLine("Project: " + projectPath);
                Console.WriteLine();

                // Load the project and check for compilation errors
                Console.WriteLine("Creating Workspace ...");
                var workspace = MSBuildWorkspace.Create(new Dictionary<string, string> { ["IsR4MvcBuild"] = "true" });

                Console.WriteLine("Loading project ...");
                var projectRoot = Path.GetDirectoryName(projectPath);
                var project = await workspace.OpenProjectAsync(projectPath);
                if (workspace.Diagnostics.Count > 0)
                {
                    var foundErrors = false;
                    foreach (var diag in workspace.Diagnostics)
                    {
                        if (diag.Kind == WorkspaceDiagnosticKind.Failure)
                        {
                            Console.Error.WriteLine($"  {diag.Kind}: {diag.Message}");
                            foundErrors = true;
                        }
                        else
                        {
                            Console.WriteLine($"  {diag.Kind}: {diag.Message}");
                        }
                    }
                    if (foundErrors)
                    {
                        Console.Error.WriteLine("Found errors during project analysis. Aborting");
                        return;
                    }
                }

                // Prep the project Compilation object, and process the Controller public methods list
                Console.WriteLine("Compiling project ...");
                var compilation = await project.GetCompilationAsync() as CSharpCompilation;
                SyntaxNodeHelpers.PopulateControllerClassMethodNames(compilation);

                // Get MVC version
                var mvcAssembly = compilation.ReferencedAssemblyNames
                    .Where(a => a.Name == "Microsoft.AspNetCore.Mvc")
                    .FirstOrDefault();
                if (mvcAssembly != null)
                    Console.WriteLine($"Detected MVC version: {mvcAssembly.Version}");
                else
                    Console.WriteLine("Error? Failed to find MVC");
                Console.WriteLine();

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

                // Analyse the razor pages in the project (updating them to be partial), as well as locate all the view files
                var hasPagesSupport = mvcAssembly?.Version >= new Version(2, 0, 0, 0);
                IList<PageView> pages;
                if (hasPagesSupport)
                {
                    var definitions = _pageRewriter.RewritePages(compilation);
                    pages = _pageViewLocators.SelectMany(x => x.Find(projectRoot)).ToList();

                    // Some of the entries in pages can be partial views i.e. not an actual Razor Page.
                    // They will have no associated code-behind class in the project, so filter them out when fetching definitions.
                    var razorPages = pages.Where(p => p.IsRazorPage).ToList();

                    foreach (var page in razorPages)
                    {
                        page.Definition = definitions.FirstOrDefault(d => d.GetFilePath() == (page.FilePath + ".cs"));
                    }
                }
                else
                {
                    pages = new List<PageView>();
                }
                Console.WriteLine();

                // Generate the R4Mvc.generated.cs file
                _generatorService.Generate(projectRoot, controllers, pages, hasPagesSupport);

                // updating the r4mvc.json settings file
                _settings._generatedByVersion = Program.GetVersion();
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

                sw.Stop();
                Console.WriteLine();
                Console.WriteLine($"Operation completed in {sw.Elapsed}");
            }

            private VisualStudioInstance InitialiseMSBuild(IConfiguration configuration)
            {
                var instances = MSBuildLocator.QueryVisualStudioInstances().ToArray();
                if (instances.Length == 0)
                    Console.WriteLine("No Visual Studio instances found. The code generation might fail");

                var vsInstanceIndex = configuration.GetValue<int?>("vsinstance") ?? 0;
                if (vsInstanceIndex < 0 || vsInstanceIndex > instances.Length)
                {
                    Console.WriteLine("Invalid VS instance. Falling back to the default one");
                    vsInstanceIndex = 0;
                }

                VisualStudioInstance instance;
                if (vsInstanceIndex > 0)
                {
                    // Register the selected vs instance. This will cause MSBuildWorkspace to use the MSBuild installed in that instance.
                    // Note: This has to be registered *before* creating MSBuildWorkspace. Otherwise, the MEF composition used by MSBuildWorkspace will fail to compose.
                    instance = instances[vsInstanceIndex - 1];
                    MSBuildLocator.RegisterInstance(instance);
                }
                else
                {
                    // Use the default vs instance and it's MSBuild
                    instance = MSBuildLocator.RegisterDefaults();
                }

                return instance;
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
}
