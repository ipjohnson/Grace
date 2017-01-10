using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.WebPages;
using Grace.DependencyInjection;

namespace Grace.MVC.DependencyInjection
{

    /// <summary>
    /// C# extension class for registering different MVC types
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static class IExportRegistrationBlockExtensions
    {
        /// <summary>
        /// C# extension that exports all the classes inheriting from Controller
        /// </summary>
        /// <param name="registrationBlock"></param>
        /// <param name="types"></param>
        public static IExportTypeSetConfiguration ExportController(this IExportRegistrationBlock registrationBlock, IEnumerable<Type> types)
        {
            return registrationBlock.Export(types).
                                     BasedOn(typeof(Controller)).
                                     ExternallyOwned();
        }

        /// <summary>
        /// C# extension that export all classes that inherit from ViewPage
        /// </summary>
        /// <param name="registrationBlock">registration block</param>
        /// <param name="types">list of types</param>
        /// <returns>configuration object</returns>
	    public static IExportTypeSetConfiguration ExportView(this IExportRegistrationBlock registrationBlock,
            IEnumerable<Type> types)
        {
            return registrationBlock.Export(types).
                                     BasedOn(typeof(WebPage));
        }
    }
}
