using System;

namespace Grace.DependencyInjection.Exceptions
{
	/// <summary>
	/// This exception is thrown when you attempt to clone an InjectionKernel that doesn't have a parent
	/// </summary>
	public class RootScopeCloneException : Exception
	{
		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="scopeName"></param>
		/// <param name="scopeId"></param>
		public RootScopeCloneException(string scopeName, Guid scopeId) :
			base(string.Format("ScopeName {0} ScopeId {1}", scopeName, scopeId))
		{
		}
	}
}