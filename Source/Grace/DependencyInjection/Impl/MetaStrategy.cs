using System;
using System.Collections.Generic;
using System.Linq;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl
{
	/// <summary>
	/// Strategy for creating a Meta&lt;T&gt;
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class MetaStrategy<T> : IExportStrategy
	{
		/// <summary>
		/// Activate the meta strategy
		/// </summary>
		/// <param name="exportInjectionScope">injection scope</param>
		/// <param name="context">context</param>
		/// <param name="consider">consider filter</param>
		/// <param name="locateKey"></param>
		/// <returns>activated object</returns>
		public object Activate(IInjectionScope exportInjectionScope, IInjectionContext context, ExportStrategyFilter consider, object locateKey)
		{
			IExportStrategy strategy = FindExportStrategy(exportInjectionScope, context, consider);

			if (strategy != null)
			{
				T activatedObject = (T)strategy.Activate(exportInjectionScope, context, consider, locateKey);

				return new Meta<T>(activatedObject, strategy.Metadata);
			}

			return null;
		}

		/// <summary>
		/// Find a matching export recursively up the chain
		/// </summary>
		/// <param name="exportInjectionScope">injection scope to check</param>
		/// <param name="context">context</param>
		/// <param name="consider">consider filter</param>
		/// <returns>strategy</returns>
		private IExportStrategy FindExportStrategy(IInjectionScope exportInjectionScope,
			IInjectionContext context,
			ExportStrategyFilter consider)
		{
			IExportStrategy strategy =  exportInjectionScope.GetStrategy(typeof(T), context, consider);

			if (strategy == null && exportInjectionScope.ParentScope != null)
			{
				return FindExportStrategy(exportInjectionScope.ParentScope, context, consider);
			}

			return strategy;
		}

		/// <summary>
		/// Dispose of strategy
		/// </summary>
		public void Dispose()
		{
		}

		/// <summary>
		/// Activation type
		/// </summary>
		public Type ActivationType
		{
			get { return typeof(Meta<T>); }
		}

		/// <summary>
		/// Activation name
		/// </summary>
		public string ActivationName
		{
			get { return typeof(Meta<T>).FullName; }
		}

		/// <summary>
		/// allow filtering
		/// </summary>
		public bool AllowingFiltering
		{
			get { return false; }
		}

		/// <summary>
		/// attributes for export
		/// </summary>
		public IEnumerable<Attribute> Attributes
		{
			get { yield break; }
		}

		/// <summary>
		/// Owning scope
		/// </summary>
		public IInjectionScope OwningScope { get; set; }

		/// <summary>
		/// Key for strategy
		/// </summary>
		public object Key
		{
			get { return null; }
		}

		/// <summary>
		/// export names
		/// </summary>
		public IEnumerable<string> ExportNames
		{
			get { yield break; }
		}

		/// <summary>
		/// export types for strategy
		/// </summary>
		public IEnumerable<Type> ExportTypes
		{
			get { yield return typeof(Meta<T>); }
		}

		/// <summary>
		/// Export environment for strategy
		/// </summary>
		public ExportEnvironment Environment
		{
			get { return ExportEnvironment.Any; }
		}

		/// <summary>
		/// Priority for strategy
		/// </summary>
		public int Priority
		{
			get { return 0; }
		}

		/// <summary>
		/// lifestyle for strategy
		/// </summary>
		public ILifestyle Lifestyle
		{
			get { return null; }
		}

		/// <summary>
		/// conditions for strategy
		/// </summary>
		public bool HasConditions
		{
			get { return false; }
		}

		/// <summary>
		/// Externally owned is false
		/// </summary>
		public bool ExternallyOwned
		{
			get { return false; }
		}

		/// <summary>
		/// MEtadata for strategy
		/// </summary>
		public IExportMetadata Metadata
		{
			get { return new ExportMetadata(null, new Dictionary<string, object>()); }
		}

		/// <summary>
		/// Initialize strategy
		/// </summary>
		public void Initialize()
		{
		}

		/// <summary>
		/// Meets conditions
		/// </summary>
		/// <param name="injectionContext">injection context</param>
		/// <returns>true</returns>
		public bool MeetsCondition(IInjectionContext injectionContext)
		{
			return true;
		}

		/// <summary>
		/// Secondary strategy
		/// </summary>
		/// <returns>empty</returns>
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
		/// Depends on nothing
		/// </summary>
		public IEnumerable<ExportStrategyDependency> DependsOn
		{
			get { yield break;}
		}
	}
}