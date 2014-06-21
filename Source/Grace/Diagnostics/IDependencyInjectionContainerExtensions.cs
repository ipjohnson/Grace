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
    /// Dependency Injection Container extensions
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static class IDependencyInjectionContainerExtensions
    {
        /// <summary>
        /// Returns diagnostic information about the container by resolving a DependencyInjectionContainerDiagnostic object
        /// </summary>
        /// <param name="container">container</param>
        /// <returns>diagnostic information</returns>
        public static DependencyInjectionContainerDiagnostic Diagnose(this IDependencyInjectionContainer container)
        {
            return new DependencyInjectionContainerDiagnostic(container);
        }
    }
}
