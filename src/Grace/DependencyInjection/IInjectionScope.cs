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
        /// Locate an export from a child scope
        /// </summary>
        /// <param name="childScope">scope where the locate originated</param>
        /// <param name="type">type to locate</param>
        /// <param name="extraData"></param>
        /// <param name="key"></param>
        /// <param name="allowNull"></param>
        /// <returns>configuration object</returns>
        object LocateFromChildScope(IExportLocatorScope childScope, Type type, object extraData, object key, bool allowNull);
        
    }
}
