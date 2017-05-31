namespace R4Mvc.Services
{
    public interface ISettings
    {
        /// <summary>
        /// The prefix used for things like MVC.Dinners.Name and MVC.Dinners.Delete(Model.DinnerID).
        /// </summary>
        string HelpersPrefix { get; }

        /// <summary>
        /// The namespace used by some of R4MVC's generated code.
        /// </summary>
        string R4MvcNamespace { get; }

        /// <summary>
        /// The namespace that the links are generated in (e.g. "Links", as in Links.Content.nerd_jpg).
        /// </summary>
        string LinksNamespace { get; }
    }
}
