using System;
using JetBrains.Annotations;

namespace Grace.DependencyInjection
{
	/// <summary>
	/// Defines a scope for injection that will dispose any objects added to it
	/// </summary>
	public interface IDisposalScope : IDisposable
	{
		/// <summary>
		/// Add an object for disposal 
		/// </summary>
		/// <param name="disposable"></param>
		/// <param name="cleanupDelegate">logic that will be run directly before the object is disposed</param>
		void AddDisposable([NotNull]IDisposable disposable, BeforeDisposalCleanupDelegate cleanupDelegate = null);

		/// <summary>
		/// Remove an object from the disposal scope
		/// </summary>
		/// <param name="disposable"></param>
		void RemoveDisposable([NotNull]IDisposable disposable);
	}
}