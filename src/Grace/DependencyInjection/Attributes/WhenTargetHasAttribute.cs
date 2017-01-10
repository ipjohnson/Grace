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
		private readonly Type _attributeType;

		/// <summary>
		/// Default constructor that takes attribute type
		/// </summary>
		/// <param name="attributeType"></param>
		public WhenTargetHasAttribute(Type attributeType)
		{
			_attributeType = attributeType;
		}

		/// <summary>
		/// Provide an export condition for an attirbuted type
		/// </summary>
		/// <param name="attributedType"></param>
		/// <returns></returns>
		public ICompiledCondition ProvideCondition(Type attributedType)
		{
			return new WhenTargetHas(_attributeType);
		}
	}
}