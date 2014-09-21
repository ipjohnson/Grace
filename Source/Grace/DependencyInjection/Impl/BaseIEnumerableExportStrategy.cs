using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.Data.Immutable;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// Base abstract export strategy that is 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseIEnumerableExportStrategy<T> : IExportStrategy
    {
        /// <summary>
        /// Activate the export
        /// </summary>
        /// <param name="exportInjectionScope"></param>
        /// <param name="context"></param>
        /// <param name="consider"></param>
        /// <param name="locateKey"></param>
        /// <returns></returns>
        public object Activate(IInjectionScope exportInjectionScope, IInjectionContext context, ExportStrategyFilter consider, object locateKey)
        {
            List<T> returnList = ProcessExportsFromInjectionScope(context.RequestingScope, context, consider, locateKey, null);

            if (returnList == null)
            {
                returnList= new List<T>();
            }

            if (returnList.Count == 0)
            {
                CheckMissingExportStrategyProviders(context.RequestingScope, context, consider, locateKey, returnList);
            }

            return returnList;
        }

        /// <summary>
        /// Dispose the Export strategy
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Initialize the export, caled by the container
        /// </summary>
        public void Initialize()
        {
        }

        /// <summary>
        /// This is type that will be activated, can be used for filtering
        /// </summary>
        public abstract Type ActivationType { get; }

        /// <summary>
        /// Inner Type
        /// </summary>
        public abstract Type InnerType { get; }

        /// <summary>
        /// Usually the type.FullName, used for blacklisting purposes
        /// </summary>
        public string ActivationName
        {
            get { return ActivationType.FullName; }
        }

        /// <summary>
        /// When considering an export should it be filtered out.
        /// True by default, usually it's only false for special export types like Array ad List
        /// </summary>
        public bool AllowingFiltering
        {
            get { return false; }
        }

        /// <summary>
        /// Attributes associated with the export strategy. 
        /// Note: do not return null. Return an empty enumerable if there are none
        /// </summary>
        public IEnumerable<Attribute> Attributes
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
        public object Key
        {
            get { return null; }
        }

        /// <summary>
        /// Names this strategy should be known as.
        /// </summary>
        public IEnumerable<string> ExportNames
        {
            get { return ImmutableArray<string>.Empty; }
        }

        /// <summary>
        /// Types this strategy should be known as
        /// </summary>
        public IEnumerable<Type> ExportTypes
        {
            get
            {
                yield return ActivationType;
            }
        }

        /// <summary>
        /// List of keyed interface to export under
        /// </summary>
        public IEnumerable<Tuple<Type, object>> KeyedExportTypes
        {
            get { return ImmutableArray<Tuple<Type, object>>.Empty; }
        }

        /// <summary>
        /// What environement is this strategy being exported under.
        /// </summary>
        public ExportEnvironment Environment
        {
            get { return ExportEnvironment.Any; }
        }

        /// <summary>
        /// What export priority is this being exported as
        /// </summary>
        public int Priority
        {
            get { return -1; }
        }

        /// <summary>
        /// ILifestyle associated with export
        /// </summary>
        public ILifestyle Lifestyle
        {
            get { return null; }
        }

        /// <summary>
        /// Does this export have any conditions, this is important when setting up the strategy
        /// </summary>
        public bool HasConditions
        {
            get { return false; }
        }

        /// <summary>
        /// Are the object produced by this export externally owned
        /// </summary>
        public bool ExternallyOwned
        {
            get { return false; }
        }

        /// <summary>
        /// Does this export meet the conditions to be used
        /// </summary>
        /// <param name="injectionContext"></param>
        /// <returns></returns>
        public bool MeetsCondition(IInjectionContext injectionContext)
        {
            return true;
        }

        /// <summary>
        /// No secondary strategies
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IExportStrategy> SecondaryStrategies()
        {
            yield break;
        }

        /// <summary>
        /// No enrichment
        /// </summary>
        /// <param name="enrichWithDelegate"></param>
        public void EnrichWithDelegate(EnrichWithDelegate enrichWithDelegate)
        {
        }

        /// <summary>
        /// no dependencies
        /// </summary>
        public IEnumerable<ExportStrategyDependency> DependsOn
        {
            get { yield break; }
        }

        /// <summary>
        /// Metadata associated with this strategy
        /// </summary>
        public IExportMetadata Metadata
        {
            get { return new ExportMetadata(null); }
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            BaseIEnumerableExportStrategy<T> strategy = obj as BaseIEnumerableExportStrategy<T>;

            if (strategy != null && strategy.OwningScope == OwningScope)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return ActivationType.GetHashCode();
        }

        private List<T> ProcessExportsFromInjectionScope(IInjectionScope injectionScope,
                                                      IInjectionContext context,
                                                      ExportStrategyFilter consider,
                                                      object locateKey,
                                                      List<T> returnValue)
        {
            IExportStrategyCollection collection = injectionScope.GetStrategyCollection(InnerType, false);

            if (collection != null)
            {
                var returnList = ActivateAll(context, consider, locateKey, collection);

                if (returnValue != null)
                {
                    returnValue.AddRange(returnList);
                }
                else
                {
                    returnValue = returnList;
                }
            }
            else if (InnerType.IsConstructedGenericType)
            {
                CheckForNewGenericExports(injectionScope, context, InnerType, InnerType.GetGenericTypeDefinition());

                collection = injectionScope.GetStrategyCollection(InnerType, false);

                if (collection != null)
                {
                    var returnList = ActivateAll(context, consider, locateKey, collection);

                    if (returnValue != null)
                    {
                        returnValue.AddRange(returnList);
                    }
                    else
                    {
                        returnValue = returnList;
                    }
                }
            }

            if (injectionScope.ParentScope != null)
            {
                returnValue = ProcessExportsFromInjectionScope(injectionScope.ParentScope, context, consider, locateKey, returnValue);
            }

            return returnValue;
        }

        private void CheckForNewGenericExports(
                    IInjectionScope injectionScope,
                    IInjectionContext injectionContext,
                    Type locateType,
                    Type genericType)
        {

            IExportStrategyCollection genericCollection = injectionScope.GetStrategyCollection(genericType, false);

            if (genericCollection != null)
            {
                List<IExportStrategy> strategies = new List<IExportStrategy>(genericCollection.ExportStrategies);

                IExportStrategyCollection exportStrategyCollection = injectionScope.GetStrategyCollection(locateType, false);

                if (exportStrategyCollection != null)
                {
                    foreach (IExportStrategy exportStrategy in exportStrategyCollection.ExportStrategies)
                    {
                        ICompiledExportStrategy compiledExport = exportStrategy as ICompiledExportStrategy;

                        if (compiledExport != null && compiledExport.CreatingStrategy != null)
                        {
                            strategies.Remove(compiledExport.CreatingStrategy);
                        }
                    }
                }

                foreach (IExportStrategy exportStrategy in strategies)
                {
                    GenericExportStrategy genericExportStrategy = exportStrategy as GenericExportStrategy;

                    if (genericExportStrategy != null &&
                         genericExportStrategy.MeetsCondition(injectionContext))
                    {
                        IExportStrategy newStrategy =
                            genericExportStrategy.CreateClosedStrategy(locateType);

                        if (newStrategy != null)
                        {
                            injectionScope.AddStrategy(newStrategy);
                        }
                    }
                }
            }
        }

        private void CheckMissingExportStrategyProviders(IInjectionScope scope, IInjectionContext injectionContext, ExportStrategyFilter exportFilter, object locateKey, List<T> returnValue)
        {
            foreach (IMissingExportStrategyProvider missingExportStrategyProvider in scope.MissingExportStrategyProviders)
            {
                var strategies = missingExportStrategyProvider.ProvideExports(injectionContext,
                                                                                null,
                                                                                InnerType,
                                                                                exportFilter,
                                                                                locateKey);

                foreach (IExportStrategy exportStrategy in strategies)
                {
                    scope.AddStrategy(exportStrategy);
                }
            }

            IExportStrategyCollection collection = scope.GetStrategyCollection(InnerType);

            if (collection != null)
            {
                returnValue.AddRange(ActivateAll(injectionContext, exportFilter, locateKey, collection));
            }

            if (scope.ParentScope != null)
            {
                CheckMissingExportStrategyProviders(scope.ParentScope,
                                                    injectionContext,
                                                    exportFilter,
                                                    locateKey,
                                                    returnValue);
            }
        }

        protected abstract List<T> ActivateAll(IInjectionContext injectionContext,
            ExportStrategyFilter exportFilter,
            object locateKey,
            IExportStrategyCollection collection);
    }
}
