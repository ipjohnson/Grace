using System;
using System.Reflection;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// Extension methods for export strategy
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static class IFluentExportStrategyConfigurationExtentions
    {
        /// <summary>
        /// auto wire properties
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="propertyFilter"></param>
        /// <returns></returns>
        public static IFluentExportStrategyConfiguration AutoWireProperties(this IFluentExportStrategyConfiguration configuration, Func<PropertyInfo, bool> propertyFilter = null)
        {
            configuration.ImportMembers(MembersThat.AreProperty(propertyFilter));

            return configuration;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configuration"></param>
        /// <param name="propertyFilter"></param>
        /// <returns></returns>
        public static IFluentExportStrategyConfiguration<T> AutoWireProperties<T>(this IFluentExportStrategyConfiguration<T> configuration, Func<PropertyInfo, bool> propertyFilter = null)
        {
            configuration.ImportMembers(MembersThat.AreProperty(propertyFilter));

            return configuration;
        }
    }
}
