using System;

namespace Grace.DependencyInjection.Attributes.Interfaces
{
	/// <summary>
	/// Attributes that implement this interface will be called during discovery time
	/// </summary>
	public interface IExportEnvironmentAttribute
	{
		/// <summary>
		/// Provide the environment for the specified type
		/// </summary>
		/// <param name="attributedType">attributed type</param>
		/// <returns>export environment</returns>
		ExportEnvironment ProvideEnvironment(Type attributedType);
	}
}