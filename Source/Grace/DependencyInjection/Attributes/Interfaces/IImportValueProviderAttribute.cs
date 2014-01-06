using System;

namespace Grace.DependencyInjection.Attributes.Interfaces
{
	/// <summary>
	/// Import attributes that implement this attribute can be used to import a particular value provider into 
	/// </summary>
	public interface IImportValueProviderAttribute
	{
		/// <summary>
		/// Provide an IExportValueProvider to be used on import
		/// </summary>
		/// <param name="attributedType"></param>
		/// <returns></returns>
		IExportValueProvider ProvideProvider(Type attributedType);
	}
}