using System;
using System.Collections.Generic;
using Grace.Data.Immutable;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl;

namespace Grace.UnitTests.Classes.FauxClasses
{
    public class FauxInjectionScope : IInjectionScope
    {
        private readonly DisposalScope disposalScope = new DisposalScope();
        private readonly Dictionary<string, object> extraData = new Dictionary<string, object>();
        private Dictionary<Type, IExportStrategyCollection> exports = new Dictionary<Type, IExportStrategyCollection>();

        public void Dispose()
        {
            disposalScope.Dispose();
        }

        public void AddDisposable(IDisposable disposable, BeforeDisposalCleanupDelegate cleanupDelegate = null)
        {
            disposalScope.AddDisposable(disposable, cleanupDelegate);
        }

        public void RemoveDisposable(IDisposable disposable)
        {
            disposalScope.RemoveDisposable(disposable);
        }

        public IDependencyInjectionContainer Container { get; set; }

        public Guid ScopeId { get; set; }

        public string ScopeName { get; set; }

        public IInjectionScope ParentScope { get; set; }

        public IEnumerable<IMissingExportStrategyProvider> MissingExportStrategyProviders
        {
            get { yield break; }
        }

        public IEnumerable<IStrategyInspector> Inspectors
        {
            get { return ImmutableArray<IStrategyInspector>.Empty; }
        }

        public void AddInspector(IStrategyInspector inspector)
        {

        }

        public void AddMissingExportStrategyProvider(IMissingExportStrategyProvider exportStrategyProvider)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IInjectionScope> ChildScopes()
        {
            return new IInjectionScope[0];
        }

        public ExportEnvironment Environment { get; set; }

        public IInjectionScope CreateChildScope(ExportRegistrationDelegate registrationDelegate = null,
            string scopeName = null,
            IDisposalScopeProvider disposalScopeProvider = null)
        {
            return new FauxInjectionScope();
        }

        public IInjectionScope CreateChildScope(IConfigurationModule configurationModule,
            string scopeName = null,
            IDisposalScopeProvider disposalScopeProvider = null)
        {
            throw new NotImplementedException();
        }

        public string Name { get; private set; }

        public void AddSecondaryLocator(ISecondaryExportLocator newLocator)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ISecondaryExportLocator> SecondaryExportLocators { get; private set; }

        public void AddStrategyInspector(IStrategyInspector inspector)
        {
            throw new NotImplementedException();
        }

        public void Configure(ExportRegistrationDelegate registrationDelegate)
        {
        }

        public void Configure(IConfigurationModule configurationModule)
        {
        }

        public IInjectionContext CreateContext(IDisposalScope disposalScope = null)
        {
            return new FauxInjectionContext();
        }

        public IEnumerable<IExportStrategy> GetAllStrategies(ExportStrategyFilter exportFilter = null)
        {
            return null;
        }

        public IExportStrategy GetStrategy(string name, IInjectionContext injectionContext = null, ExportStrategyFilter filter = null, object withKey = null)
        {
            return null;
        }

        public IExportStrategy GetStrategy(Type exportType, IInjectionContext injectionContext = null, ExportStrategyFilter filter = null, object withKey = null)
        {
            return null;
        }

        public IEnumerable<IExportStrategy> GetStrategies(string name,
            IInjectionContext injectionContext,
            ExportStrategyFilter exportFilter = null)
        {
            return ImmutableArray<IExportStrategy>.Empty;
        }

        public IEnumerable<IExportStrategy> GetStrategies(Type exportType,
            IInjectionContext injectionContext = null,
            ExportStrategyFilter exportFilter = null)
        {
            return ImmutableArray<IExportStrategy>.Empty;
        }

        public IExportStrategyCollection GetStrategyCollection(Type exportType)
        {
            IExportStrategyCollection returnValue;

            if (!exports.TryGetValue(exportType, out returnValue) && ParentScope == null)
            {
                Dictionary<Type, IExportStrategyCollection> newExports =
                    new Dictionary<Type, IExportStrategyCollection>(exports);

                returnValue = new ExportStrategyCollection(this, Environment, DependencyInjectionContainer.CompareExportStrategies);

                newExports[exportType] = returnValue;

                exports = newExports;
            }

            return returnValue;
        }

        public IExportStrategyCollection GetStrategyCollection(string exportName)
        {
            throw new NotImplementedException();
        }

        public void AddStrategy(IExportStrategy addStrategy)
        {
            IExportStrategyCollection collection;

            foreach (Type exportType in addStrategy.ExportTypes)
            {
                if (!exports.TryGetValue(exportType, out collection))
                {
                    collection = new ExportStrategyCollection(this, Environment, DependencyInjectionContainer.CompareExportStrategies);

                    exports[exportType] = collection;
                }

                collection.AddExport(addStrategy, null);
            }
        }

        public void RemoveStrategy(IExportStrategy knownStrategy)
        {
            IExportStrategyCollection collection;

            foreach (Type exporType in knownStrategy.ExportTypes)
            {
                if (exports.TryGetValue(exporType, out collection))
                {
                    collection.RemoveExport(knownStrategy, null);
                }
            }
        }

        public object GetExtraData(string dataName)
        {
            object returnValue;

            extraData.TryGetValue(dataName, out returnValue);

            return returnValue;
        }

        public void SetExtraData(string dataName, object newValue)
        {
            extraData[dataName] = newValue;
        }

        public T Locate<T>(IInjectionContext injectionContext = null, ExportStrategyFilter consider = null, object locateKey = null)
        {
            return (T)Locate(typeof(T), injectionContext, consider, locateKey);
        }

        public object Locate(Type objectType, IInjectionContext injectionContext = null, ExportStrategyFilter consider = null, object locateKey = null)
        {
            object returnValue = null;

            if (injectionContext == null)
            {
                injectionContext = new InjectionContext(this);
            }

            if (exports != null)
            {
                IExportStrategyCollection collection;

                if (exports.TryGetValue(objectType, out collection))
                {
                    returnValue = collection.Activate(null, objectType, injectionContext, consider, locateKey);
                }
            }

            if (returnValue == null && ParentScope != null)
            {
                returnValue = ParentScope.Locate(objectType, injectionContext, consider, locateKey);
            }

            return returnValue;
        }

        public object Locate(string exportName, IInjectionContext injectionContext = null, ExportStrategyFilter consider = null, object locateKey = null)
        {
            throw new NotImplementedException();
        }

        public List<T> LocateAll<T>(IInjectionContext injectionContext = null, ExportStrategyFilter consider = null, object locateKey = null, IComparer<T> comparer = null)
        {
            return new List<T>();
        }

        public List<object> LocateAll(string name, IInjectionContext injectionContext = null, ExportStrategyFilter consider = null, object locateKey = null, IComparer<object> comparer = null)
        {
            throw new NotImplementedException();
        }

        public List<object> LocateAll(Type exportType, IInjectionContext injectionContext = null, ExportStrategyFilter consider = null, object locateKey = null, IComparer<object> comparer = null)
        {
            throw new NotImplementedException();
        }

        public T LocateByKey<T, TKey>(TKey key, IInjectionContext injectionContext = null)
        {
            throw new NotImplementedException();
        }

        public object LocateByKey<TKey>(string exportName, TKey key, IInjectionContext injectionContext = null)
        {
            throw new NotImplementedException();
        }

        public object LocateByKey<TKey>(Type exportType, TKey key, IInjectionContext injectionContext = null)
        {
            throw new NotImplementedException();
        }

        public void Inject(object injectionObject, IInjectionContext injectionContext = null)
        {
            throw new NotImplementedException();
        }
    }
}