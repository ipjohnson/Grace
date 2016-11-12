using System;
using System.Collections.Generic;

namespace Grace.DependencyInjection
{
    public interface IInjectionScope : IExportLocatorScope
    {
        /// <summary>
        /// Configure the injection scope
        /// </summary>
        /// <param name="registrationBlock"></param>
        void Configure(Action<IExportRegistrationBlock> registrationBlock);

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
        /// List of missing export strategy providers
        /// </summary>
        IEnumerable<IMissingExportStrategyProvider> MissingExportStrategyProviders { get; }

        /// <summary>
        /// List of value providers that can be used during construction of linq expression
        /// </summary>
        IEnumerable<IInjectionValueProvider> InjectionValueProviders { get; }

        /// <summary>
        /// Locate an export from a child scope
        /// </summary>
        /// <param name="childScope">scope where the locate originated</param>
        /// <param name="disposalScope">disposal scope to use</param>
        /// <param name="type">type to locate</param>
        /// <param name="extraData">extra data passed in</param>
        /// <param name="key">key to use during locate</param>
        /// <param name="allowNull">allow null to be returned</param>
        /// <returns>configuration object</returns>
        object LocateFromChildScope(IExportLocatorScope childScope, IDisposalScope disposalScope, Type type, object extraData, object key, bool allowNull);

        /// <summary>
        /// Creates a new child scope
        /// This is best used for long term usage, not per request scenario
        /// </summary>
        /// <param name="configure">configure scope</param>
        /// <param name="scopeName">scope name </param>
        /// <returns></returns>
        IInjectionScope CreateChildScope(Action<IExportRegistrationBlock> configure = null, string scopeName = "");
    }
}
