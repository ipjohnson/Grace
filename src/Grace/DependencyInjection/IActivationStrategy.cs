using System;
using System.Collections.Generic;
using Grace.Data;
using Grace.DependencyInjection.Conditions;
using Grace.DependencyInjection.Impl;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// Type of strategy, export, wrapper, or decorator
    /// </summary>
    public enum ActivationStrategyType
    {
        /// <summary>
        /// Export
        /// </summary>
        ExportStrategy,

        /// <summary>
        /// Export from framework
        /// </summary>
        FrameworkExportStrategy,

        /// <summary>
        /// Wrapper strategy
        /// </summary>
        WrapperStrategy,

        /// <summary>
        /// Decorator strategy
        /// </summary>
        DecoratorStrategy
    }

    /// <summary>
    /// Activation strategy
    /// </summary>
    public interface IActivationStrategy : IExtraDataContainer, IDisposable
    {
        /// <summary>
        /// Injection scope for strategy
        /// </summary>
        IInjectionScope InjectionScope { get; }

        /// <summary>
        /// Priority
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// Type being activated
        /// </summary>
        Type ActivationType { get; }

        /// <summary>
        /// Type of activation strategy
        /// </summary>
        ActivationStrategyType StrategyType { get; }

        /// <summary>
        /// Lifestyle for this activation stratgey
        /// </summary>
        ICompiledLifestyle Lifestyle { get; }

        /// <summary>
        /// Export as a particular type
        /// </summary>
        IEnumerable<Type> ExportAs { get; }

        /// <summary>
        /// Export as a keyed
        /// </summary>
        IEnumerable<KeyValuePair<Type, object>> ExportAsKeyed { get; }

        /// <summary>
        /// Export as a name
        /// </summary>
        IEnumerable<string> ExportAsName { get; }

        /// <summary>
        /// Does the activation strategy have conditions for it's use
        /// </summary>
        bool HasConditions { get; }

        /// <summary>
        /// Conditions for this activation strategy to be used
        /// </summary>
        IEnumerable<ICompiledCondition> Conditions { get; }

        /// <summary>
        /// Are the object produced by this export externally owned
        /// </summary>
        bool ExternallyOwned { get; }

        /// <summary>
        /// Get activation configuration for strategy
        /// </summary>
        /// <param name="activationType"></param>
        /// <returns></returns>
        TypeActivationConfiguration GetActivationConfiguration(Type activationType);

        /// <summary>
        /// Get the metadata for this activation strategy
        /// </summary>
        /// <returns></returns>
        IActivationStrategyMetadata Metadata { get; }

        /// <summary>
        /// This is a value set by the container at registration time.
        /// Do not set this yourself.
        /// </summary>
        int ExportOrder { get; set; }

        /// <summary>
        /// Dependencies needed to activate strategy given a specific request
        /// </summary>
        IEnumerable<ActivationStrategyDependency> GetDependencies(IActivationExpressionRequest request = null);
    }
}
