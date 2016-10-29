using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// Extension class for IExportLocatorScope
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static class IExportLocatorScopeExtensions
    {
        /// <summary>
        /// Get the parent injection scope for this export locator scope
        /// </summary>
        /// <param name="scope">export locator scope</param>
        /// <returns>parent injection scope</returns>
        public static IInjectionScope GetInjectionScope(this IExportLocatorScope scope)
        {
            while (!(scope is IInjectionScope))
            {
                scope = scope.Parent;
            }

            return (IInjectionScope) scope;
        }
    }
}
