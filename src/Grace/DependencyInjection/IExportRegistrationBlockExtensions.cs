using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Grace.DependencyInjection.Impl;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// C# extension class for IExportRegistrationBlock
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
            var types = new List<Type>();

            foreach (var assembly in assemblies)
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
        
        /// <summary>
        /// Extension to export a list of types to a registration block
        /// </summary>
        /// <param name="types">list of types</param>
        /// <param name="registrationBlock">registration block</param>
        /// <returns>configuration object</returns>
        public static IExportTypeSetConfiguration ExportTo(this IEnumerable<Type> types, IExportRegistrationBlock registrationBlock)
        {
            return registrationBlock.Export(types);
        }
        
        /// <summary>
        /// This is a short cut to registering a value as a name using the member name for exporting
        /// ExportNamedValue(() => someValue) export the value of someValue under the name someValue
        /// ExportInstance(someValue).AsKeyed(someValue.GetType(), "someValue") is the long hand form
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="registrationBlock"></param>
        /// <param name="valueExpression"></param>
        /// <returns></returns>
        public static IFluentExportInstanceConfiguration<T> ExportNamedValue<T>(
            this IExportRegistrationBlock registrationBlock,
            Expression<Func<T>> valueExpression)
        {
            var memberExpression = valueExpression.Body as MemberExpression;
            string exportName = null;

            if (memberExpression != null)
            {
                exportName = memberExpression.Member.Name;
            }

            if (exportName != null)
            {
                var func = valueExpression.Compile();

                return registrationBlock.ExportFactory(func).AsKeyed(typeof(T), exportName);
            }

            throw new Exception("This method can only be used on members (i.e. ExportNamedValue(() => SomeProperty))");
        }

        /// <summary>
        /// Import all members of a specific type and can be filtered
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="registrationBlock"></param>
        /// <param name="filter"></param>
        public static IExportRegistrationBlock ImportMember<T>(this IExportRegistrationBlock registrationBlock, Func<MemberInfo,bool> filter = null)
        {
            registrationBlock.AddMemberInjectionSelector(new MemberInjectionSelector(typeof(T),filter));  
            
            return registrationBlock;
        }

        /// <summary>
        /// Export func with context
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="block"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static IFluentExportInstanceConfiguration<T> ExportFuncWithContext<T>(
            this IExportRegistrationBlock block,
            Func<IExportLocatorScope, StaticInjectionContext, IInjectionContext, T> func)
        {
            return block.ExportFactory(func);
        }

        /// <summary>
        /// Export only if type is not exported
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configuration"></param>
        /// <param name="type"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static IFluentExportInstanceConfiguration<T> IfNotRegistered<T>(
            this IFluentExportInstanceConfiguration<T> configuration, Type type, object key = null)
        {
            var activationStrategyProvider = configuration as IActivationStrategyProvider;

            return configuration.OnlyIf(block => !block.IsExported(type, key, activationStrategyProvider?.GetStrategy() as ICompiledExportStrategy));
        }

        /// <summary>
        /// Export only if type is not exported
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="type"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static IFluentExportStrategyConfiguration IfNotRegistered(this IFluentExportStrategyConfiguration configuration, Type type, object key = null)
        {
            var activationStrategyProvider = configuration as IActivationStrategyProvider;

            return configuration.OnlyIf(block => !block.IsExported(type, key, activationStrategyProvider?.GetStrategy() as ICompiledExportStrategy));
        }

        /// <summary>
        /// Export only if type is not exported
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configuration"></param>
        /// <param name="type"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static IFluentExportStrategyConfiguration<T> IfNotRegistered<T>(this IFluentExportStrategyConfiguration<T> configuration, Type type, object key = null)
        {
            var activationStrategyProvider = configuration as IActivationStrategyProvider;

            return configuration.OnlyIf(block => !block.IsExported(type, key, activationStrategyProvider?.GetStrategy() as ICompiledExportStrategy));
        }
    }
}
