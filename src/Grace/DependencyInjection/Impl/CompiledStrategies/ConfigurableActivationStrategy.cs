using System;
using System.Collections.Generic;
using System.Threading;
using Grace.Data.Immutable;
using Grace.DependencyInjection.Conditions;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl.CompiledStrategies
{
    public abstract class ConfigurableActivationStrategy : IConfigurableActivationStrategy
    {
        protected readonly TypeActivationConfiguration ActivationConfiguration;

        private IActivationStrategyMetadata _metadataObject;
        private ImmutableLinkedList<Type> _exportAs = ImmutableLinkedList<Type>.Empty;
        private ImmutableLinkedList<KeyValuePair<Type, object>> _exportAsKeyed = ImmutableLinkedList<KeyValuePair<Type, object>>.Empty;
        private ImmutableArray<ICompiledCondition> _conditions = ImmutableArray<ICompiledCondition>.Empty;
        private ImmutableHashTree<object, object> _metadata = ImmutableHashTree<object, object>.Empty;

        protected ConfigurableActivationStrategy(Type activationType, IInjectionScope injectionScope)
        {
            ActivationConfiguration = new TypeActivationConfiguration(activationType, this);

            InjectionScope = injectionScope;
        }

        public IInjectionScope InjectionScope { get; }

        public int Priority { get; set; }

        public Type ActivationType => ActivationConfiguration.ActivationType;

        public abstract ActivationStrategyType StrategyType { get; }

        public IEnumerable<Type> ExportAs => _exportAs;

        public IEnumerable<KeyValuePair<Type, object>> ExportAsKeyed => _exportAsKeyed;

        public bool HasConditions => _conditions.Length > 0;

        public IEnumerable<ICompiledCondition> Conditions => _conditions;

        public ICompiledLifestyle Lifestyle
        {
            get { return ActivationConfiguration.Lifestyle; }
            set { ActivationConfiguration.Lifestyle = value; }
        }

        public object DisposalDelegate
        {
            get { return ActivationConfiguration.DisposalDelegate; }
            set { ActivationConfiguration.DisposalDelegate = value; }
        }

        public bool ExternallyOwned
        {
            get { return ActivationConfiguration.ExternallyOwned; }
            set { ActivationConfiguration.ExternallyOwned = value; }
        }

        public void SetMetadata(object key, object value)
        {
            _metadata = _metadata.Add(key, value, (o, n) => n);
        }

        public virtual TypeActivationConfiguration GetActivationConfiguration(Type activationType)
        {
            return ActivationConfiguration;
        }
        
        public IActivationStrategyMetadata Metadata
        {
            get
            {
                if (_metadataObject == null)
                {
                    Interlocked.CompareExchange(ref _metadataObject, CreateMetadataObject(), null);
                }

                return _metadataObject;
            }
        }

        private IActivationStrategyMetadata CreateMetadataObject()
        {
            return new ActivationStrategyMetadata(ActivationType, _exportAs, _exportAsKeyed, _metadata);
        }

        public void AddExportAs(Type exportType)
        {
            _exportAs = _exportAs.Add(exportType);
        }

        public void AddExportAsKeyed(Type exportType, object key)
        {
            _exportAsKeyed = _exportAsKeyed.Add(new KeyValuePair<Type, object>(exportType, key));
        }

        public void AddCondition(ICompiledCondition condition)
        {
            _conditions = _conditions.Add(condition);
        }
        
        public void MemberInjectionSelector(IMemeberInjectionSelector selector)
        {
            ActivationConfiguration.MemberInjectionSelector(selector);
        }

        public void EnrichmentDelegate(object enrichmentDelegate)
        {
            ActivationConfiguration.EnrichmentDelegate(enrichmentDelegate);
        }

        public void ConstructorParameter(ConstructorParameterInfo info)
        {
            ActivationConfiguration.ConstructorParameter(info);
        }

    }
}
