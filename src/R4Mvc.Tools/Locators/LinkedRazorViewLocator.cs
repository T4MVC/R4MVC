using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace R4Mvc.Tools.Locators
{
    public class LinkedRazorViewLocator : DefaultRazorViewLocator
    {
        public LinkedRazorViewLocator(IFileLocator fileLocator, Settings settings) : base(fileLocator, settings)
        {
        }

        public override IEnumerable<View> Find(string projectRoot)
        {
            var csProject = Directory.EnumerateFiles(projectRoot, "*.csproj").SingleOrDefault();

            if (csProject != null)
            {
                using (var reader = XmlReader.Create(new StringReader(System.IO.File.ReadAllText(csProject))))
                {
                    while (reader.ReadToDescendant("Content"))
                    {
                        if (reader.HasAttributes && reader.GetAttribute("LinkBase") == ViewsFolder)
                        {
                            // We have to remove Views, in order to force to get to the root (real) path
                            var includeFolder = reader.GetAttribute("Include").Replace("*", "").Replace("\\Views\\", "");
                            var newFolder = Path.GetFullPath(Path.Combine(projectRoot, includeFolder));
                            return base.Find(newFolder);
                        }
                    }
                }
            }
            return null;
        }
    }
}
