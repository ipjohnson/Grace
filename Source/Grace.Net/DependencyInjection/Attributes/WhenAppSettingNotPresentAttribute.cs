using System;
using Grace.DependencyInjection.Attributes.Interfaces;
using Grace.DependencyInjection.Conditions;

namespace Grace.DependencyInjection.Attributes
{
	/// <summary>
	/// Limits an export to be used only when an app setting is not present
	/// </summary>
	public class WhenAppSettingNotPresentAttribute : Attribute, IExportConditionAttribute
	{
		private readonly string settingName;

		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="settingName"></param>
		public WhenAppSettingNotPresentAttribute(string settingName)
		{
			CacheAnswer = true;
			this.settingName = settingName;
		}

		/// <summary>
		/// Cache the answer to if the condition is meet
		/// </summary>
		public bool CacheAnswer { get; set; }

		/// <summary>
		/// Provide an export condition for an attirbuted type
		/// </summary>
		/// <param name="attributedType"></param>
		/// <returns></returns>
		public IExportCondition ProvideCondition(Type attributedType)
		{
			return new WhenAppSettingNotPresent(settingName, CacheAnswer);
		}
	}
}