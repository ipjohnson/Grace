using System;
using Grace.DependencyInjection.Attributes.Interfaces;
using Grace.DependencyInjection.Conditions;

namespace Grace.DependencyInjection.Attributes
{
	/// <summary>
	/// Limits export to when an app setting is present
	/// </summary>
	public class WhenAppSettingPresentAttribute : Attribute, IExportConditionAttribute
	{
		private readonly string settingName;

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="settingName"></param>
		public WhenAppSettingPresentAttribute(string settingName)
		{
			this.settingName = settingName;
		}

		/// <summary>
		/// Cache the answer
		/// </summary>
		public bool CacheAnswer { get; set; }

		/// <summary>
		/// Provide an export condition for an attirbuted type
		/// </summary>
		/// <param name="attributedType"></param>
		/// <returns></returns>
		public IExportCondition ProvideCondition(Type attributedType)
		{
			return new WhenAppSettingPresent(settingName, CacheAnswer);
		}
	}
}