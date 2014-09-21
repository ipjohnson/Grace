using System;
using System.Collections.Generic;
using System.Diagnostics;
using Grace.Data.Immutable;
using Grace.DependencyInjection.Conditions;
using Grace.DependencyInjection.Lifestyle;
using Grace.Diagnostics;
using Grace.Logging;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// This is the base export class, it provides method to configure itself
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplayString,nq}", Name = "{DebuggerNameDisplayString,nq}")]
    [DebuggerTypeProxy(typeof(ConfigurableExportStrategyDiagnostic))]
    public abstract class ConfigurableExportStrategy : IConfigurableExportStrategy
    {
        private ILog _log;
        private bool _allowingFiltering = true;
        protected bool _disposed;
        protected ImmutableArray<IExportCondition> _conditions;
        protected ImmutableArray<EnrichWithDelegate> _enrichWithDelegates;
        protected ImmutableArray<string> _exportNames;
        protected ImmutableArray<Type> _exportTypes;
        protected ImmutableArray<Tuple<Type, object>> _keyedExportTypes;
        protected ImmutableArray<IExportStrategy> _secondaryExports;
        protected Type _exportType;
        protected ILifestyle _lifestyle;
        private bool _locked;
        protected ExportMetadata _metadata;

        /// <summary>
        /// Default constructor takes the type to export
        /// </summary>
        /// <param name="exportType">export type</param>
        protected ConfigurableExportStrategy(Type exportType)
        {
            ActivationName = exportType.FullName;
            ActivationType = exportType;

            _exportType = exportType;

            _exportNames = ImmutableArray<string>.Empty;
            _exportTypes = ImmutableArray<Type>.Empty;
            _keyedExportTypes = ImmutableArray<Tuple<Type, object>>.Empty;
            _enrichWithDelegates = ImmutableArray<EnrichWithDelegate>.Empty;
            _secondaryExports = ImmutableArray<IExportStrategy>.Empty;
            _conditions = ImmutableArray<IExportCondition>.Empty;
            
            Environment = ExportEnvironment.Any;
        }

        /// <summary>
        /// Dispose this strategy
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Locks the export for any more changes
        /// </summary>
        public virtual void Lock()
        {
            _locked = true;
        }

        /// <summary>
        /// Activation Type for this export
        /// </summary>
        public Type ActivationType { get; private set; }

        /// <summary>
        /// Usually the type.FullName, used for blacklisting purposes
        /// </summary>
        public string ActivationName { get; private set; }

        /// <summary>
        /// When considering an export should it be filtered out.
        /// True by default, usually it's only false for special export types like Array ad List
        /// </summary>
        public virtual bool AllowingFiltering
        {
            get { return _allowingFiltering; }
            protected set { _allowingFiltering = value; }
        }

        /// <summary>
        /// Attributes associated with the export strategy. 
        /// Note: do not return null. Return an empty enumerable if there are none
        /// </summary>
        public virtual IEnumerable<Attribute> Attributes
        {
            get { return ImmutableArray<Attribute>.Empty; }
        }

        /// <summary>
        /// The scope that owns this export
        /// </summary>
        public IInjectionScope OwningScope { get; set; }

        /// <summary>
        /// Export Key
        /// </summary>
        public virtual object Key { get; private set; }

        /// <summary>
        /// Set the key value for the strategy
        /// </summary>
        /// <param name="key"></param>
        public IConfigurableExportStrategy SetKey(object key)
        {
            Key = key;

            return this;
        }

        /// <summary>
        /// Names this strategy should be known as.
        /// </summary>
        public virtual IEnumerable<string> ExportNames
        {
            get { return _exportNames; }
        }

        /// <summary>
        /// Type this strategy should be known as
        /// </summary>
        public IEnumerable<Type> ExportTypes
        {
            get
            {
                if (_exportTypes.Count == 0 && _exportNames.Count == 0)
                {
                    return new[] { _exportType };
                }

                return _exportTypes;
            }
        }

        /// <summary>
        /// List of keyed interface to export under
        /// </summary>
        public IEnumerable<Tuple<Type, object>> KeyedExportTypes
        {
            get { return _keyedExportTypes; }
        }

        /// <summary>
        /// Add an export name for strategy
        /// </summary>
        /// <param name="exportName"></param>
        public virtual IConfigurableExportStrategy AddExportName(string exportName)
        {
            if (_locked)
            {
                throw new ArgumentException("Strategy is locked can't be changed");
            }

            _exportNames = _exportNames.Add(exportName);
            
            return this;
        }

        /// <summary>
        /// Add an export type for the strategy
        /// </summary>
        /// <param name="exportType"></param>
        public virtual IConfigurableExportStrategy AddExportType(Type exportType)
        {
            if (_locked)
            {
                throw new ArgumentException("Strategy is locked can't be changed");
            }

            _exportTypes = _exportTypes.Add(exportType);

            return this;
        }

        /// <summary>
        /// Add Keyed export type
        /// </summary>
        /// <param name="exportType"></param>
        /// <param name="key"></param>
        public IConfigurableExportStrategy AddKeyedExportType(Type exportType, object key)
        {
            if (_locked)
            {
                throw new ArgumentException("Strategy is locked can't be changed");
            }

            _keyedExportTypes = _keyedExportTypes.Add(new Tuple<Type, object>(exportType, key));

            return this;
        }

        public virtual ExportEnvironment Environment { get; private set; }

        /// <summary>
        /// Set the export environment for the strategy
        /// </summary>
        /// <param name="environment"></param>
        public virtual IConfigurableExportStrategy SetEnvironment(ExportEnvironment environment)
        {
            Environment = environment;

            return this;
        }

        /// <summary>
        /// What export priority is this being exported as
        /// </summary>
        public virtual int Priority { get; private set; }

        /// <summary>
        /// Set the priority for the strategy
        /// </summary>
        /// <param name="priority"></param>
        public virtual IConfigurableExportStrategy SetPriority(int priority)
        {
            Priority = priority;

            return this;
        }

        /// <summary>
        /// Set this export to be externally owned (IDisposable will not be tracked)
        /// </summary>
        public IConfigurableExportStrategy SetExternallyOwned()
        {
            ExternallyOwned = true;

            return this;
        }

        /// <summary>
        /// ILifestyle associated with export
        /// </summary>
        public virtual ILifestyle Lifestyle
        {
            get { return _lifestyle; }
        }

        /// <summary>
        /// Set the life cycle container for the strategy
        /// </summary>
        /// <param name="container"></param>
        public virtual IConfigurableExportStrategy SetLifestyleContainer(ILifestyle container)
        {
            if (_lifestyle == null)
            {
                _lifestyle = container;
            }

            return this;
        }

        /// <summary>
        /// Initialize the strategy
        /// </summary>
        public virtual void Initialize()
        {
            Lock();
        }

        /// <summary>
        /// Add a condition to the export
        /// </summary>
        /// <param name="exportCondition"></param>
        public virtual IConfigurableExportStrategy AddCondition(IExportCondition exportCondition)
        {
            if (_locked)
            {
                throw new ArgumentException("Strategy is locked, can't be changed");
            }

            _conditions = _conditions.Add(exportCondition);

            return this;
        }

        /// <summary>
        /// Does this export meet the conditions to be used
        /// </summary>
        /// <param name="injectionContext"></param>
        /// <returns></returns>
        public virtual bool MeetsCondition(IInjectionContext injectionContext)
        {
            if (_conditions.Length == 0)
            {
                return true;
            }

            ImmutableArray<IExportCondition> currentConditions = _conditions;

            for (int i = 0; i < currentConditions.Count; i++)
            {
                if (!currentConditions[i].ConditionMeet(OwningScope, injectionContext, this))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// An export can specify it's own strategy
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<IExportStrategy> SecondaryStrategies()
        {
            return _secondaryExports;
        }

        /// <summary>
        /// Adds an enrich with delegate to the pipeline
        /// </summary>
        /// <param name="enrichWithDelegate"></param>
        public virtual void EnrichWithDelegate(EnrichWithDelegate enrichWithDelegate)
        {
            _enrichWithDelegates = _enrichWithDelegates.Add(enrichWithDelegate);
        }

        /// <summary>
        /// List of dependencies for this strategy
        /// </summary>
        public virtual IEnumerable<ExportStrategyDependency> DependsOn
        {
            get { return ImmutableArray<ExportStrategyDependency>.Empty; }
        }

        /// <summary>
        /// Metadata for export
        /// </summary>
        public IExportMetadata Metadata
        {
            get
            {
                if (_metadata != null)
                {
                    return _metadata;
                }

                return new ExportMetadata(null);
            }
        }

        /// <summary>
        /// Does this export have any conditions, this is important when setting up the strategy
        /// </summary>
        public bool HasConditions
        {
            get { return _conditions != null && _conditions.Count > 0; }
        }

        /// <summary>
        /// Are the object produced by this export externally owned
        /// </summary>
        public bool ExternallyOwned { get; private set; }

        /// <summary>
        /// Activate the export
        /// </summary>
        /// <param name="exportInjectionScope">injection scope that is activating this strategy (not always owning)</param>
        /// <param name="context">injection context</param>
        /// <param name="consider">export filter</param>
        /// <param name="locateKey">locate key</param>
        /// <returns>activated object</returns>
        public abstract object Activate(IInjectionScope exportInjectionScope, IInjectionContext context, ExportStrategyFilter consider, object locateKey);

        /// <summary>
        /// Add metadata to the export
        /// </summary>
        /// <param name="name">metadata name</param>
        /// <param name="value">metadata value</param>
        public IConfigurableExportStrategy AddMetadata(string name, object value)
        {
            if (_metadata == null)
            {
                _metadata = new ExportMetadata(Key, new Dictionary<string, object> { { name, value } });
            }

            _metadata.AddOrUpdate(name, value);

            return this;
        }

        /// <summary>
        /// Dispose of this object
        /// </summary>
        /// <param name="dispose"></param>
        protected virtual void Dispose(bool dispose)
        {
            if (_disposed)
            {
                return;
            }

            if (dispose)
            {
                ILifestyle lifestyleTemp = _lifestyle;

                _lifestyle = null;

                if (lifestyleTemp != null)
                {
                    lifestyleTemp.Dispose();
                }

                _conditions = ImmutableArray<IExportCondition>.Empty;
                _enrichWithDelegates = ImmutableArray<EnrichWithDelegate>.Empty;

                _disposed = true;
            }
        }

        /// <summary>
        /// Adds a secondary export strategy to this strategy
        /// </summary>
        /// <param name="strategy">export strategy</param>
        protected void AddSecondaryExport(IExportStrategy strategy)
        {
            _secondaryExports = _secondaryExports.Add(strategy);
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        // ReSharper disable once UnusedMember.Local
        private string DebuggerDisplayString
        {
            get
            {
                string returnValue = null;

                if (_exportNames.Count > 0)
                {
                    string exportName = _exportNames[0];

                    returnValue = "  as  " + exportName;

                    if (_exportNames.Count > 1)
                    {
                        returnValue += " ...";
                    }
                }
                else if (_exportTypes.Count > 0)
                {
                    string exportName = _exportTypes[0].FullName;

                    returnValue += "  as  " + exportName;

                    if (_exportNames.Count > 1)
                    {
                        returnValue += " ...";
                    }
                }
                else
                {
                    returnValue = " as " + ActivationType.FullName;
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
                return ActivationType.FullName;
            }
        }

        /// <summary>
        /// Logger for strategies
        /// </summary>
        protected ILog Log
        {
            get { return _log ?? (_log = Logger.GetLogger(GetType())); }
        }
    }
}