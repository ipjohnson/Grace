using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Grace.Validation.Attributes
{
	/// <summary>
	/// Provides validation rules for an object type based on attributes
	/// </summary>
	public class AttributeValidationRuleProvider : IValidationRuleProvider
	{
		private readonly IReadOnlyCollection<IAttributeValidationRuleProvider> attributeValidationRuleProviders;

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="attributeValidationRuleProviders">attribute validation providers</param>
		public AttributeValidationRuleProvider(
			IReadOnlyCollection<IAttributeValidationRuleProvider> attributeValidationRuleProviders)
		{
			this.attributeValidationRuleProviders = attributeValidationRuleProviders;
		}

		public IEnumerable<IValidationRule> ProvideRules(Type objectType, IEnumerable<string> standards)
		{
			List<IValidationRule> returnList = new List<IValidationRule>();
			List<Attribute> typeAttributes = new List<Attribute>(objectType.GetTypeInfo().GetCustomAttributes());

			foreach (IAttributeValidationRuleProvider attributeValidationRuleProvider in attributeValidationRuleProviders)
			{
				returnList.AddRange(attributeValidationRuleProvider.ProvideRules(objectType, typeAttributes, null, null));
			}

			foreach (PropertyInfo runtimeProperty in objectType.GetRuntimeProperties())
			{
				if (!runtimeProperty.CanRead || 
					 !runtimeProperty.GetMethod.IsPublic ||
					 runtimeProperty.GetMethod.IsStatic)
				{
					continue;
				}

				foreach (IAttributeValidationRuleProvider attributeValidationRuleProvider in attributeValidationRuleProviders)
				{
					List<Attribute> propertyAttributes = new List<Attribute>(runtimeProperty.GetCustomAttributes());

					returnList.AddRange(attributeValidationRuleProvider.ProvideRules(objectType, typeAttributes, runtimeProperty, propertyAttributes));
				}
			}

			return returnList;
		}

		public IEnumerable<string> GetPropertiesToValidate(Type objectType, IEnumerable<string> standards)
		{
			return new string[0];
		}
	}
}
