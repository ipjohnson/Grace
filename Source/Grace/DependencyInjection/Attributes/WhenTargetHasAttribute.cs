using System;
using Grace.DependencyInjection.Attributes.Interfaces;
using Grace.DependencyInjection.Conditions;

namespace Grace.DependencyInjection.Attributes
{
	/// <summary>
	/// Limits an export to only be used when the target has a particular attribute
	/// </summary>
	public class WhenTargetHasAttribute : Attribute, IExportConditionAttribute
	{
		private readonly Type attributeType;

		public WhenTargetHasAttribute(Type attributeType)
		{
			this.attributeType = attributeType;
		}

		/// <summary>
		/// Provide an export condition for an attirbuted type
		/// </summary>
		/// <param name="attributedType"></param>
		/// <returns></returns>
		public IExportCondition ProvideCondition(Type attributedType)
		{
			return new WhenTargetHas(attributeType);
		}
	}
}