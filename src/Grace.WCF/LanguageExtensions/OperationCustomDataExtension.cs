using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl;

namespace Grace.WCF.LanguageExtensions
{
    /// <summary>
    /// Extension class to help store and dispose of data
    /// </summary>
    public class OperationCustomDataExtension : IExtension<OperationContext>, IDisposable
    {
        private IExportLocatorScope _locatorScope;

        /// <summary>
        /// Default constructor
        /// </summary>
        public OperationCustomDataExtension(IExportLocatorScope locatorScope)
        {
            _locatorScope = locatorScope;
        }

        /// <summary>
        /// Locator scope for service call
        /// </summary>
        public IExportLocatorScope LocatorScope => _locatorScope;

        /// <summary>
        /// Dispose of extension
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Enables an extension object to find out when it has been aggregated. Called when the extension is added to the <see cref="P:System.ServiceModel.IExtensibleObject`1.Extensions"/> property.
        /// </summary>
        /// <param name="owner">The extensible object that aggregates this extension.</param>
        public void Attach(OperationContext owner)
        {
        }

        /// <summary>
        /// Enables an object to find out when it is no longer aggregated. Called when an extension is removed from the <see cref="P:System.ServiceModel.IExtensibleObject`1.Extensions"/> property.
        /// </summary>
        /// <param name="owner">The extensible object that aggregates this extension.</param>
        public void Detach(OperationContext owner)
        {
        }

        private void Dispose(bool dispose)
        {
            if (!dispose)
            {
                return;
            }

            var localScope = _locatorScope;

            if (localScope != null &&
                Interlocked.CompareExchange(ref _locatorScope, null, _locatorScope) == localScope)
            {
                localScope.Dispose();

                GC.SuppressFinalize(this);
            }
        }
    }
}