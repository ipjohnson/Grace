using System;
using Grace.Data;

namespace Grace.DependencyInjection
{
    public interface IExportLocatorScope : ILocatorService, IExtraDataContainer, IDisposalScope
    {
        /// <summary>
        /// Parent scope
        /// </summary>
        IExportLocatorScope Parent { get; }
        
        /// <summary>
        /// Unique id for each scope
        /// </summary>
        Guid ScopeId { get; }

        /// <summary>
        /// Name of the scope
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets a named object that can be used for locking
        /// </summary>
        /// <param name="lockName">lock name</param>
        /// <returns>lock</returns>
        object GetLockObject(string lockName);

        /// <summary>
        /// Create as a new IExportLocate scope
        /// </summary>
        /// <param name="scopeName">scope name</param>
        /// <returns>new scope</returns>
        IExportLocatorScope BeginLifetimeScope(string scopeName = "");
    }
}
