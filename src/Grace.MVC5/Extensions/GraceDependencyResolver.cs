using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Grace.DependencyInjection;

namespace Grace.MVC.Extensions
{
    /// <summary>
    /// Dependency resolver for MVC
    /// </summary>
    public class GraceDependencyResolver : IDependencyResolver
    {
        private readonly IExportLocatorScope _scope;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="scope"></param>
        public GraceDependencyResolver(IExportLocatorScope scope)
        {
            _scope = scope;
        }

        /// <summary>Resolves singly registered services that support arbitrary object creation.</summary>
        /// <returns>The requested service or object.</returns>
        /// <param name="serviceType">The type of the requested service or object.</param>
        public object GetService(Type serviceType)
        {
            return _scope.Locate(serviceType);
        }

        /// <summary>Resolves multiply registered services.</summary>
        /// <returns>The requested services.</returns>
        /// <param name="serviceType">The type of the requested services.</param>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _scope.LocateAll(serviceType);
        }
    }
}
