using System;
using System.Collections.Generic;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl.DelegateFactory
{
	/// <summary>
	/// Base class to be used to import generic delegates
	/// </summary>
	public abstract class BaseGenericDelegateExportStrategy : IExportStrategy
	{
		/// <summary>
		/// Activate the object
		/// </summary>
		/// <param name="exportInjectionScope">injetion scope</param>
		/// <param name="context">context for the activation</param>
		/// <param name="consider">consider filter</param>
		/// <returns>activated object</returns>
		public abstract object Activate(IInjectionScope exportInjectionScope,
			IInjectionContext context,
			ExportStrategyFilter consider);

		/// <summary>
		/// Dispose of strategy
		/// </summary>
		public void Dispose()
		{
		}

		/// <summary>
		/// Activation Type
		/// </summary>
		public abstract Type ActivationType { get; }

		/// <summary>
		/// Activation name
		/// </summary>
		public abstract string ActivationName { get; }

		/// <summary>
		/// Allow filtering of type
		/// </summary>
		public bool AllowingFiltering
		{
			get { return false; }
		}

		/// <summary>
		/// Empty list of attributes
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
		/// Export strategy key
		/// </summary>
		public object Key { get; private set; }

		/// <summary>
		/// List of export names
		/// </summary>
		public IEnumerable<string> ExportNames
		{
			get {  yield break;}
		}

		/// <summary>
		/// List of export types
		/// </summary>
		public abstract IEnumerable<Type> ExportTypes { get; }

		/// <summary>
		/// Export as any
		/// </summary>
		public ExportEnvironment Environment
		{
			get { return ExportEnvironment.Any; }
		}

		/// <summary>
		/// Priority of zero
		/// </summary>
		public int Priority
		{
			get { return 0; }
		}

		/// <summary>
		/// Has no lifestyle
		/// </summary>
		public ILifestyle Lifestyle
		{
			get { return null; }
		}

		/// <summary>
		/// Has no conditions
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
		/// Metadata for export
		/// </summary>
		public IExportMetadata Metadata
		{
			get { return new ExportMetadata(null, new Dictionary<string, object>()); }
		}

		/// <summary>
		/// Base initialize
		/// </summary>
		public virtual void Initialize()
		{
		}

		/// <summary>
		/// Always meets condition
		/// </summary>
		/// <param name="injectionContext"></param>
		/// <returns></returns>
		public bool MeetsCondition(IInjectionContext injectionContext)
		{
			return true;
		}

		/// <summary>
		/// Secondary strategies
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IExportStrategy> SecondaryStrategies()
		{
			yield break;
		}

		/// <summary>
		/// Enrich with delegate
		/// </summary>
		/// <param name="enrichWithDelegate">enrichment delegate</param>
		public void EnrichWithDelegate(EnrichWithDelegate enrichWithDelegate)
		{
		}

		/// <summary>
		/// Doesn't depend on anything
		/// </summary>
		public IEnumerable<ExportStrategyDependency> DependsOn
		{
			get { yield break; }
		}
	}
}