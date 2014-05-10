using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Grace.Validation.Attributes
{
	public interface ISyncValidationRuleAttribute
	{
		IEnumerable<IValidationRule> ProvideValidationRule(Type objectType,
																			IEnumerable<Attribute> typeAttributes,
																			PropertyInfo propertyInfo,
																			IEnumerable<Attribute> propertyInfoAttributes);
	}
}
