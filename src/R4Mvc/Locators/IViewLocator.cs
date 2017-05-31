using System.Collections.Generic;

namespace R4Mvc.Locators
{
	public interface IViewLocator
	{
		IEnumerable<View> Find();
	}
}
