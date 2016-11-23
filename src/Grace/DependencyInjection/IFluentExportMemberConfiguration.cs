using System;
using Grace.DependencyInjection.Conditions;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// Interface for configuring exporting a member
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IFluentExportMemberConfiguration<T> : IFluentExportStrategyConfiguration<T>
    {
        /// <summary>
        /// Control what type the member is exported as
        /// </summary>
        /// <param name="exportType">export type</param>
        /// <returns></returns>
        IFluentExportMemberConfiguration<T> WithType(Type exportType);

        /// <summary>
        /// Add a condition for property export
        /// </summary>
        /// <param name="condition">condition to add</param>
        /// <returns></returns>
        IFluentExportMemberConfiguration<T> WithCondition(ICompiledCondition condition);
    }
}
