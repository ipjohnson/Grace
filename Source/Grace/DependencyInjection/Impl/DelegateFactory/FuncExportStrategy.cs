using System;
using System.Collections.Generic;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl
{
	/// <summary>
	/// This export strategy creates a new Func(T) that calls the current scope 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class FuncExportStrategy<T> : IExportStrategy
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
			IInjectionScope scope = context.RequestingScope;
			IDisposalScope disposalScope = context.DisposalScope;
			IInjectionTargetInfo targetInfo = context.TargetInfo;

			return new Func<T>(
				() =>
				{
					IInjectionContext newInjectionContext = context.Clone();

					newInjectionContext.RequestingScope = scope;
					newInjectionContext.DisposalScope = disposalScope;
					newInjectionContext.TargetInfo = targetInfo;

					return newInjectionContext.RequestingScope.Locate<T>(injectionContext: newInjectionContext);
				});
		}

		/// <summary>
		/// Dispose the func export, nothing to do
		/// </summary>
		public void Dispose()
		{
		}

		/// <summary>
		/// Initialize the strategy
		/// </summary>
		public void Initialize()
		{
		}

		/// <summary>
		/// This is type that will be activated, can be used for filtering
		/// </summary>
		public Type ActivationType
		{
			get { return typeof(Func<T>); }
		}

		/// <summary>
		/// Usually the type.FullName, used for blacklisting purposes
		/// </summary>
		public string ActivationName
		{
			get { return typeof(Func<T>).FullName; }
		}

		/// <summary>
		/// Allows filter of strategy
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
			get { return new Attribute[0]; }
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
			get { return new string[0]; }
		}

		/// <summary>
		/// Types this strategy should be known as
		/// </summary>
		public IEnumerable<Type> ExportTypes
		{
			get { yield return typeof(Func<T>); }
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
		/// An export can specify it's own strategy
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IExportStrategy> SecondaryStrategies()
		{
			return new IExportStrategy[0];
		}

		/// <summary>
		/// Adds an enrich with delegate to the pipeline
		/// </summary>
		/// <param name="enrichWithDelegate"></param>
		public void EnrichWithDelegate(EnrichWithDelegate enrichWithDelegate)
		{

		}

		/// <summary>
		/// Doesn't depend on anything to construct
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
			get { return new ExportMetadata(Key, new Dictionary<string, object>()); }
		}
	}
}