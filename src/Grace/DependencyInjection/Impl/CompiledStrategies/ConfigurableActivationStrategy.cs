using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Grace.Data;
using Grace.Data.Immutable;
using Grace.DependencyInjection.Conditions;
using Grace.DependencyInjection.Lifestyle;
using Grace.Diagnostics;

namespace Grace.DependencyInjection.Impl.CompiledStrategies
{
    /// <summary>
    /// Abstract class that most strategies are based off of
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplayString,nq}", Name = "{DebuggerNameDisplayString,nq}")]
    [DebuggerTypeProxy(typeof(ConfigurableActivationStrategyDiagnostic))]
    public abstract class ConfigurableActivationStrategy : IConfigurableActivationStrategy
    {
        /// <summary>
        /// Activation configuration
        /// </summary>
        protected readonly TypeActivationConfiguration ActivationConfiguration;

        private IActivationStrategyMetadata _metadataObject;
        private ImmutableLinkedList<Type> _exportAs = ImmutableLinkedList<Type>.Empty;
        private ImmutableLinkedList<KeyValuePair<Type, object>> _exportAsKeyed = ImmutableLinkedList<KeyValuePair<Type, object>>.Empty;
        private ImmutableArray<ICompiledCondition> _conditions = ImmutableArray<ICompiledCondition>.Empty;
        private ImmutableHashTree<object, object> _metadata = ImmutableHashTree<object, object>.Empty;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="activationType">type to activate</param>
        /// <param name="injectionScope">owning injection scope</param>
        protected ConfigurableActivationStrategy(Type activationType, IInjectionScope injectionScope)
        {
            ActivationConfiguration = new TypeActivationConfiguration(activationType, this);

            InjectionScope = injectionScope;
        }

        /// <summary>
        /// Injection scope for strategy
        /// </summary>
        public IInjectionScope InjectionScope { get; }

        /// <summary>
        /// Priority for the strategy
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Type being activated
        /// </summary>
        public Type ActivationType => ActivationConfiguration.ActivationType;

        /// <summary>
        /// Type of activation strategy
        /// </summary>
        public abstract ActivationStrategyType StrategyType { get; }

        /// <summary>
        /// Export as a particular type
        /// </summary>
        public IEnumerable<Type> ExportAs => _exportAs;

        /// <summary>
        /// Export as a keyed
        /// </summary>
        public IEnumerable<KeyValuePair<Type, object>> ExportAsKeyed => _exportAsKeyed;

        /// <summary>
        /// Does the activation strategy have conditions for it's use
        /// </summary>
        public bool HasConditions => _conditions.Length > 0;

        /// <summary>
        /// Conditions for this activation strategy to be used
        /// </summary>
        public IEnumerable<ICompiledCondition> Conditions => _conditions;

        /// <summary>
        /// lifestyle associated with the strategy
        /// </summary>
        public ICompiledLifestyle Lifestyle
        {
            get { return ActivationConfiguration.Lifestyle; }
            set { ActivationConfiguration.Lifestyle = value; }
        }

        /// <summary>
        /// Disposal delegate for strategy
        /// </summary>
        public object DisposalDelegate
        {
            get { return ActivationConfiguration.DisposalDelegate; }
            set { ActivationConfiguration.DisposalDelegate = value; }
        }

        /// <summary>
        /// IS strategy externally owned
        /// </summary>
        public bool ExternallyOwned
        {
            get { return ActivationConfiguration.ExternallyOwned; }
            set { ActivationConfiguration.ExternallyOwned = value; }
        }

        /// <summary>
        /// Add metadata to strategy
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetMetadata(object key, object value)
        {
            _metadata = _metadata.Add(key, value, (o, n) => n);
        }

        /// <summary>
        /// Method that's called when the type is activated
        /// </summary>
        public MethodInjectionInfo ActivationMethod
        {
            get
            {
                return ActivationConfiguration.ActivationMethod;
            }
            set
            {
                ActivationConfiguration.ActivationMethod = value;
            }
        }

        /// <summary>
        /// Get activation configuration for strategy
        /// </summary>
        /// <param name="activationType"></param>
        /// <returns></returns>
        public virtual TypeActivationConfiguration GetActivationConfiguration(Type activationType)
        {
            return ActivationConfiguration;
        }

        /// <summary>
        /// Get the metadata for this activation strategy
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Export as a specific type
        /// </summary>
        /// <param name="exportType">type to export as</param>
        public void AddExportAs(Type exportType)
        {
            _exportAs = _exportAs.Add(exportType);
        }

        /// <summary>
        /// Export as keyed type
        /// </summary>
        /// <param name="exportType">type to export as</param>
        /// <param name="key">export key</param>
        public void AddExportAsKeyed(Type exportType, object key)
        {
            _exportAsKeyed = _exportAsKeyed.Add(new KeyValuePair<Type, object>(exportType, key));
        }

        /// <summary>
        /// Add condition for strategy
        /// </summary>
        /// <param name="condition">condition</param>
        public void AddCondition(ICompiledCondition condition)
        {
            _conditions = _conditions.Add(condition);
        }

        /// <summary>
        /// Add member injection selector
        /// </summary>
        /// <param name="selector">member selector</param>
        public void MemberInjectionSelector(IMemeberInjectionSelector selector)
        {
            ActivationConfiguration.MemberInjectionSelector(selector);
        }

        /// <summary>
        /// Add method injection info
        /// </summary>
        /// <param name="methodInjectionInfo"></param>
        public void MethodInjectionInfo(MethodInjectionInfo methodInjectionInfo)
        {
            ActivationConfiguration.MethodInjection(methodInjectionInfo);
        }

        /// <summary>
        /// Delegate to enrich strategy with
        /// </summary>
        /// <param name="enrichmentDelegate">enrichment delegate</param>
        public void EnrichmentDelegate(object enrichmentDelegate)
        {
            ActivationConfiguration.EnrichmentDelegate(enrichmentDelegate);
        }

        /// <summary>
        /// Constructor parameter
        /// </summary>
        /// <param name="info"></param>
        public void ConstructorParameter(ConstructorParameterInfo info)
        {
            ActivationConfiguration.ConstructorParameter(info);
        }


        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        // ReSharper disable once UnusedMember.Local
        private string DebuggerDisplayString
        {
            get
            {
                string returnValue = null;

                if (_exportAs.Count > 0)
                {
                    string exportName = ReflectionService.GetFriendlyNameForType(_exportAs.First());

                    returnValue = "  as  " + exportName;

                    if (_exportAs.Count > 1)
                    {
                        returnValue += " ...";
                    }
                }
                else if (_exportAsKeyed.Count > 0)
                {
                    var keyedType = _exportAsKeyed.First();
                    string typeName = ReflectionService.GetFriendlyNameForType(keyedType.Key);

                    returnValue = $"  as  {typeName}({keyedType.Value}) ";

                    if (_exportAsKeyed.Count > 1)
                    {
                        returnValue += " ...";
                    }
                }
                else
                {
                    returnValue = " as " + ReflectionService.GetFriendlyNameForType(ActivationType);
                }

                return returnValue;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        // ReSharper disable once UnusedMember.Local
        private string DebuggerNameDisplayString
        {
            get
            {
                return ReflectionService.GetFriendlyNameForType(ActivationType, true);
            }
        }

        private IActivationStrategyMetadata CreateMetadataObject()
        {
            return new ActivationStrategyMetadata(ActivationType, _exportAs, _exportAsKeyed, _metadata);
        }

    }
}
