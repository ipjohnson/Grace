using System;
using System.Web.Mvc;
using System.Web.Routing;
using Grace.DependencyInjection;

namespace Grace.MVC.Extensions
{
    /// <summary>
    /// Controller activator that uses scope
    /// </summary>
    public class DisposalScopeControllerActivator : DefaultControllerFactory
    {
        private readonly IExportLocatorScope _scope;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="injectionScope"></param>
        public DisposalScopeControllerActivator(IExportLocatorScope injectionScope)
        {
            _scope = injectionScope;
        }

        /// <summary>Retrieves the controller instance for the specified request context and controller type.</summary>
        /// <returns>The controller instance.</returns>
        /// <param name="requestContext">The context of the HTTP request, which includes the HTTP context and route data.</param>
        /// <param name="controllerType">The type of the controller.</param>
        /// <exception cref="T:System.Web.HttpException">
        /// <paramref name="controllerType" /> is null.</exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="controllerType" /> cannot be assigned.</exception>
        /// <exception cref="T:System.InvalidOperationException">An instance of <paramref name="controllerType" /> cannot be created.</exception>
        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null)
            {
                return base.GetControllerInstance(requestContext, null);
            }

            return _scope.Locate(controllerType) as IController;
        }
    }
}
