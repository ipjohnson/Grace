using System;
using System.Collections.Generic;
using System.Reflection;
using Grace.DependencyInjection.Attributes.Interfaces;

namespace Grace.DependencyInjection.Attributes
{
	/// <summary>
	/// Classes that have this attribute will be exported by the interfaces it implements
	/// </summary>
	public class ExportByInterfacesAttribute : Attribute, IExportAttribute
	{
		/// <summary>
		/// Provides a list of export types (i.e. implemented interfaces)
		/// </summary>
		/// <param name="attributedType"></param>
		/// <returns></returns>
		public IEnumerable<Type> ProvideExportTypes(Type attributedType)
		{
			return attributedType.GetTypeInfo().ImplementedInterfaces;
		}
	}
}