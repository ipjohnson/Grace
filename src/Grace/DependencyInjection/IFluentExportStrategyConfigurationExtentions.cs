using System;
using System.Linq.Expressions;
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
        
        /// <summary>
        /// This is intended to be a short cut for setting named property values
        /// The expression will be inspected and the value will used by the property name
        /// WithNameCtorValue(() => someLocalVariable) will export the value under the name someLocalVariable
        /// WithCtorParam(() => someLocalVariable).Named("someLocalVariable") is the long hand form
        /// </summary>
        /// <typeparam name="TValue">value type being used</typeparam>
        /// <param name="strategy">export strategy</param>
        /// <param name="valueExpression">value expression, the name of the parameter will be used as the parameter name</param>
        /// <returns>configuration object</returns>
        public static IFluentWithCtorConfiguration<TValue> WithNamedCtorValue<TValue>(this IFluentExportStrategyConfiguration strategy, Expression<Func<TValue>> valueExpression)
        {
            var memberExpression = valueExpression.Body as MemberExpression;
            string exportName = null;

            if (memberExpression != null)
            {
                exportName = memberExpression.Member.Name;
            }

            if (exportName != null)
            {
                Func<TValue> func = valueExpression.Compile().Invoke;

                return strategy.WithCtorParam(func).Named(memberExpression.Member.Name);
            }

            throw new Exception("WithNamedCtorValue must be passed a Func that references a member");
        }

        /// <summary>
        /// This is intended to be a short cut for setting named property values
        /// The expression will be inspected and the value will used by the property name
        /// WithNameCtorValue(() => someLocalVariable) will export the value under the name someLocalVariable
        /// </summary>
        /// <typeparam name="T">Type being exported</typeparam>
        /// <typeparam name="TValue">value type being used</typeparam>
        /// <param name="strategy">export strategy</param>
        /// <param name="valueExpression">value expression, the name of the parameter will be used as the parameter name</param>
        /// <returns>configuration object</returns>
        public static IFluentWithCtorConfiguration<T, TValue> WithNamedCtorValue<T, TValue>(this IFluentExportStrategyConfiguration<T> strategy, Expression<Func<TValue>> valueExpression)
        {
            var memberExpression = valueExpression.Body as MemberExpression;
            string exportName = null;

            if (memberExpression != null)
            {
                exportName = memberExpression.Member.Name;
            }

            if (exportName != null)
            {
                Func<TValue> func = valueExpression.Compile().Invoke;

                return strategy.WithCtorParam(func).Named(memberExpression.Member.Name);
            }

            throw new Exception("WithNamedCtorValue must be passed a Func that references a member");
        }

    }
}
