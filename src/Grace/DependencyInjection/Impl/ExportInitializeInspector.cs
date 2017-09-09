using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// Inspector that will apply an enrichment delegate to call strategies that are of a specific type
    /// </summary>
    public class ExportInitializeInspector : IActivationStrategyInspector
    {
        private readonly object _initializeDelegate;
        private readonly Type _initializeType;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="initializeDelegate"></param>
        /// <param name="initializeType"></param>
        public ExportInitializeInspector(object initializeDelegate, Type initializeType)
        {
            _initializeDelegate = initializeDelegate;
            _initializeType = initializeType;
        }

        void IActivationStrategyInspector.Inspect<T>(T strategy)
        {
            if (strategy is ICompiledExportStrategy compiledExportStrategy)
            {
                if (_initializeType.GetTypeInfo().IsAssignableFrom(strategy.ActivationType.GetTypeInfo()))
                {
                    compiledExportStrategy.EnrichmentDelegate(_initializeDelegate);
                }
            }
        }
    }
}
