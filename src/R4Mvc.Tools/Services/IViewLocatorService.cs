using System.Collections.Generic;

namespace R4Mvc.Tools.Services
{
	public interface IViewLocatorService
	{
		IEnumerable<View> FindViews();
	}
}
