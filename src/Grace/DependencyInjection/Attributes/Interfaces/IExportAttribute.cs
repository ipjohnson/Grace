using System;
using System.Collections.Generic;

namespace Grace.DependencyInjection.Attributes.Interfaces
{
	/// <summary>
	/// Attributes that implement this interface will be included while scanning for exports
	/// </summary>
	public interface IExportAttribute
	{
		/// <summary>
		/// Provide a list of types to export as
		/// </summary>
		/// <param name="attributedType"></param>
		/// <returns></returns>
		IEnumerable<Type> ProvideExportTypes(Type attributedType);
	}
}