using System;
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
			get { return exportStrategies; }
		}

		public List<TLazy> ActivateAllLazy<TLazy, T>(IInjectionContext injectionContext, ExportStrategyFilter filter)
			where TLazy : Lazy<T>
		{
			List<TLazy> returnValue = new List<TLazy>();
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
					try
					{
						IInjectionContext clonedContext = injectionContext.Clone();

						Lazy<T> lazyT = new Lazy<T>(() => (T)testStrategy.Activate(injectionKernel, clonedContext, filter));

						returnValue.Add((TLazy)lazyT);
					}
					catch (Exception exp)
					{
						if (injectionKernel.Container == null ||
						    injectionKernel.Container.ThrowExceptions)
						{
							throw;
						}

						log.Error("Exception thrown while trying to activate strategy for type " + testStrategy.ActivationType.FullName,
							exp);
					}
				}
			}

			return returnValue;
		}

		public List<TOwned> ActivateAllOwned<TOwned, T>(IInjectionContext injectionContext, ExportStrategyFilter filter)
			where TOwned : Owned<T> where T : class
		{
			List<TOwned> returnValue = new List<TOwned>();
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
					try
					{
						Owned<T> owned = new Owned<T>();

						IDisposalScope currentDisposalScope = injectionContext.DisposalScope;

						injectionContext.DisposalScope = owned;

						T activated = (T)testStrategy.Activate(injectionKernel, injectionContext, filter);

						owned.SetValue(activated);

						returnValue.Add((TOwned)owned);

						injectionContext.DisposalScope = currentDisposalScope;
					}
					catch (Exception exp)
					{
						if (injectionKernel.Container == null ||
						    injectionKernel.Container.ThrowExceptions)
						{
							throw;
						}

						log.Error("Exception thrown while trying to activate strategy for type " + testStrategy.ActivationType.FullName,
							exp);
					}
				}
			}

			return returnValue;
		}

		public List<TMeta> ActivateAllMeta<TMeta, T>(IInjectionContext injectionContext, ExportStrategyFilter filter)
			where TMeta : Meta<T>
		{
			List<TMeta> returnValue = new List<TMeta>();
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
					try
					{
						Meta<T> meta =
							new Meta<T>((T)testStrategy.Activate(injectionKernel, injectionContext, filter), testStrategy.Metadata);

						returnValue.Add((TMeta)meta);
					}
					catch (Exception exp)
					{
						if (injectionKernel.Container == null ||
						    injectionKernel.Container.ThrowExceptions)
						{
							throw;
						}

						log.Error("Exception thrown while trying to activate strategy for type " + testStrategy.ActivationType.FullName,
							exp);
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
		/// <returns></returns>
		public object Activate(string exportName,
			Type exportType,
			IInjectionContext injectionContext,
			ExportStrategyFilter filter)
		{
			IExportStrategy currentPrimary = primaryStrategy;

			if (currentPrimary != null && filter == null)
			{
				return currentPrimary.Activate(injectionKernel, injectionContext, null);
			}

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
					return testStrategy.Activate(injectionKernel, injectionContext, filter);
				}
			}

			IMissingExportHandler handler = injectionKernel as IMissingExportHandler;

			if (handler != null)
			{
				return handler.LocateMissingExport(injectionContext, exportName, exportType, filter);
			}

			return null;
		}

		/// <summary>
		/// Activate all instances of a type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="injectionContext"></param>
		/// <param name="filter"></param>
		/// <returns></returns>
		public List<T> ActivateAll<T>(IInjectionContext injectionContext, ExportStrategyFilter filter)
		{
			List<T> returnValue = new List<T>();
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
					try
					{
						returnValue.Add((T)testStrategy.Activate(injectionKernel, injectionContext, filter));
					}
					catch (Exception exp)
					{
						if (injectionKernel.Container == null ||
						    injectionKernel.Container.ThrowExceptions)
						{
							throw;
						}

						log.Error("Exception thrown while trying to activate strategy for type " + testStrategy.ActivationType.FullName,
							exp);
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
		}

		/// <summary>
		/// Remove an export strategy from the collection
		/// </summary>
		/// <param name="exportStrategy"></param>
		public void RemoveExport(IExportStrategy exportStrategy)
		{
			lock (exportStrategiesLock)
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
					exportStrategies = exportStrategies
				};
		}
	}
}