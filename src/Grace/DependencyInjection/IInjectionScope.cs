using System;
using System.Collections.Generic;
using Grace.DependencyInjection.Impl;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// Injection scope abstraction
    /// </summary>
    public interface IInjectionScope : IExportLocatorScope
    {
        /// <summary>
        /// Configure the injection scope
        /// </summary>
        /// <param name="registrationBlock"></param>
        void Configure(Action<IExportRegistrationBlock> registrationBlock);

        /// <summary>
        /// Configure with module
        /// </summary>
        /// <param name="module">configuration module</param>
        void Configure(IConfigurationModule module);

        /// <summary>
        /// Scope configuration
        /// </summary>
        IInjectionScopeConfiguration ScopeConfiguration { get; }

        /// <summary>
        /// Strategies associated with this scope
        /// </summary>
        IActivationStrategyCollectionContainer<ICompiledExportStrategy> StrategyCollectionContainer { get; }

        /// <summary>
        /// Wrappers associated with this scope
        /// </summary>
        IActivationStrategyCollectionContainer<ICompiledWrapperStrategy> WrapperCollectionContainer { get; }

        /// <summary>
        /// Decorators associated with this scope
        /// </summary>
        IActivationStrategyCollectionContainer<ICompiledDecoratorStrategy> DecoratorCollectionContainer { get; }

        /// <summary>
        /// Member
        /// </summary>
        IEnumerable<IMemberInjectionSelector> MemberInjectionSelectors { get; }

        /// <summary>
        /// List of missing export strategy providers
        /// </summary>
        IEnumerable<IMissingExportStrategyProvider> MissingExportStrategyProviders { get; }

        /// <summary>
        /// List of missing dependency expression providers
        /// </summary>
        IEnumerable<IMissingDependencyExpressionProvider> MissingDependencyExpressionProviders { get; }

        /// <summary>
        /// List of value providers that can be used during construction of linq expression
        /// </summary>
        IEnumerable<IInjectionValueProvider> InjectionValueProviders { get; }

        /// <summary>
        /// Locate an export from a child scope (for internal use)
        /// </summary>
        /// <param name="childScope">scope where the locate originated</param>
        /// <param name="disposalScope">disposal scope to use</param>
        /// <param name="type">type to locate</param>
        /// <param name="extraData">extra data passed in</param>
        /// <param name="consider">filter for strategies</param>
        /// <param name="key">key to use during locate</param>
        /// <param name="allowNull">allow null to be returned</param>
        /// <param name="isDynamic">is the lookup dynamic</param>
        /// <returns>configuration object</returns>
        object LocateFromChildScope(IExportLocatorScope childScope, IDisposalScope disposalScope, Type type, object extraData,ActivationStrategyFilter consider, object key, bool allowNull, bool isDynamic);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="childScope"></param>
        /// <param name="disposalScope"></param>
        /// <param name="name"></param>
        /// <param name="extraData"></param>
        /// <param name="consider"></param>
        /// <param name="allowNull"></param>
        /// <returns></returns>
        object LocateByNameFromChildScope(IExportLocatorScope childScope, IDisposalScope disposalScope, string name,
            object extraData, ActivationStrategyFilter consider, bool allowNull);

        /// <summary>
        /// Internal locate all method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scope"></param>
        /// <param name="disposalScope"></param>
        /// <param name="type"></param>
        /// <param name="extraData"></param>
        /// <param name="consider"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        List<T> InternalLocateAll<T>(IExportLocatorScope scope, IDisposalScope disposalScope, Type type, object extraData, ActivationStrategyFilter consider, IComparer<T> comparer);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="disposalScope"></param>
        /// <param name="exportName"></param>
        /// <param name="extraData"></param>
        /// <param name="consider"></param>
        /// <returns></returns>
        List<object> InternalLocateAllByName(IExportLocatorScope scope, IDisposalScope disposalScope,string exportName, object extraData,
            ActivationStrategyFilter consider);

            /// <summary>
        /// Creates a new child scope
        /// This is best used for long term usage, not per request scenario
        /// </summary>
        /// <param name="configure">configure scope</param>
        /// <param name="scopeName">scope name </param>
        /// <returns></returns>
        IInjectionScope CreateChildScope(Action<IExportRegistrationBlock> configure = null, string scopeName = "");

        /// <summary>
        /// Strategy compiler used for this scope
        /// </summary>
        IActivationStrategyCompiler StrategyCompiler { get; }
    }
}
