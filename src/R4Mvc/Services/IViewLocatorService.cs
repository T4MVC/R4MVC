using System.Collections.Generic;

namespace R4Mvc.Services
{
	public interface IViewLocatorService
	{
		IEnumerable<View> FindViews();
	}
}
