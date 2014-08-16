using System;
using System.Collections.Generic;
using Grace.Data.Immutable;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl.DelegateFactory
{
	/// <summary>
	/// Keyed locate delegate creates new locate delegate
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	public class KeyedLocateDelegateStrategy<TKey, TValue> : IExportStrategy
	{
		public object Activate(IInjectionScope exportInjectionScope,
			IInjectionContext context,
			ExportStrategyFilter consider,
			object locateKey)
		{
			IInjectionScope injectionScope = context.RequestingScope;
			IDisposalScope disposalScope = context.DisposalScope;
			IInjectionTargetInfo targetInfo = context.TargetInfo;

			return new KeyedLocateDelegate<TKey, TValue>(key =>
																		{
																			IInjectionContext newContext = context.Clone();

																			newContext.RequestingScope = injectionScope;
																			newContext.DisposalScope = disposalScope;
																			newContext.TargetInfo = targetInfo;

																			return injectionScope.Locate<TValue>(newContext, consider, key);
																		});
		}

		/// <summary>
		/// Dispose of strategy
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
		/// Activation Type
		/// </summary>
		public Type ActivationType
		{
			get { return typeof(KeyedLocateDelegate<TKey, TValue>); }
		}

		/// <summary>
		/// Activation Name
		/// </summary>
		public string ActivationName
		{
			get { return typeof(KeyedLocateDelegate<TKey, TValue>).FullName; }
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
		/// Export names
		/// </summary>
		public IEnumerable<string> ExportNames
		{
			get { return ImmutableArray<string>.Empty; }
		}

		/// <summary>
		/// Export types
		/// </summary>
		public IEnumerable<Type> ExportTypes
		{
			get { yield return typeof(KeyedLocateDelegate<TKey, TValue>); }
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
		/// An export can specify it's own strategy
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IExportStrategy> SecondaryStrategies()
		{
			return ImmutableArray<IExportStrategy>.Empty;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="enrichWithDelegate"></param>
		public void EnrichWithDelegate(EnrichWithDelegate enrichWithDelegate)
		{

		}

		/// <summary>
		/// 
		/// </summary>
		public IEnumerable<ExportStrategyDependency> DependsOn
		{
            get { return ImmutableArray<ExportStrategyDependency>.Empty; }
		}

		/// <summary>
		/// Metadata associated with this strategy
		/// </summary>
		public IExportMetadata Metadata
		{
            get { return new ExportMetadata(null); }
		}
	}
}
