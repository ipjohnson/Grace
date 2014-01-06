using System;
using Grace.DependencyInjection.Attributes.Interfaces;
using Grace.DependencyInjection.Conditions;

namespace Grace.DependencyInjection.Attributes
{
	/// <summary>
	/// 
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class ExportWhenInjectedIntoAttribute : Attribute, IExportConditionAttribute
	{
		private readonly Type[] injectedType;

		public ExportWhenInjectedIntoAttribute(params Type[] injectedType)
		{
			this.injectedType = injectedType;
		}

		public IExportCondition ProvideCondition(Type exportType)
		{
			return new WhenInjectedInto(injectedType);
		}
	}
}