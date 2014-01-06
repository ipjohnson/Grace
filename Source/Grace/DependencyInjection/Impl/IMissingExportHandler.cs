using System;

namespace Grace.DependencyInjection.Impl
{
	/// <summary>
	/// This interface is an internal interface that is used to tell the scope that there is a missing
	/// </summary>
	public interface IMissingExportHandler
	{
		/// <summary>
		/// Locate missing exports
		/// </summary>
		/// <param name="context">injection context</param>
		/// <param name="exportName">export name</param>
		/// <param name="exportType">export type</param>
		/// <param name="consider">export filter</param>
		/// <returns>export object</returns>
		object LocateMissingExport(IInjectionContext context,
			string exportName,
			Type exportType,
			ExportStrategyFilter consider);
	}
}