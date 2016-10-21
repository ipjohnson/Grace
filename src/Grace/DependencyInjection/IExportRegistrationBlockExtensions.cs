using System;
using System.Collections.Generic;
using System.Reflection;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// C# extension class for IExportRegistrationBlock
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static class IExportRegistrationBlockExtensions
    {
        /// <summary>
        /// Export types from an assembly
        /// </summary>
        /// <param name="registrationBlock"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static IExportTypeSetConfiguration ExportAssembly(this IExportRegistrationBlock registrationBlock,Assembly assembly)
        {
            return registrationBlock.Export(assembly.ExportedTypes);
        }

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
        /// Export types from a set of assemblies
        /// </summary>
        /// <param name="registrationBlock"></param>
        /// <param name="assemblies">assemblies to export</param>
        /// <returns></returns>
        public static IExportTypeSetConfiguration ExportAssemblies(this IExportRegistrationBlock registrationBlock, IEnumerable<Assembly> assemblies)
        {
            List<Type> types = new List<Type>();

            foreach (Assembly assembly in assemblies)
            {
                types.AddRange(assembly.ExportedTypes);
            }

            return registrationBlock.Export(types);
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
