using System;
using System.Web.Mvc;
using Grace.DependencyInjection;

namespace Grace.MVC.Extensions
{
    /// <summary>
    /// Activates a new view
    /// </summary>
    public class ViewPageActivator : IViewPageActivator
    {
        private readonly IExportLocatorScope _scope;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="container"></param>
        public ViewPageActivator(IExportLocatorScope container)
        {
            _scope = container;
        }

        /// <summary>Provides fine-grained control over how view pages are created using dependency injection.</summary>
        /// <returns>The created view page.</returns>
        /// <param name="controllerContext">The controller context.</param>
        /// <param name="type">The type of the controller.</param>
        public object Create(ControllerContext controllerContext, Type type)
        {
            return _scope.Locate(type);
        }
    }
}
