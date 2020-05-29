using System;
using Grace.DependencyInjection.Attributes.Interfaces;
using Grace.DependencyInjection.Conditions;

namespace Grace.DependencyInjection.Attributes
{
	/// <summary>
	/// Limits an export to only be used when the class it's being injected into has the specified attribute
	/// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class ExportWhenClassHasAttribute : Attribute, IExportConditionAttribute
	{
		private readonly Type _attributeType;

		/// <summary>
		/// Default constructor that takes an attribute type to filter on
		/// </summary>
		/// <param name="attributeType"></param>
		public ExportWhenClassHasAttribute(Type attributeType)
		{
			_attributeType = attributeType;
		}

		/// <summary>
		/// Provide a condition for the type provided
		/// </summary>
		/// <param name="exportType"></param>
		/// <returns></returns>
		public ICompiledCondition ProvideCondition(Type exportType)
		{
			return new WhenClassHas(_attributeType);
		}
	}
}