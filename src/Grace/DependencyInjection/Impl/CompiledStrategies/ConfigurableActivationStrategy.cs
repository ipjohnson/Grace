using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using Grace.Data;
using Grace.Data.Immutable;
using Grace.DependencyInjection.Conditions;
using Grace.DependencyInjection.Impl.Expressions;
using Grace.DependencyInjection.Lifestyle;
using Grace.Diagnostics;

namespace Grace.DependencyInjection.Impl.CompiledStrategies
{
    /// <summary>
    /// Abstract class that most strategies are based off of
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplayString) + ",nq}", Name = "{DebuggerNameDisplayString,nq}")]
    [DebuggerTypeProxy(typeof(ConfigurableActivationStrategyDebuggerView))]
    public abstract class ConfigurableActivationStrategy : IConfigurableActivationStrategy
    {
        /// <summary>
        /// Activation configuration
        /// </summary>
        protected readonly TypeActivationConfiguration ActivationConfiguration;

        private IActivationStrategyMetadata _metadataObject;
        private ImmutableLinkedList<Type> _exportAs = ImmutableLinkedList<Type>.Empty;
        private ImmutableLinkedList<KeyValuePair<Type, object>> _exportAsKeyed = ImmutableLinkedList<KeyValuePair<Type, object>>.Empty;
        private ImmutableLinkedList<string> _exportAsName = ImmutableLinkedList<string>.Empty;
        private ImmutableArray<ICompiledCondition> _conditions = ImmutableArray<ICompiledCondition>.Empty;
        private ImmutableHashTree<object, object> _metadata = ImmutableHashTree<object, object>.Empty;
        private ImmutableHashTree<object,object> _extraData = ImmutableHashTree<object, object>.Empty;

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
        /// Dispose of strategy
        /// </summary>
        public virtual void Dispose()
        {
            var disposable = Lifestyle as IDisposable;

            disposable?.Dispose();

            Lifestyle = null;
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
        /// Export as a name
        /// </summary>
        public IEnumerable<string> ExportAsName => _exportAsName;

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
            get => ActivationConfiguration.Lifestyle;
            set => ActivationConfiguration.Lifestyle = value;
        }

        /// <summary>
        /// Disposal delegate for strategy
        /// </summary>
        public object DisposalDelegate
        {
            get => ActivationConfiguration.DisposalDelegate;
            set => ActivationConfiguration.DisposalDelegate = value;
        }

        /// <summary>
        /// Constructor selection method
        /// </summary>
        public IConstructorExpressionCreator ConstructorSelectionMethod
        {
            get => ActivationConfiguration.ConstructorSelectionMethod;
            set => ActivationConfiguration.ConstructorSelectionMethod = value;
        }

        /// <summary>
        /// IS strategy externally owned
        /// </summary>
        public bool ExternallyOwned
        {
            get => ActivationConfiguration.ExternallyOwned;
            set => ActivationConfiguration.ExternallyOwned = value;
        }

        /// <summary>
        /// Add metadata to strategy
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetMetadata(object key, object value)
        {
            _metadata = _metadata.Add(key, value, (o, n) => n);

            _metadataObject = null;
        }

        /// <summary>
        /// Method that's called when the type is activated
        /// </summary>
        public MethodInjectionInfo ActivationMethod
        {
            get => ActivationConfiguration.ActivationMethod;
            set => ActivationConfiguration.ActivationMethod = value;
        }

        /// <summary>
        /// If not null then instance will be created in new scope.
        /// </summary>
        public string CustomScopeName
        {
            get => ActivationConfiguration.CustomScopeName;
            set => ActivationConfiguration.CustomScopeName = value;
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
        /// Order assigned by the container
        /// </summary>
        public int ExportOrder { get; set; }

        /// <summary>
        /// Dependencies needed to activate strategy
        /// </summary>
        public virtual IEnumerable<ActivationStrategyDependency> GetDependencies(IActivationExpressionRequest request)
        {
            return ImmutableLinkedList<ActivationStrategyDependency>.Empty;
        }

        /// <summary>
        /// Export as a specific type
        /// </summary>
        /// <param name="exportType">type to export as</param>
        public void AddExportAs(Type exportType)
        {
            if (_exportAs == ImmutableLinkedList<Type>.Empty ||
                !_exportAs.Contains(exportType))
            {
                _exportAs = _exportAs.Add(exportType);
            }
        }

        /// <summary>
        /// Export as keyed type
        /// </summary>
        /// <param name="exportType">type to export as</param>
        /// <param name="key">export key</param>
        public void AddExportAsKeyed(Type exportType, object key)
        {
            var kvp = new KeyValuePair<Type, object>(exportType, key);

            if (_exportAsKeyed == ImmutableLinkedList<KeyValuePair<Type, object>>.Empty ||
                !_exportAsKeyed.Contains(kvp))
            {
                _exportAsKeyed = _exportAsKeyed.Add(kvp);
            }
        }

        /// <summary>
        /// Export as name
        /// </summary>
        /// <param name="name"></param>
        public void AddExportAsName(string name)
        {
            _exportAsName = _exportAsName.Add(name);
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
        public void MemberInjectionSelector(IMemberInjectionSelector selector)
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

        /// <summary>
        /// Constructor to use when creating
        /// </summary>
        public ConstructorInfo SelectedConstructor
        {
            get => ActivationConfiguration.SelectedConstructor;
            set => ActivationConfiguration.SelectedConstructor = value;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        // ReSharper disable once UnusedMember.Local
        private string DebuggerDisplayString
        {
            get
            {
                string returnValue;

                if (_exportAs.Count > 0)
                {
                    var exportName = ReflectionService.GetFriendlyNameForType(_exportAs.First());

                    returnValue = "  as  " + exportName;

                    if (_exportAs.Count > 1)
                    {
                        returnValue += " ...";
                    }
                }
                else if (_exportAsKeyed.Count > 0)
                {
                    var keyedType = _exportAsKeyed.First();
                    var typeName = ReflectionService.GetFriendlyNameForType(keyedType.Key);

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
        private string DebuggerNameDisplayString => ReflectionService.GetFriendlyNameForType(ActivationType, true);

        private IActivationStrategyMetadata CreateMetadataObject()
        {
            return new ActivationStrategyMetadata(ActivationType, _exportAs, _exportAsKeyed, _metadata);
        }

        /// <summary>
        /// Keys for data
        /// </summary>
        public IEnumerable<object> Keys => _extraData.Keys;

        /// <summary>
        /// Values for data
        /// </summary>
        public IEnumerable<object> Values => _extraData.Values;

        /// <summary>
        /// Enumeration of all the key value pairs
        /// </summary>
        public IEnumerable<KeyValuePair<object, object>> KeyValuePairs => _extraData;

        /// <summary>
        /// Extra data associated with the injection request. 
        /// </summary>
        /// <param name="key">key of the data object to get</param>
        /// <returns>data value</returns>
        public object GetExtraData(object key)
        {
            return _extraData.GetValueOrDefault(key);
        }

        /// <summary>
        /// Sets extra data on the injection context
        /// </summary>
        /// <param name="key">object name</param>
        /// <param name="newValue">new object value</param>
        /// <param name="replaceIfExists">replace value if key exists</param>
        /// <returns>the final value of key</returns>
        public object SetExtraData(object key, object newValue, bool replaceIfExists = true)
        {
            return ImmutableHashTree.ThreadSafeAdd(ref _extraData, key, newValue, replaceIfExists);
        }
    }
}
