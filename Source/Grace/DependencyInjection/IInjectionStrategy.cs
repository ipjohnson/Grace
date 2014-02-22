using System;

namespace Grace.DependencyInjection
{
	/// <summary>
	/// Provides a mechanism to inject an object
	/// </summary>
	public interface IInjectionStrategy
	{
		/// <summary>
		/// Initialize the injection strategy
		/// </summary>
		void Initialize();

		/// <summary>
		/// Type that this strategy targets
		/// </summary>
		Type TargeType { get; }

		/// <summary>
		/// Inject an object
		/// </summary>
		/// <param name="injectionContext"></param>
		/// <param name="injectTarget"></param>
		void Inject(IInjectionContext injectionContext, object injectTarget);
	}
}