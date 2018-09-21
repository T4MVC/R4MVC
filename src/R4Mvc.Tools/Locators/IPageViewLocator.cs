using System.Collections.Generic;

namespace R4Mvc.Tools.Locators
{
    public interface IPageViewLocator
    {
        IEnumerable<PageView> Find(string projectRoot);
    }
}
