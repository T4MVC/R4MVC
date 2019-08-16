using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

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
                var xdoc = XDocument.Load(csProject);
                var xmlNode = xdoc.Descendants("Content").Where(x => x.HasAttributes && x.Attribute("LinkBase") != null && x.Attribute("LinkBase").Value == ViewsFolder).SingleOrDefault();
                if (xmlNode != null)
                {
                    var includeFolder = xmlNode.Attribute("Include").Value.Replace("*", "").Replace("\\Views\\", "");
                    var newFolder = Path.GetFullPath(Path.Combine(projectRoot, includeFolder));
                    return base.Find(newFolder);
                }
            }
            return new List<View>();
        }
    }
}
