using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection.Attributes.Interfaces;

namespace Grace.DependencyInjection.Attributes
{
	/// <summary>
	/// Properties that are marked with this attribute will be imported after construction
	/// </summary>
	[AttributeUsage(AttributeTargets.Property,AllowMultiple = false)]
	public class AfterConstructionAttribute : Attribute, IImportAfterConstructionAttribute
	{
		/// <summary>
		/// Import after construction
		/// </summary>
		/// <param name="activationType"></param>
		/// <param name="propertyType"></param>
		/// <returns></returns>
		public bool ImportAfterConstruction(Type activationType, Type propertyType)
		{
			return true;
		}
	}
}
