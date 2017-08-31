using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.Setup.Configuration;
using R4Mvc.Tools.Extensions;
using R4Mvc.Tools.Services;

namespace R4Mvc.Tools.Commands
{
    public class GenerateCommand
    {
        private readonly IControllerRewriterService _controllerRewriter;
        private readonly IViewLocatorService _viewLocator;
        private readonly R4MvcGeneratorService _generatorService;
        private readonly Settings _settings;
        public GenerateCommand(IControllerRewriterService controllerRewriter, IViewLocatorService viewLocator, R4MvcGeneratorService generatorService, IOptions<Settings> settings)
        {
            _controllerRewriter = controllerRewriter;
            _viewLocator = viewLocator;
            _generatorService = generatorService;
            _settings = settings.Value;
        }

        public async Task Run(string projectPath)
        {
            var projectRoot = Path.GetDirectoryName(projectPath);
            ConfigureMSBuild();

            var workspace = MSBuildWorkspace.Create();
            var project = await workspace.OpenProjectAsync(projectPath);
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

            var compilation = await project.GetCompilationAsync() as CSharpCompilation;
            SyntaxNodeHelpers.PopulateControllerClassMethodNames(compilation);

            var controllers = _controllerRewriter.RewriteControllers(compilation);
            var allViewFiles = _viewLocator.FindViews(projectRoot);

            foreach (var view in allViewFiles)
            {
                var controller = controllers
                    .Where(c => string.Equals(c.Name, view.ControllerName, StringComparison.OrdinalIgnoreCase))
                    .Where(c => string.Equals(c.Area ?? "", view.AreaName ?? "", StringComparison.OrdinalIgnoreCase))
                    .FirstOrDefault();
                if (controller == null)
                    controllers.Add(controller = new ControllerDefinition
                    {
                        Area = view.AreaName,
                        Name = view.ControllerName,
                    });
                controller.Views.Add(view);
            }

            var allControllers = controllers.ToLookup(c => c.Area);
            var areaMap = controllers.Select(c => c.Area).Where(a => !string.IsNullOrWhiteSpace(a)).Distinct(StringComparer.OrdinalIgnoreCase).ToDictionary(a => a);
            foreach (var area in areaMap.Keys.ToArray())
                if (allControllers[""].Any(c => c.Name == area))
                    areaMap[area] = area + "Area";

            _generatorService.Generate(projectRoot, allControllers, areaMap);
        }
        private void ConfigureMSBuild()
        {
            var query = new SetupConfiguration();
            var query2 = (ISetupConfiguration2)query;

            try
            {
                if (query2.GetInstanceForCurrentProcess() is ISetupInstance2 instance)
                {
                    Environment.SetEnvironmentVariable("VSINSTALLDIR", instance.GetInstallationPath());
                    Environment.SetEnvironmentVariable("VisualStudioVersion", @"15.0");
                    return;
                }
            }
            catch { }

            var instances = new ISetupInstance[1];
            var e = query2.EnumAllInstances();
            int fetched;
            do
            {
                e.Next(1, instances, out fetched);
                if (fetched > 0)
                {
                    var instance = instances[0] as ISetupInstance2;
                    if (instance.GetInstallationVersion().StartsWith("15."))
                    {
                        Environment.SetEnvironmentVariable("VSINSTALLDIR", instance.GetInstallationPath());
                        Environment.SetEnvironmentVariable("VisualStudioVersion", @"15.0");
                        return;
                    }
                }
            }
            while (fetched > 0);
        }
    }
}
