using System;
using Grace.DependencyInjection.Conditions;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// Configuration object for exporting a member
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FluentExportMemberConfiguration<T> : ProxyFluentExportStrategyConfiguration<T>, IFluentExportMemberConfiguration<T>
    {
        private readonly ICompiledExportStrategy _exportStrategy;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="strategy">strategy to wrap</param>
        /// <param name="exportStrategy">export strategy for member</param>
        public FluentExportMemberConfiguration(IFluentExportStrategyConfiguration<T> strategy, ICompiledExportStrategy exportStrategy) : base(strategy)
        {
            _exportStrategy = exportStrategy;
        }

        /// <summary>
        /// Control what type the member is exported as
        /// </summary>
        /// <param name="exportType">export type</param>
        /// <returns></returns>
        public IFluentExportMemberConfiguration<T> WithType(Type exportType)
        {
            _exportStrategy.AddExportAs(exportType);

            return this;
        }

        /// <summary>
        /// Add a condition for property export
        /// </summary>
        /// <param name="condition">condition to add</param>
        /// <returns></returns>
        public IFluentExportMemberConfiguration<T> WithCondition(ICompiledCondition condition)
        {
            _exportStrategy.AddCondition(condition);

            return this;
        }
    }
}
