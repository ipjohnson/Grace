using System;
using System.Web;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl;

namespace Grace.MVC.Extensions
{
    /// <summary>
    /// Disposal scope used for MVC5 apps
    /// </summary>
    public class MVCDisposalScopeProvider : IDisposalScopeProvider
    {
        private static readonly string _disposalScopeKey = Guid.NewGuid().ToString();

        /// <summary>
        /// Provide a disposal scope for locator
        /// </summary>
        /// <param name="locatorScope">locator scope</param>
        /// <returns>new disposal scope</returns>
        public IDisposalScope ProvideDisposalScope(IExportLocatorScope locatorScope)
        {
            return GetDisposalScopeFromHttpContext(locatorScope);
        }

        /// <summary>
        /// Get disposal scope from http context
        /// </summary>
        /// <param name="locatorScope"></param>
        /// <returns></returns>
        public static IDisposalScope GetDisposalScopeFromHttpContext(IExportLocatorScope locatorScope)
        {
            if (HttpContext.Current == null)
            {
                return locatorScope;
            }

            var disposalScope = HttpContext.Current.Items[_disposalScopeKey] as IDisposalScope;

            if (disposalScope != null)
            {
                return disposalScope;
            }

            disposalScope = new DisposalScope();

            HttpContext.Current.Items[_disposalScopeKey] = disposalScope;

            return disposalScope;
        }

        /// <summary>
        /// Dispose scope in http context
        /// </summary>
        public static void DisposeScopeInHttpContext()
        {
            var disposalScope = HttpContext.Current.Items[_disposalScopeKey] as IDisposalScope;

            disposalScope?.Dispose();
        }
    }
}
