using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection.Attributes.Interfaces;

namespace Grace.DependencyInjection.Attributes
{
	/// <summary>
	/// Adds a key to an exported class
	/// </summary>
	public class ExportKeyAttribute : Attribute, IExportKeyAttribute
	{
		private readonly object key;

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="key">export key</param>
		public ExportKeyAttribute(object key)
		{
			this.key = key;
		}

		/// <summary>
		/// Provide key
		/// </summary>
		/// <param name="attributedType">attributed type</param>
		/// <returns>export key</returns>
		public object ProvideKey(Type attributedType)
		{
			return key;
		}
	}
}
