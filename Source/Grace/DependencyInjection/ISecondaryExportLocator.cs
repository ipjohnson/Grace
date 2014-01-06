using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Grace.DependencyInjection
{
	/// <summary>
	/// Classes that wish to participate in resolving unknown exports can implement this interface.
	/// If an export can't be located in a scope a secondary dependency resolvers will be called
	/// </summary>
	public interface ISecondaryExportLocator
	{
		/// <summary>
		/// Can locate a type
		/// </summary>
		/// <param name="context"></param>
		/// <param name="resolveName"></param>
		/// <param name="resolveType"></param>
		/// <param name="consider"></param>
		/// <returns></returns>
		bool CanLocate([NotNull] IInjectionContext context,
			[CanBeNull] string resolveName,
			[CanBeNull] Type resolveType,
			[CanBeNull] ExportStrategyFilter consider);

		/// <summary>
		/// Locate will be called when the injection scope can't locate a particular resource
		/// </summary>
		/// <param name="owningScope">the scope that the locate came through</param>
		/// <param name="context">injection context for the locate</param>
		/// <param name="resolveName">name being resolved</param>
		/// <param name="resolveType">type being resolved</param>
		/// <param name="consider">filter to use while resolving</param>
		/// <returns></returns>
		object Locate([NotNull]IInjectionScope owningScope,
			[NotNull]IInjectionContext context,
			[CanBeNull]string resolveName,
			[CanBeNull]Type resolveType,
			[CanBeNull]ExportStrategyFilter consider);

		/// <summary>
		/// LocateAll will be called every time a collection is resolved
		/// </summary>
		/// <param name="owningScope">the scope that the locate came through</param>
		/// <param name="context">injection context for the locate</param>
		/// <param name="resolveName">name of the export being resolved</param>
		/// <param name="resolveType">type that is being resolved</param>
		/// <param name="collectionEmpty">value saying if there are already values in the collection</param>
		/// <param name="consider">filter to use while locating</param>
		/// <returns>list of exports</returns>
		[NotNull]
		IEnumerable<object> LocateAll([NotNull]IInjectionScope owningScope,
			[NotNull]IInjectionContext context,
			[CanBeNull]string resolveName,
			[CanBeNull]Type resolveType,
			bool collectionEmpty,
			[CanBeNull]ExportStrategyFilter consider);
	}
}