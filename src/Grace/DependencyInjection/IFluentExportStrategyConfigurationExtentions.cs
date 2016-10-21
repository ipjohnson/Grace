using System;
using System.Reflection;

namespace Grace.DependencyInjection
{
    public static class IFluentExportStrategyConfigurationExtentions
    {
        public static IFluentExportStrategyConfiguration AutoWireProperties(this IFluentExportStrategyConfiguration configuration, Func<PropertyInfo, bool> propertyFilter = null)
        {
            configuration.ImportMembers(MembersThat.AreProperty(propertyFilter));

            return configuration;
        }

        public static IFluentExportStrategyConfiguration<T> AutoWireProperties<T>(this IFluentExportStrategyConfiguration<T> configuration, Func<PropertyInfo, bool> propertyFilter = null)
        {
            configuration.ImportMembers(MembersThat.AreProperty(propertyFilter));

            return configuration;
        }
    }
}
