using System;
using Grace.DependencyInjection.Attributes.Interfaces;
using Grace.DependencyInjection.Conditions;

namespace Grace.DependencyInjection.Attributes
{
	/// <summary>
	/// Limits an export to only be used when an app setting is a particular value
	/// </summary>
	public class WhenAppSettingAttribute : Attribute, IExportConditionAttribute
	{
		private readonly string settingName;
		private readonly object settingValue;

		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="settingName"></param>
		/// <param name="settingValue"></param>
		public WhenAppSettingAttribute(string settingName, object settingValue)
		{
			CacheAnswer = true;
			this.settingName = settingName;
			this.settingValue = settingValue;
		}

		/// <summary>
		/// Cache the answer on if the attribute should be used
		/// </summary>
		public bool CacheAnswer { get; set; }

		/// <summary>
		/// Provide an export condition for an attirbuted type
		/// </summary>
		/// <param name="attributedType"></param>
		/// <returns></returns>
		public IExportCondition ProvideCondition(Type attributedType)
		{
			return new WhenAppSetting(settingName, settingValue, CacheAnswer);
		}
	}
}