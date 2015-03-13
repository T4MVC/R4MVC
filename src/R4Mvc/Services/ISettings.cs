using System;

namespace R4Mvc.Services
{
    public interface ISettings
    {
        /// <summary>
        /// The prefix used for things like MVC.Dinners.Name and MVC.Dinners.Delete(Model.DinnerID).
        /// </summary>
        string HelpersPrefix { get; }
    }
}