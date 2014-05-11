using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection.Attributes.Interfaces;

namespace Grace.DependencyInjection.Attributes
{
	/// <summary>
	/// Exports a class by it's name
	/// </summary>
	public class ExportByClassNameAttribute : Attribute, IExportAttribute
	{
		/// <summary>
		/// Exports a class by name
		/// </summary>
		/// <param name="attributedType">class to export</param>
		/// <returns>list of export names</returns>
		public virtual IEnumerable<string> ProvideExportNames(Type attributedType)
		{
			yield return attributedType.Name;
		}

		/// <summary>
		/// Exports a class by type
		/// </summary>
		/// <param name="attributedType">class to be exported</param>
		/// <returns>list of types to export by</returns>
		public virtual IEnumerable<Type> ProvideExportTypes(Type attributedType)
		{
			return new Type[0];
		}
	}
}
