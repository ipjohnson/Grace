using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// Extensions for IInjectionContext
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static class IInjectionContextExtensions
    {
        /// <summary>
        /// Locates a type from requesting scope of the specified injection context
        /// </summary>
        /// <typeparam name="T">type to locate</typeparam>
        /// <param name="injectionContext">injection context</param>
        /// <returns>located value</returns>
        public static T LocateFromRequestingScope<T>(this IInjectionContext injectionContext)
        {
            var clone = injectionContext.Clone();

            return clone.RequestingScope.Locate<T>(clone);
        }

        /// <summary>
        /// Locates a type from requesting scope of the specified injection context
        /// </summary>
        /// <param name="injectionContext">injection context</param>
        /// <param name="type">type to locate</param>
        /// <returns>located type</returns>
        public static object LocateFromRequestingScope(this IInjectionContext injectionContext, Type type)
        {
            var clone = injectionContext.Clone();

            return clone.Locate(type);
        }
    }
}
