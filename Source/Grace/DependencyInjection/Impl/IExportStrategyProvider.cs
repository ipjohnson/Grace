using System.Collections.Generic;

namespace Grace.DependencyInjection.Impl
{
	/// <summary>
	/// Used by classes that provide strategies to the registration block
	/// </summary>
	public interface IExportStrategyProvider
	{
		/// <summary>
		/// Provide a list of strategies
		/// </summary>
		/// <returns></returns>
		IEnumerable<IExportStrategy> ProvideStrategies();
	}
}