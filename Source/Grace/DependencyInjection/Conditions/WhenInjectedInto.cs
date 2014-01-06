using System;
using System.Reflection;

namespace Grace.DependencyInjection.Conditions
{
	/// <summary>
	/// Export condition that is true when being injected into
	/// </summary>
	public class WhenInjectedInto : IExportCondition
	{
		private readonly Type[] injectedType;

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="injectedType"></param>
		public WhenInjectedInto(params Type[] injectedType)
		{
			this.injectedType = injectedType;
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
			IInjectionTargetInfo targetInfo = injectionContext.TargetInfo;

			if (targetInfo != null)
			{
				foreach (Type type in injectedType)
				{
					if (targetInfo.InjectionType.GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
					{
						return true;
					}
				}
			}

			return false;
		}
	}
}