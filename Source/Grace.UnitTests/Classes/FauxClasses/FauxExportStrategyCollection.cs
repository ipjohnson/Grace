using System;
using System.Collections.Generic;
using Grace.DependencyInjection;

namespace Grace.UnitTests.Classes.FauxClasses
{
	public class FauxExportStrategyCollection : IExportStrategyCollection
	{
		private readonly IInjectionScope owningScope;
		private readonly List<IExportStrategy> strategies;

		public FauxExportStrategyCollection(IInjectionScope owningScope, params IExportStrategy[] strategies)
		{
			this.owningScope = owningScope;
			this.strategies = strategies != null ? new List<IExportStrategy>(strategies) : new List<IExportStrategy>();
		}

		public IEnumerable<IExportStrategy> ExportStrategies
		{
			get { return strategies; }
		}

		public List<T> ActivateAll<T>(IInjectionContext injectionContext, ExportStrategyFilter filter)
		{
			List<T> returnValue = new List<T>();

			foreach (IExportStrategy exportStrategy in strategies)
			{
				if (exportStrategy.MeetsCondition(injectionContext) && (filter == null || filter(injectionContext, exportStrategy)))
				{
					returnValue.Add((T)exportStrategy.Activate(owningScope, injectionContext, filter));
				}
			}

			return returnValue;
		}

		public List<TLazy> ActivateAllLazy<TLazy, T>(IInjectionContext injectionContext, ExportStrategyFilter filter)
			where TLazy : Lazy<T>
		{
			throw new NotImplementedException();
		}

		public List<TOwned> ActivateAllOwned<TOwned, T>(IInjectionContext injectionContext, ExportStrategyFilter filter)
			where TOwned : Owned<T> where T : class
		{
			throw new NotImplementedException();
		}

		public List<TMeta> ActivateAllMeta<TMeta, T>(IInjectionContext injectionContext, ExportStrategyFilter filter)
			where TMeta : Meta<T>
		{
			throw new NotImplementedException();
		}

		public object Activate(string exportName,
			Type exportType,
			IInjectionContext injectionContext,
			ExportStrategyFilter filter)
		{
			foreach (IExportStrategy exportStrategy in strategies)
			{
				if (exportStrategy.MeetsCondition(injectionContext))
				{
					return exportStrategy.Activate(owningScope, injectionContext, filter);
				}
			}

			return null;
		}

		public void AddExport(IExportStrategy exportStrategy)
		{
		}

		public void RemoveExport(IExportStrategy exportStrategy)
		{
		}
	}
}