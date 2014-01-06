using System;
using System.Collections.Generic;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl;

namespace Grace.UnitTests.Classes.FauxClasses
{
	public class FauxInjectionScope : IInjectionScope
	{
		private readonly DisposalScope disposalScope = new DisposalScope();
		private readonly Dictionary<string, object> extraData = new Dictionary<string, object>();
		private Dictionary<string, IExportStrategyCollection> exports = new Dictionary<string, IExportStrategyCollection>();

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

		public IExportStrategy GetStrategy(string name, IInjectionContext injectionContext = null)
		{
			return null;
		}

		public IExportStrategy GetStrategy(Type exportType, IInjectionContext injectionContext = null)
		{
			return null;
		}

		public IEnumerable<IExportStrategy> GetStrategies(string name,
			IInjectionContext injectionContext,
			ExportStrategyFilter exportFilter = null)
		{
			return new IExportStrategy[0];
		}

		public IEnumerable<IExportStrategy> GetStrategies(Type exportType,
			IInjectionContext injectionContext = null,
			ExportStrategyFilter exportFilter = null)
		{
			return new IExportStrategy[0];
		}

		public IExportStrategyCollection GetStrategyCollection(string exportName)
		{
			IExportStrategyCollection returnValue;

			if (!exports.TryGetValue(exportName, out returnValue) && ParentScope == null)
			{
				Dictionary<string, IExportStrategyCollection> newExports =
					new Dictionary<string, IExportStrategyCollection>(exports);

				returnValue = new ExportStrategyCollection(this, Environment, DependencyInjectionContainer.CompareExportStrategies);

				newExports[exportName] = returnValue;

				exports = newExports;
			}

			return returnValue;
		}

		public void AddStrategy(IExportStrategy addStrategy)
		{
			IExportStrategyCollection collection;

			foreach (string exportName in addStrategy.ExportNames)
			{
				if (!exports.TryGetValue(exportName, out collection))
				{
					collection = new ExportStrategyCollection(this, Environment, DependencyInjectionContainer.CompareExportStrategies);

					exports[exportName] = collection;
				}

				collection.AddExport(addStrategy);
			}
		}

		public void RemoveStrategy(IExportStrategy knownStrategy)
		{
			IExportStrategyCollection collection;

			foreach (string exportName in knownStrategy.ExportNames)
			{
				if (exports.TryGetValue(exportName, out collection))
				{
					collection.RemoveExport(knownStrategy);
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

		public T Locate<T>(IInjectionContext injectionContext = null, ExportStrategyFilter consider = null)
		{
			object returnValue = null;

			if (injectionContext == null)
			{
				injectionContext = new InjectionContext(this);
			}

			if (exports != null)
			{
				IExportStrategyCollection collection;

				if (exports.TryGetValue(typeof(T).FullName, out collection))
				{
					returnValue = (T)collection.Activate(null, typeof(T), injectionContext, consider);
				}
			}

			if (returnValue == null && ParentScope != null)
			{
				returnValue = ParentScope.Locate<T>(injectionContext, consider);
			}

			return (T)returnValue;
		}

		public object Locate(Type objectType, IInjectionContext injectionContext = null, ExportStrategyFilter consider = null)
		{
			throw new NotImplementedException();
		}

		public object Locate(string exportName,
			IInjectionContext injectionContext = null,
			ExportStrategyFilter consider = null)
		{
			throw new NotImplementedException();
		}

		public List<T> LocateAll<T>(IInjectionContext injectionContext = null, ExportStrategyFilter consider = null, IComparer<T> comparer = null)
		{
			return new List<T>();
		}

		public List<object> LocateAll(string name, IInjectionContext injectionContext = null, ExportStrategyFilter consider = null, IComparer<object> comparer = null)
		{
			throw new NotImplementedException();
		}

		public List<object> LocateAll(Type exportType, IInjectionContext injectionContext = null, ExportStrategyFilter consider = null)
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

		public void Inject(object injectionObject)
		{
			throw new NotImplementedException();
		}
	}
}