using System;
using Grace.Data;
using JetBrains.Annotations;

namespace Grace.DependencyInjection
{
	/// <summary>
	/// IInjectionScope represents a scope for injection that can be created and inherited from at any time
	/// Note: The implementation for IInjectionScope is thread safe. 
	/// </summary>
	public interface IInjectionScope : IExportLocator, IExtraDataContainer, IDisposalScope
	{
		/// <summary>
		/// The container this scope was created in
		/// </summary>
		[NotNull]
		IDependencyInjectionContainer Container { get; }

		/// <summary>
		/// Unique identifier for the instance of the injection scope
		/// </summary>
		Guid ScopeId { get; }

		/// <summary>
		/// The scopes name
		/// </summary>
		[CanBeNull]
		string ScopeName { get; }

		/// <summary>
		/// Parent scope, can be null if it's the root scope
		/// </summary>
		[CanBeNull]
		IInjectionScope ParentScope { get; }
	}
}