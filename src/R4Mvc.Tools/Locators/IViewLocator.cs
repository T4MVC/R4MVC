using System.Collections.Generic;

namespace R4Mvc.Tools.Locators
{
    public interface IViewLocator
    {
        IEnumerable<View> Find();
    }
}
