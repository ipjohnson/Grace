using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Grace.Data;
using Grace.DependencyInjection.Impl;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// Extension methods for registration block
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static class IExportRegistrationBlockExtensions
    {
        /// <summary>
        /// Ups the priority of partially closed generics based on the number of closed parameters
        /// </summary>
        /// <param name="registrationBlock">registration block</param>
        public static void PrioritizePartiallyClosedGenerics(this IExportRegistrationBlock registrationBlock)
        {
            registrationBlock.AddInspector(new PartiallyClosedGenericPriorityAugmenter());
        }

        /// <summary>
        /// Prioritize types that match the provided delegate
        /// </summary>
        /// <param name="registrationBlock">registration block</param>
        /// <param name="typesThat">delegate to filter types</param>
        /// <param name="priority">priority to assign, default is 1</param>
        public static void Prioritize(this IExportRegistrationBlock registrationBlock,
            Func<Type, bool> typesThat,
            int priority = 1)
        {
            throw new NotImplementedException("TODO");
        }

        /// <summary>
        /// Register a configuration module
        /// </summary>
        /// <typeparam name="T">module type</typeparam>
        public static void RegisterModule<T>(this IExportRegistrationBlock registrationBlock) where T : IConfigurationModule, new()
        {
            registrationBlock.RegisterModule(new T());
        }

        /// <summary>
        /// Register a configuration module
        /// </summary>
        /// <typeparam name="T">module type</typeparam>
        /// <param name="registrationBlock">registration block</param>
        /// <param name="module">configuration module</param>
        public static void RegisterModule<T>(this IExportRegistrationBlock registrationBlock, T module) where T : IConfigurationModule
        {
            if (ReferenceEquals(null, module))
            {
                throw new ArgumentNullException("module");
            }

            module.Configure(registrationBlock);
        }

        /// <summary>
        /// Scans the list of types looking for IConfigurationModule types to register
        /// </summary>
        /// <param name="registrationBlock">registration block</param>
        /// <param name="types">types to scan</param>
        public static void RegisterModules(this IExportRegistrationBlock registrationBlock, IEnumerable<Type> types)
        {
            foreach (Type type in types)
            {
                if (type.GetTypeInfo().ImplementedInterfaces.Any(x => x == typeof(IConfigurationModule)))
                {
                    IConfigurationModule activatedModule = (IConfigurationModule)Activator.CreateInstance(type);

                    activatedModule.Configure(registrationBlock);
                }
            }
        }

        /// <summary>
        /// Scans the list of types looking for IConfigurationModule types to register
        /// </summary>
        /// <typeparam name="T">base module type</typeparam>
        /// <param name="registrationBlock">registration block</param>
        /// <param name="types">types to scan</param>
        public static void RegisterModules<T>(this IExportRegistrationBlock registrationBlock, IEnumerable<Type> types)
        {
            RegisterModules(registrationBlock, typeof(T), types);
        }

        /// <summary>
        /// Scans the list of types looking for IConfigurationModule types to register
        /// </summary>
        /// <param name="registrationBlock">registration block</param>
        /// <param name="baseType">base type</param>
        /// <param name="types">types to scan</param>
        public static void RegisterModules(this IExportRegistrationBlock registrationBlock, Type baseType, IEnumerable<Type> types)
        {
            foreach (Type type in types)
            {
                if (type.GetTypeInfo().ImplementedInterfaces.Any(x => x == typeof(IConfigurationModule)) &&
                    ReflectionService.CheckTypeIsBasedOnAnotherType(type,baseType))
                {
                    IConfigurationModule activatedModule = (IConfigurationModule)Activator.CreateInstance(type);

                    activatedModule.Configure(registrationBlock);
                }
            }
        }

        /// <summary>
        /// This is a short cut to registering a value as a name using the member name for exporting
        /// ExportNamedValue(() => someValue) export the value of someValue under the name someValue
        /// ExportInstance(someValue).AsName("someValue") is the long hand form
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="registrationBlock"></param>
        /// <param name="valueExpression"></param>
        /// <returns></returns>
        public static IFluentExportInstanceConfiguration<T> ExportNamedValue<T>(
            this IExportRegistrationBlock registrationBlock,
            Expression<Func<T>> valueExpression)
        {
            MemberExpression memberExpression = valueExpression.Body as MemberExpression;
            string exportName = null;

            if (memberExpression != null)
            {
                exportName = memberExpression.Member.Name;
            }

            if (exportName != null)
            {
                Func<T> func = valueExpression.Compile();

                return registrationBlock.ExportInstance((s, c) => func()).AsName(exportName);
            }

            throw new Exception("This method can only be used on members (i.e. ExportNamedValue(() => SomeProperty))");
        }

        /// <summary>
        /// Export a particular type as a particular interface
        /// </summary>
        /// <typeparam name="T">Type to export</typeparam>
        /// <typeparam name="TInterface">type to export as</typeparam>
        /// <param name="registrationBlock"></param>
        /// <returns></returns>
        public static IFluentExportStrategyConfiguration<T> ExportAs<T,TInterface>(this IExportRegistrationBlock registrationBlock) where T : TInterface
        {
            return registrationBlock.Export<T>().As<TInterface>();
        }
    }
}
