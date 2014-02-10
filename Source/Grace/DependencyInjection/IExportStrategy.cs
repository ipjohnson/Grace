using System;
using System.Collections.Generic;
using Grace.DependencyInjection.Lifestyle;
using JetBrains.Annotations;

namespace Grace.DependencyInjection
{
	/// <summary>
	/// Classes that implement this interface can be used to export a particular type
	/// Note: All implementations of ExportStrategy should be thread safe. 
	/// It is expected that N number callers can activate at the same time.
	/// It's also recommended that the strategy me as immutable as possible
	/// </summary>
	public interface IExportStrategy : IExportValueProvider, IDisposable
	{
		/// <summary>
		/// This is type that will be activated, can be used for filtering
		/// </summary>
		[NotNull]
		Type ActivationType { get; }

		/// <summary>
		/// Usually the type.FullName, used for blacklisting purposes
		/// </summary>
		[NotNull]
		string ActivationName { get; }

		/// <summary>
		/// When considering an export should it be filtered out.
		/// True by default, usually it's only false for special export types like Array ad List
		/// </summary>
		bool AllowingFiltering { get; }

		/// <summary>
		/// Attributes associated with the export strategy. 
		/// Note: do not return null. Return an empty enumerable if there are none
		/// </summary>
		[NotNull]
		IEnumerable<Attribute> Attributes { get; }

		/// <summary>
		/// The scope that owns this export
		/// </summary>
		[NotNull]
		IInjectionScope OwningScope { get; set; }

		/// <summary>
		/// Export Key
		/// </summary>
		[CanBeNull]
		object Key { get; }

		/// <summary>
		/// Names this strategy should be known as.
		/// </summary>
		[NotNull]
		IEnumerable<string> ExportNames { get; }

		/// <summary>
		/// Type this strategy should be known as
		/// </summary>
		[NotNull]
		IEnumerable<Type> ExportTypes { get; }
			
		/// <summary>
		/// What environement is this strategy being exported under.
		/// </summary>
		ExportEnvironment Environment { get; }

		/// <summary>
		/// What export priority is this being exported as
		/// </summary>
		int Priority { get; }

		/// <summary>
		/// ILifestyle associated with export
		/// </summary>
		[CanBeNull]
		ILifestyle Lifestyle { get; }

		/// <summary>
		/// Does this export have any conditions, this is important when setting up the strategy
		/// </summary>
		bool HasConditions { get; }

		/// <summary>
		/// Are the object produced by this export externally owned
		/// </summary>
		bool ExternallyOwned { get; }

		/// <summary>
		/// Metadata associated with this strategy
		/// </summary>
		[NotNull]
		IExportMetadata Metadata { get; }

		/// <summary>
		/// Initialize the export, caled by the container
		/// </summary>
		void Initialize();

		/// <summary>
		/// Does this export meet the conditions to be used
		/// </summary>
		/// <param name="injectionContext">injection context</param>
		/// <returns>true if the export should be used</returns>
		bool MeetsCondition([NotNull]IInjectionContext injectionContext);

		/// <summary>
		/// An export can specify it's own strategy
		/// </summary>
		/// <returns>a list of strategies this export exports</returns>
		[NotNull]
		IEnumerable<IExportStrategy> SecondaryStrategies();

		/// <summary>
		/// Adds an enrich with delegate to the pipeline
		/// </summary>
		/// <param name="enrichWithDelegate">delegate called during activation</param>
		void EnrichWithDelegate([NotNull]EnrichWithDelegate enrichWithDelegate);

		/// <summary>
		/// List of dependencies for this strategy
		/// </summary>
		[NotNull]
		IEnumerable<ExportStrategyDependency> DependsOn { get; } 
	}
}