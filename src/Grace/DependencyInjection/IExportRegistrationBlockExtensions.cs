using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// C# extension class for IExportRegistrationBlock
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static class IExportRegistrationBlockExtensions
    {
        /// <summary>
        /// Export all types from an assembly comtaining a specific type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="registrationBlock"></param>
        /// <returns></returns>
        public static IExportTypeSetConfiguration ExportAssemblyContaining<T>(this IExportRegistrationBlock registrationBlock)
        {
            return registrationBlock.Export(typeof(T).GetTypeInfo().Assembly.ExportedTypes);
        }

        /// <summary>
        /// Export a particular type as a particular interface
        /// </summary>
        /// <typeparam name="T">Type to export</typeparam>
        /// <typeparam name="TInterface">type to export as</typeparam>
        /// <param name="registrationBlock"></param>
        /// <returns></returns>
        public static IFluentExportStrategyConfiguration<T> ExportAs<T, TInterface>(this IExportRegistrationBlock registrationBlock) where T : TInterface
        {
            return registrationBlock.Export<T>().As<TInterface>();
        }


    }
}
