using System;

namespace Grace.DependencyInjection.Exceptions
{
	/// <summary>
	/// Exception thrown when you can't resolve a dependency
	/// </summary>
	public class DependencyResolveException : Exception
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="injectedType"></param>
		/// <param name="injectedName"></param>
		/// <param name="importName"></param>
		public DependencyResolveException(Type injectedType, string injectedName, string importName)
			: base(string.Format("Could not locate {0} for {1} on {2}", importName, injectedName, injectedType.FullName))
		{
			InjectedType = injectedType;
			InjectedName = injectedName;
			ImportName = importName;
		}

		/// <summary>
		/// Type being injected into
		/// </summary>
		public Type InjectedType { get; private set; }

		/// <summary>
		/// Name of property or parameter being injected into
		/// </summary>
		public string InjectedName { get; private set; }

		/// <summary>
		/// Export name or Type that is being located
		/// </summary>
		public string ImportName { get; private set; }
	}
}