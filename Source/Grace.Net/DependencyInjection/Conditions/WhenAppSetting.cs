using System;
using System.Configuration;
using Grace.Logging;

namespace Grace.DependencyInjection.Conditions
{
	/// <summary>
	/// Export condition that limits the export to only be used when the provided app.config set value is provided
	/// </summary>
	public class WhenAppSetting : IExportCondition
	{
		private static readonly string supplemental = typeof(WhenAppSetting).FullName;
		private readonly bool cacheAnswer;
		private readonly string settingName;
		private readonly object settingValue;
		private bool? answer;

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="settingName">app setting name</param>
		/// <param name="settingValue">app setting value</param>
		/// <param name="cacheAnswer">should the answer be cached</param>
		public WhenAppSetting(string settingName, object settingValue, bool cacheAnswer = true)
		{
			this.settingName = settingName;
			this.settingValue = settingValue;
			this.cacheAnswer = cacheAnswer;
		}

		/// <summary>
		/// returns true when the app.config setting is equal to the provided value
		/// </summary>
		/// <param name="scope"></param>
		/// <param name="injectionContext"></param>
		/// <param name="exportStrategy"></param>
		/// <returns></returns>
		public bool ConditionMeet(IInjectionScope scope, IInjectionContext injectionContext, IExportStrategy exportStrategy)
		{
			if (answer.HasValue)
			{
				return answer.Value;
			}

			string appSetting = ConfigurationManager.AppSettings[settingName];
			bool returnValue = false;

			if (appSetting != null)
			{
				if (settingValue == null)
				{
					return false;
				}

				if (appSetting.GetType() == settingValue.GetType())
				{
					returnValue = Equals(settingValue, appSetting);
				}

				try
				{
					returnValue = Equals(settingValue, Convert.ChangeType(appSetting, settingValue.GetType()));
				}
				catch (Exception exp)
				{
					Logger.Error(
						string.Format("Exception thrown while converting {0} to {1}", appSetting, settingValue.GetType().FullName),
						supplemental,
						exp);
				}
			}
			else if (settingValue == null)
			{
				returnValue = true;
			}

			if (cacheAnswer)
			{
				answer = returnValue;
			}

			return returnValue;
		}
	}
}