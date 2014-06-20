using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.Diagnostics;

namespace Grace.Diagnostics
{
    /// <summary>
    /// C# extension methods for IInjectionScope
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static class IInjectionScopeExtensions
    {
        /// <summary>
        /// Returns information about the injection scope
        /// </summary>
        /// <param name="injectionScope">injection scope</param>
        /// <returns>diagnostic information</returns>
        public static InjectionScopeDiagnostic Diagnose(this IInjectionScope injectionScope)
        {
            return new InjectionScopeDiagnostic(injectionScope);
        }
    }
}
