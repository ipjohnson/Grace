using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl
{
	/// <summary>
	/// Export strategy to create ReadOnlyCollection(T) objects
	/// </summary>
	/// <typeparam name="T">type to locate</typeparam>
	public class ReadOnlyCollectionExportStrategy<T> : IExportStrategy
	{
		/// <summary>
		/// Activate the export
		/// </summary>
		/// <param name="exportInjectionScope">export injection scope</param>
		/// <param name="context">injection context</param>
		/// <param name="consider">consider filter</param>
		/// <param name="locateKey"></param>
		/// <returns>activated object</returns>
		public object Activate(IInjectionScope exportInjectionScope, IInjectionContext context, ExportStrategyFilter consider, object locateKey)
		{
			return new ReadOnlyCollection<T>(exportInjectionScope.LocateAll<T>(injectionContext: context, consider: consider));
		}

		/// <summary>
		/// Dispose strategy
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
		public Type ActivationType
		{
			get { return typeof(ReadOnlyCollection<T>); }
		}

		/// <summary>
		/// Usually the type.FullName, used for blacklisting purposes
		/// </summary>
		public string ActivationName
		{
			get { return typeof(ReadOnlyCollection<T>).FullName; }
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
			get
			{
				yield return typeof(IReadOnlyCollection<T>);
				yield return typeof(IReadOnlyList<T>);
				yield return typeof(ReadOnlyCollection<T>);
			}
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
		/// 
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IExportStrategy> SecondaryStrategies()
		{
			yield break;
		}

		/// <summary>
		/// Enrichment delegate
		/// </summary>
		/// <param name="enrichWithDelegate"></param>
		public void EnrichWithDelegate(EnrichWithDelegate enrichWithDelegate)
		{
		}

		/// <summary>
		/// doesn't depend on anything
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

		/// <summary>
		/// Override equals to check if the export strategy is the same
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			ReadOnlyCollectionExportStrategy<T> collectionExport = obj as ReadOnlyCollectionExportStrategy<T>;

			if (collectionExport != null && collectionExport.OwningScope == OwningScope)
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Override because of equals override
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return ActivationName.GetHashCode();
		}
	}
}