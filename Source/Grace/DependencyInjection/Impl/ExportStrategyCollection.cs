using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Grace.Logging;

namespace Grace.DependencyInjection.Impl
{
	/// <summary>
	/// A collection of exports to be used for dependency resolution
	/// </summary>
	public class ExportStrategyCollection : IExportStrategyCollection
	{
		private readonly ExportStrategyComparer comparer;
		private readonly ExportEnvironment environment;
		private readonly object exportStrategiesLock = new object();
		private readonly IInjectionScope injectionKernel;
		private readonly ILog log = Logger.GetLogger<ExportStrategyCollection>();
		private bool disposed;
		private ReadOnlyCollection<IExportStrategy> exportStrategies;
		private Dictionary<object, IExportStrategy> keyedStrategies;
		private volatile IExportStrategy primaryStrategy;

		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="injectionKernel"></param>
		/// <param name="environment"></param>
		/// <param name="comparer"></param>
		public ExportStrategyCollection(IInjectionScope injectionKernel,
			ExportEnvironment environment,
			ExportStrategyComparer comparer)
		{
			this.environment = environment;
			this.comparer = comparer;
			this.injectionKernel = injectionKernel;

			exportStrategies = new ReadOnlyCollection<IExportStrategy>(new IExportStrategy[0]);
		}

		/// <summary>
		/// A enumerable of export strategies
		/// </summary>
		public IEnumerable<IExportStrategy> ExportStrategies
		{
			get
			{
				if (keyedStrategies == null)
				{
					return exportStrategies;
				}

				var returnValue = new List<IExportStrategy>(exportStrategies);

				returnValue.AddRange(keyedStrategies.Values);

				return returnValue;

			}
		}

		public List<TLazy> ActivateAllLazy<TLazy, T>(IInjectionContext injectionContext, ExportStrategyFilter filter, object locateKey)
			where TLazy : Lazy<T>
		{
			List<TLazy> returnValue = new List<TLazy>();
			ReadOnlyCollection<IExportStrategy> localStrategies = exportStrategies;
			int count = localStrategies.Count;

			if (locateKey == null)
			{
				for (int i = 0; i < count; i++)
				{
					IExportStrategy testStrategy = localStrategies[i];

					if (testStrategy.MeetsCondition(injectionContext) &&
						 (filter == null ||
						  !testStrategy.AllowingFiltering ||
						  filter(injectionContext, testStrategy)))
					{
						IInjectionContext clonedContext = injectionContext.Clone();

						Lazy<T> lazyT = new Lazy<T>(() => (T)testStrategy.Activate(injectionKernel, clonedContext, filter, locateKey));

						returnValue.Add((TLazy)lazyT);

					}
				}
			}
			else if (keyedStrategies != null)
			{
				IEnumerable enumerable = locateKey as IEnumerable;

				if (enumerable != null && !(locateKey is string))
				{
					foreach (object key in enumerable)
					{
						IExportStrategy testStrategy;

						if (keyedStrategies.TryGetValue(key, out testStrategy))
						{
							IInjectionContext clonedContext = injectionContext.Clone();

							Lazy<T> lazyT = new Lazy<T>(() => (T)testStrategy.Activate(injectionKernel, clonedContext, filter, locateKey));

							returnValue.Add((TLazy)lazyT);

						}
					}
				}
				else
				{
					IExportStrategy testStrategy;

					if (keyedStrategies.TryGetValue(locateKey, out testStrategy))
					{
						IInjectionContext clonedContext = injectionContext.Clone();

						Lazy<T> lazyT = new Lazy<T>(() => (T)testStrategy.Activate(injectionKernel, clonedContext, filter, locateKey));

						returnValue.Add((TLazy)lazyT);

					}
				}
			}

			return returnValue;
		}

		public List<TOwned> ActivateAllOwned<TOwned, T>(IInjectionContext injectionContext, ExportStrategyFilter filter, object locateKey)
			where TOwned : Owned<T>
			where T : class
		{
			List<TOwned> returnValue = new List<TOwned>();
			ReadOnlyCollection<IExportStrategy> localStrategies = exportStrategies;
			int count = localStrategies.Count;

			if (locateKey == null)
			{
				for (int i = 0; i < count; i++)
				{
					IExportStrategy testStrategy = localStrategies[i];

					if (testStrategy.MeetsCondition(injectionContext) &&
						 (filter == null ||
						  !testStrategy.AllowingFiltering ||
						  filter(injectionContext, testStrategy)))
					{
						Owned<T> owned = new Owned<T>();

						IDisposalScope currentDisposalScope = injectionContext.DisposalScope;

						injectionContext.DisposalScope = owned;

						T activated = (T)testStrategy.Activate(injectionKernel, injectionContext, filter, locateKey);

						owned.SetValue(activated);

						returnValue.Add((TOwned)owned);

						injectionContext.DisposalScope = currentDisposalScope;

					}
				}
			}
			else if (keyedStrategies != null)
			{
				IEnumerable enumerable = locateKey as IEnumerable;

				if (enumerable != null && !(locateKey is string))
				{
					foreach (object key in enumerable)
					{
						IExportStrategy testStrategy;

						if (keyedStrategies.TryGetValue(key, out testStrategy))
						{
							Owned<T> owned = new Owned<T>();

							IDisposalScope currentDisposalScope = injectionContext.DisposalScope;

							injectionContext.DisposalScope = owned;

							T activated = (T)testStrategy.Activate(injectionKernel, injectionContext, filter, locateKey);

							owned.SetValue(activated);

							returnValue.Add((TOwned)owned);

							injectionContext.DisposalScope = currentDisposalScope;

						}
					}
				}
				else
				{
					IExportStrategy testStrategy;

					if (keyedStrategies.TryGetValue(locateKey, out testStrategy))
					{
						Owned<T> owned = new Owned<T>();

						IDisposalScope currentDisposalScope = injectionContext.DisposalScope;

						injectionContext.DisposalScope = owned;

						T activated = (T)testStrategy.Activate(injectionKernel, injectionContext, filter, locateKey);

						owned.SetValue(activated);

						returnValue.Add((TOwned)owned);

						injectionContext.DisposalScope = currentDisposalScope;
					}
				}
			}

			return returnValue;
		}

		public List<TMeta> ActivateAllMeta<TMeta, T>(IInjectionContext injectionContext, ExportStrategyFilter filter, object locateKey)
			where TMeta : Meta<T>
		{
			List<TMeta> returnValue = new List<TMeta>();
			ReadOnlyCollection<IExportStrategy> localStrategies = exportStrategies;
			int count = localStrategies.Count;

			if (locateKey == null)
			{
				for (int i = 0; i < count; i++)
				{
					IExportStrategy testStrategy = localStrategies[i];

					if (testStrategy.MeetsCondition(injectionContext) &&
						 (filter == null ||
						  !testStrategy.AllowingFiltering ||
						  filter(injectionContext, testStrategy)))
					{
						Meta<T> meta =
							new Meta<T>((T)testStrategy.Activate(injectionKernel, injectionContext, filter, locateKey),
								testStrategy.Metadata);

						returnValue.Add((TMeta)meta);
					}

				}
			}
			else if (keyedStrategies != null)
			{
				IEnumerable enumerable = locateKey as IEnumerable;

				if (enumerable != null && !(locateKey is string))
				{
					foreach (object key in enumerable)
					{
						IExportStrategy testStrategy;

						if (keyedStrategies.TryGetValue(key, out testStrategy))
						{
							Meta<T> meta =
									new Meta<T>((T)testStrategy.Activate(injectionKernel, injectionContext, filter, locateKey),
										testStrategy.Metadata);

							returnValue.Add((TMeta)meta);

						}
					}
				}
				else
				{
					IExportStrategy testStrategy;

					if (keyedStrategies.TryGetValue(locateKey, out testStrategy))
					{

						Meta<T> meta =
							new Meta<T>((T)testStrategy.Activate(injectionKernel, injectionContext, filter, locateKey),
								testStrategy.Metadata);

						returnValue.Add((TMeta)meta);
					}
				}
			}

			return returnValue;
		}

		/// <summary>
		/// Activate the first export strategy that meets conditions
		/// </summary>
		/// <param name="exportName"></param>
		/// <param name="exportType"></param>
		/// <param name="injectionContext"></param>
		/// <param name="filter"></param>
		/// <param name="locateKey"></param>
		/// <returns></returns>
		public object Activate(string exportName, Type exportType, IInjectionContext injectionContext, ExportStrategyFilter filter, object locateKey)
		{
			IExportStrategy currentPrimary = primaryStrategy;

			if (currentPrimary != null && filter == null && locateKey == null)
			{
				return currentPrimary.Activate(injectionKernel, injectionContext, null, null);
			}

			if (locateKey == null)
			{
				ReadOnlyCollection<IExportStrategy> localStrategies = exportStrategies;
				int count = localStrategies.Count;

				for (int i = 0; i < count; i++)
				{
					IExportStrategy testStrategy = localStrategies[i];

					if (testStrategy.MeetsCondition(injectionContext) &&
						 (filter == null ||
						  !testStrategy.AllowingFiltering ||
						  filter(injectionContext, testStrategy)))
					{
						return testStrategy.Activate(injectionKernel, injectionContext, filter, locateKey);
					}
				}
			}
			else if (keyedStrategies != null)
			{
				IEnumerable enumerable = locateKey as IEnumerable;

				if (enumerable != null && !(locateKey is string))
				{
					foreach (object key in enumerable)
					{
						IExportStrategy testStrategy;

						if (keyedStrategies.TryGetValue(key, out testStrategy))
						{
							return testStrategy.Activate(injectionKernel, injectionContext, filter, locateKey);
						}
					}
				}
				else
				{
					IExportStrategy testStrategy;

					if (keyedStrategies.TryGetValue(locateKey, out testStrategy))
					{
						return testStrategy.Activate(injectionKernel, injectionContext, filter, locateKey);
					}
				}
			}

			IMissingExportHandler handler = injectionKernel as IMissingExportHandler;

			if (handler != null)
			{
				return handler.LocateMissingExport(injectionContext, exportName, exportType, filter, locateKey);
			}

			return null;
		}

		/// <summary>
		/// Activate all instances of a type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="injectionContext"></param>
		/// <param name="filter"></param>
		/// <param name="locateKey"></param>
		/// <returns></returns>
		public List<T> ActivateAll<T>(IInjectionContext injectionContext, ExportStrategyFilter filter, object locateKey)
		{
			List<T> returnValue = new List<T>();
			ReadOnlyCollection<IExportStrategy> localStrategies = exportStrategies;
			int count = localStrategies.Count;

			if (locateKey == null)
			{
				for (int i = 0; i < count; i++)
				{
					IExportStrategy testStrategy = localStrategies[i];

					if (testStrategy.MeetsCondition(injectionContext) &&
						 (filter == null ||
						  !testStrategy.AllowingFiltering ||
						  filter(injectionContext, testStrategy)))
					{
						returnValue.Add((T)testStrategy.Activate(injectionKernel, injectionContext, filter, locateKey));
					}
				}
			}
			else if (keyedStrategies != null)
			{
				IEnumerable enumerable = locateKey as IEnumerable;

				if (enumerable != null && !(locateKey is string))
				{
					foreach (object key in enumerable)
					{
						IExportStrategy testStrategy;

						if (keyedStrategies.TryGetValue(key, out testStrategy))
						{
							returnValue.Add((T)testStrategy.Activate(injectionKernel, injectionContext, filter, locateKey));
						}
					}
				}
				else
				{
					IExportStrategy testStrategy;

					if (keyedStrategies.TryGetValue(locateKey, out testStrategy))
					{
						returnValue.Add((T)testStrategy.Activate(injectionKernel, injectionContext, filter, locateKey));
					}
				}
			}

			return returnValue;
		}

		/// <summary>
		/// Add an export strategy to the collection
		/// </summary>
		/// <param name="exportStrategy"></param>
		public void AddExport(IExportStrategy exportStrategy)
		{
			lock (exportStrategiesLock)
			{
				if (exportStrategy.Key == null)
				{
					if (exportStrategies.Contains(exportStrategy))
					{
						return;
					}

					List<IExportStrategy> newList = new List<IExportStrategy>(exportStrategies) { exportStrategy };

					newList.Sort((x, y) => comparer(x, y, environment));

					// I reverse the list because the sort goes from lowest to highest and it needs to be reversed
					newList.Reverse();

					exportStrategies = new ReadOnlyCollection<IExportStrategy>(newList.ToArray());

					primaryStrategy = exportStrategies[0].HasConditions ? null : exportStrategies[0];
				}
				else
				{
					Dictionary<object, IExportStrategy> newDictionary;

					if (keyedStrategies == null)
					{
						newDictionary = new Dictionary<object, IExportStrategy>();
					}
					else
					{
						newDictionary = new Dictionary<object, IExportStrategy>(keyedStrategies);
					}

					newDictionary[exportStrategy.Key] = exportStrategy;

					keyedStrategies = newDictionary;
				}
			}
		}

		/// <summary>
		/// Remove an export strategy from the collection
		/// </summary>
		/// <param name="exportStrategy"></param>
		public void RemoveExport(IExportStrategy exportStrategy)
		{
			lock (exportStrategiesLock)
			{
				if (exportStrategy.Key == null)
				{
					if (exportStrategies.Contains(exportStrategy))
					{
						List<IExportStrategy> newList = new List<IExportStrategy>(exportStrategies);

						newList.Remove(exportStrategy);

						exportStrategies = new ReadOnlyCollection<IExportStrategy>(newList);

						if (exportStrategies.Count > 0)
						{
							primaryStrategy = exportStrategies[0].HasConditions ? null : exportStrategies[0];
						}
						else
						{
							primaryStrategy = null;
						}
					}
				}
				else
				{
					if (keyedStrategies != null)
					{
						Dictionary<object, IExportStrategy> newDictionary = new Dictionary<object, IExportStrategy>(keyedStrategies);

						newDictionary.Remove(exportStrategy.Key);

						keyedStrategies = newDictionary;
					}
				}
			}
		}

		/// <summary>
		/// Dispose this collection of strategies
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		/// <summary>
		/// Dispose this collection strategies
		/// </summary>
		/// <param name="dispose"></param>
		protected void Dispose(bool dispose)
		{
			if (disposed)
			{
				return;
			}

			if (dispose)
			{
				disposed = true;

				foreach (IExportStrategy exportStrategy in exportStrategies)
				{
					if (exportStrategy.OwningScope == injectionKernel)
					{
						exportStrategy.Dispose();
					}
				}

				if (keyedStrategies != null)
				{
					foreach (KeyValuePair<object, IExportStrategy> exportStrategy in keyedStrategies)
					{
						exportStrategy.Value.Dispose();
					}
				}
			}
		}

		/// <summary>
		/// Clone the collection
		/// </summary>
		/// <param name="injectionKernel"></param>
		/// <returns></returns>
		public ExportStrategyCollection Clone(InjectionKernel injectionKernel)
		{
			return
				new ExportStrategyCollection(injectionKernel, environment, comparer)
				{
					exportStrategies = exportStrategies,
					keyedStrategies = keyedStrategies
				};
		}
	}
}