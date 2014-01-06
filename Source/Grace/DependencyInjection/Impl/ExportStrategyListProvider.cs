using System.Collections.Generic;

namespace Grace.DependencyInjection.Impl
{
	/// <summary>
	/// List of export strategies that can act as a IExportStrategyProvider
	/// </summary>
	public class ExportStrategyListProvider : List<IExportStrategy>, IExportStrategyProvider
	{
		/// <summary>
		/// Provide a list of strategies
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IExportStrategy> ProvideStrategies()
		{
			return this;
		}
	}
}