using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection.Attributes.Interfaces
{
	/// <summary>
	/// Attributes that implement this interface will be used to figure out if the property should be done after construction
	/// </summary>
	public interface IImportAfterConstructionAttribute
	{
		/// <summary>
		/// Should import after construction
		/// </summary>
		/// <param name="activationType">type being activated</param>
		/// <param name="propertyType">property type</param>
		/// <returns>should import after</returns>
		bool ImportAfterConstruction(Type activationType, Type propertyType);
	}
}
