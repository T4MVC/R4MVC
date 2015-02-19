using System.IO;
using Microsoft.Framework.Runtime.Roslyn;
using R4Mvc.Extensions;
using R4Mvc.Services;

namespace R4Mvc
{
    public class R4MvcGenerator 
    {
        private readonly IControllerLocatorService _controllerLocator;
        private readonly IControllerRewriterService _controllerRewriter;
        private readonly IControllerGeneratorService _controllerGenerator;
        private readonly IStaticFileGeneratorService _staticFileGenerator;
        
        public const string R4MvcFileName = "R4Mvc.generated.cs";

        public R4MvcGenerator(IControllerLocatorService controllerLocator, IControllerRewriterService controllerRewriter, IControllerGeneratorService controllerGenerator, IStaticFileGeneratorService staticFileGenerator)
        {
            _controllerLocator = controllerLocator;
            _controllerRewriter = controllerRewriter;
            _controllerGenerator = controllerGenerator;
            _staticFileGenerator = staticFileGenerator;
        }

        public void Generate(CompilationContext context)
        {
            var controllers = _controllerLocator.FindControllers(context);

            _controllerRewriter.RewriteControllers(controllers);

            var generatedNode = _controllerGenerator.GenerateControllers(controllers);
            generatedNode = _staticFileGenerator.GerateStaticFiles(generatedNode);

            var generatedFileName = Path.Combine(context.Project.ProjectDirectory, R4MvcFileName);
            generatedNode.WriteFile(generatedFileName);
        }

    }
}