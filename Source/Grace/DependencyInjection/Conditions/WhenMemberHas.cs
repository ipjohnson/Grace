using System;
using System.Reflection;

namespace Grace.DependencyInjection.Conditions
{
	/// <summary>
	/// Export condition that is true when the member (Constructor,Method or Property) has a particular attribute
	/// </summary>
	public class WhenMemberHas : IExportCondition
	{
		private readonly Type attributeType;

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="attributeType"></param>
		public WhenMemberHas(Type attributeType)
		{
			this.attributeType = attributeType;
		}

		/// <summary>
		/// Called to determine if the export strategy meets the condition to be activated
		/// </summary>
		/// <param name="scope">injection scope that this export exists in</param>
		/// <param name="injectionContext">injection context for this request</param>
		/// <param name="exportStrategy">export strategy being tested</param>
		/// <returns>true if the export meets the condition</returns>
		public bool ConditionMeet(IInjectionScope scope, IInjectionContext injectionContext, IExportStrategy exportStrategy)
		{
			if (injectionContext.TargetInfo != null)
			{
				foreach (Attribute injectionTargetAttribute in injectionContext.TargetInfo.InjectionMemberAttributes)
				{
					if (injectionTargetAttribute.GetType().GetTypeInfo().IsAssignableFrom(attributeType.GetTypeInfo()))
					{
						return true;
					}
				}
			}

			return false;
		}
	}
}