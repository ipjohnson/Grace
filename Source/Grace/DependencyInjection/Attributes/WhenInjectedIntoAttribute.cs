using System;
using Grace.DependencyInjection.Attributes.Interfaces;
using Grace.DependencyInjection.Conditions;

namespace Grace.DependencyInjection.Attributes
{
	/// <summary>
	/// Limits an export to only be used when being injected into one of the types
	/// </summary>
	public class WhenInjectedIntoAttribute : Attribute, IExportConditionAttribute
	{
		private readonly Type[] injectionType;

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="injectionType"></param>
		public WhenInjectedIntoAttribute(params Type[] injectionType)
		{
			this.injectionType = injectionType;
		}

		/// <summary>
		/// Provide an export condition for an attirbuted type
		/// </summary>
		/// <param name="attributedType"></param>
		/// <returns></returns>
		public IExportCondition ProvideCondition(Type attributedType)
		{
			return new WhenInjectedInto(injectionType);
		}
	}
}