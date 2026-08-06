using System;
using System.Collections.Generic;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// Interface for service that tests if a type can be resolved
    /// </summary>
    public interface ICanLocateTypeService
    {
        /// <summary>
        /// Can the service be located
        /// </summary>
        /// <param name="injectionScope">injection scope</param>
        /// <param name="type">type to be located</param>
        /// <param name="filter">filter for locate</param>
        /// <param name="key">key to use for locate</param>
        /// <param name="includeProviders"></param>
        bool CanLocate(IInjectionScope injectionScope, Type type, ActivationStrategyFilter filter, object key = null, bool includeProviders = true);
    }

    /// <summary>
    /// Class tests if a type can be located
    /// </summary>
    public class CanLocateTypeService : ICanLocateTypeService
    {
        /// <summary>
        /// Can the service be located
        /// </summary>
        /// <param name="injectionScope">injection scope</param>
        /// <param name="type">type to be located</param>
        /// <param name="filter">filter for locate</param>
        /// <param name="key">key to use for locate</param>
        /// <param name="includeProviders"></param>
        public bool CanLocate(IInjectionScope injectionScope, Type type, ActivationStrategyFilter filter, object key = null, bool includeProviders = true)
        {
            var openGenericType = type.IsConstructedGenericType ? type.GetGenericTypeDefinition() : null;

            var collection = injectionScope.StrategyCollectionContainer.GetActivationStrategyCollection(type);
            collection ??= openGenericType != null 
                ? injectionScope.StrategyCollectionContainer.GetActivationStrategyCollection(openGenericType) 
                : null;

            if (key != null)
            {
                if (ReferenceEquals(key, ImportKey.Key))
                {
                    return true;
                }
                
                if (collection?.GetKeyedStrategy(key) != null)
                {
                    return true;
                }                
            }
            else
            {                
                if (collection?.GetPrimary() != null)
                {
                    return true;
                }                
            }

            var wrapperCollection = injectionScope.WrapperCollectionContainer.GetActivationStrategyCollection(type);
            wrapperCollection ??= openGenericType != null
                ? injectionScope.WrapperCollectionContainer.GetActivationStrategyCollection(openGenericType)
                : null;

            if (wrapperCollection != null)
            {
                foreach (var strategy in wrapperCollection.GetStrategies())
                {
                    var wrappedType = strategy.GetWrappedType(type);

                    if (CanLocate(injectionScope, wrappedType, filter, key))
                    {
                        return true;
                    }
                }
            }

            if (type.IsArray || openGenericType == typeof(IEnumerable<>))
            {
                return true;
            }

            if (type == typeof(ILocatorService) ||
                type == typeof(IExportLocatorScope) ||
                type == typeof(IInjectionContext) ||
                type == typeof(IDisposalScope) ||
                type == typeof(StaticInjectionContext) ||
                (type == typeof(IDisposable) && injectionScope.ScopeConfiguration.InjectIDisposable) ||
                (type == typeof(IInjectionScope) && injectionScope.ScopeConfiguration.Behaviors.AllowInjectionScopeLocation))
            {
                return true;
            }

            if (includeProviders && key == null)
            {
                var request = injectionScope.StrategyCompiler.CreateNewRequest(type, 0, injectionScope);

                foreach (var provider in injectionScope.InjectionValueProviders)
                {
                    if (provider.CanLocate(injectionScope, request))
                    {
                        return true;
                    }
                }

                foreach (var provider in injectionScope.MissingExportStrategyProviders)
                {
                    if (provider.CanLocate(injectionScope, request))
                    {
                        return true;
                    }
                }
            }

            return injectionScope.Parent?.CanLocate(type, filter, key) ?? false;
        }
    }
}
