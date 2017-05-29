using System.Collections.Generic;

namespace R4Mvc.Locators
{
	public interface IStaticFileLocator
	{
		IEnumerable<StaticFile> Find();
	}
}
