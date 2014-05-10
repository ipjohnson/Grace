using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Grace.Validation
{
	/// <summary>
	/// Provides validations rules based on the type and attributes provided
	/// </summary>
	public interface IAttributeValidationRuleProvider
	{
		/// <summary>
		/// Provide an enumeration of validation rules based on the attributes on a type
		/// </summary>
		/// <param name="objectType">type being validated</param>
		/// <param name="typeAttributes">attributes on the type</param>
		/// <param name="propertyInfo">property info that is tagged, can be null when the attribute is on the type</param>
		/// <param name="propertyInfoAttributes">attributes on the property, can be null</param>
		/// <returns>list of validation rules</returns>
		IEnumerable<IValidationRule> ProvideRules(Type objectType, IEnumerable<Attribute> typeAttributes, PropertyInfo propertyInfo, IEnumerable<Attribute> propertyInfoAttributes);
	}
}
