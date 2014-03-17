using System;

namespace Grace.DependencyInjection.Impl
{
	/// <summary>
	/// Inteface implemented by strategies that are open generic and can be closed
	/// </summary>
	public interface IGenericExportStrategy : IExportStrategy
	{
		/// <summary>
		/// Creates a closed representation of the strategy.
		/// It returns null when it can't be support the requested type
		/// </summary>
		/// <param name="requestedType">type being requested</param>
		/// <returns>export strategy</returns>
		IExportStrategy CreateClosedStrategy(Type requestedType);
	}
}