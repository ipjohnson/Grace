using System;
using System.Collections.Generic;
using System.Reflection;
using Grace.DependencyInjection;
using Grace.Factory.Impl;

namespace Grace.Factory
{
    /// <summary>
    /// Creates factories for missing interface types
    /// </summary>
    public class FactoryStrategyProvider : IMissingExportStrategyProvider
    {
        private readonly Func<Type, bool> _typeFilter;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="typeFilter"></param>
        public FactoryStrategyProvider(Func<Type, bool> typeFilter = null)
        {
            _typeFilter = typeFilter;
        }

        /// <summary>
        /// Can a given request be located using this provider
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public bool CanLocate(IInjectionScope scope, IActivationExpressionRequest request)
        {
            return request.ActivationType.GetTypeInfo().IsInterface &&
                   (_typeFilter?.Invoke(request.ActivationType) ?? true);
        }

        /// <summary>
        /// Provide exports for a missing type
        /// </summary>
        /// <param name="scope">scope to provide value</param>
        /// <param name="request">request</param>
        /// <returns>set of activation strategies</returns>
        public IEnumerable<IActivationStrategy> ProvideExports(IInjectionScope scope, IActivationExpressionRequest request)
        {
            if (request.ActivationType.GetTypeInfo().IsInterface &&
                (_typeFilter?.Invoke(request.ActivationType) ?? true))
            {
                yield return new DynamicFactoryStrategy(request.ActivationType, request.RequestingScope);
            }
        }
    }
}
