using System.Configuration;

namespace Grace.DependencyInjection.Conditions
{
	/// <summary>
	/// Limits an export to only be used when an app setting value is not present
	/// </summary>
	public class WhenAppSettingNotPresent : IExportCondition
	{
		private readonly bool cacheAnswer;
		private readonly string settingName;
		private bool? answer;

		/// <summary>
		/// Defaut Constructor
		/// </summary>
		/// <param name="settingName"></param>
		/// <param name="cacheAnswer"></param>
		public WhenAppSettingNotPresent(string settingName, bool cacheAnswer)
		{
			this.settingName = settingName;
			this.cacheAnswer = cacheAnswer;
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
			if (answer.HasValue)
			{
				return answer.Value;
			}

			bool returnValue = ConfigurationManager.AppSettings[settingName] == null;

			if (cacheAnswer)
			{
				answer = returnValue;
			}

			return returnValue;
		}
	}
}