using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Grace.Validation.Attributes
{
	public class CustomAttributeValidationRuleProvider : IAttributeValidationRuleProvider
	{
		public IEnumerable<IValidationRule> ProvideRules(Type objectType,
																		IEnumerable<Attribute> typeAttributes,
																		PropertyInfo propertyInfo,
																		IEnumerable<Attribute> propertyInfoAttributes)
		{
			if (propertyInfoAttributes != null)
			{
				foreach (Attribute propertyInfoAttribute in propertyInfoAttributes)
				{
					ISyncValidationRuleAttribute validationAttribute = propertyInfoAttributes as ISyncValidationRuleAttribute;

					if (validationAttribute != null)
					{
						foreach (IValidationRule validationRule in validationAttribute.ProvideValidationRule(objectType, typeAttributes, propertyInfo, propertyInfoAttributes))
						{
							yield return validationRule;
						}
					}
				}
			}
		}
	}
}
