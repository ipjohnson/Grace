using System;

namespace Grace.DependencyInjection.Attributes.Interfaces
{
	/// <summary>
	/// Attributes that implement this interface can be used to filter an import
	/// </summary>
	public interface IImportFilterAttribute
	{
		/// <summary>
		/// Provide a filter to be used when importing
		/// </summary>
		/// <param name="attributedType">attributed type</param>
		/// <param name="attributedName">attributed name</param>
		/// <returns>new filter</returns>
		ExportStrategyFilter ProvideFilter(Type attributedType, string attributedName);
	}
}