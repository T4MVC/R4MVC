using System.Collections.Generic;

namespace R4Mvc.Tools.Locators
{
    public interface IStaticFileLocator
    {
        IEnumerable<StaticFile> Find(string staticPathRoot);
    }
}
