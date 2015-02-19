namespace R4Mvc
{
	public interface IViewLocator : IR4MvcPlugin
	{
		View[] Find();
	}
}