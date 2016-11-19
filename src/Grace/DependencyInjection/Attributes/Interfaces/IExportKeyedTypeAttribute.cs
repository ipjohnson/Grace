using System;

namespace Grace.DependencyInjection.Attributes.Interfaces
{
	/// <summary>
	/// Attributes that implement this interface will be used to provide an export key
	/// </summary>
	public interface IExportKeyedTypeAttribute
	{
		/// <summary>
		/// Provide an export key
		/// </summary>
		/// <param name="attributedType">attributed type</param>
		/// <returns>export key</returns>
		Tuple<Type,object> ProvideKey(Type attributedType);
	}
}
