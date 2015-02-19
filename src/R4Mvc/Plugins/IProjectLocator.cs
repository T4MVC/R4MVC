using System.Collections.Generic;

namespace R4Mvc
{
	public interface IProjectLocator : IR4MvcPlugin
	{
		string GetProjectDirectory();

		IEnumerable<string> GetContentFiles();

		IEnumerable<string> GetSourceFiles();

		string GetGeneratedFilePath();
	}
}