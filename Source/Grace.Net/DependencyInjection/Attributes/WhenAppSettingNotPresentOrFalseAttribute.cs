using System;
using Grace.DependencyInjection.Attributes.Interfaces;
using Grace.DependencyInjection.Conditions;

namespace Grace.DependencyInjection.Attributes
{
	/// <summary>
	/// Export condition attribute that limits the export to only when 
	/// </summary>
	public class WhenAppSettingNotPresentOrFalseAttribute : Attribute, IExportConditionAttribute
	{
		private readonly string settingName;

		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="settingName"></param>
		public WhenAppSettingNotPresentOrFalseAttribute(string settingName)
		{
			CacheAnswer = true;
			this.settingName = settingName;
		}

		/// <summary>
		/// Cache the answer, default is true
		/// </summary>
		public bool CacheAnswer { get; set; }

		/// <summary>
		/// Provide an export condition for an attirbuted type
		/// </summary>
		/// <param name="attributedType"></param>
		/// <returns></returns>
		public IExportCondition ProvideCondition(Type attributedType)
		{
			return new WhenAppSettingNotPresentOrFalse(settingName, CacheAnswer);
		}
	}
}