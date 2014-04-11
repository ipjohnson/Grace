using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection
{
   /// <summary>
   /// C# extensions for registration block
   /// </summary>
   // ReSharper disable once InconsistentNaming
	public static class IExportRegistrationBlockExtensions
	{
		/// <summary>
		/// Export an app config section, by default it's exported as a singleton
		/// </summary>
		/// <typeparam name="T">type of section</typeparam>
		/// <param name="registrationBlock">registration block to export section in</param>
		/// <param name="sectionName">section name</param>
		/// <returns>configuration object</returns>
		public static IFluentExportInstanceConfiguration<T> ExportConfigurationSection<T>(
				this IExportRegistrationBlock registrationBlock,
				string sectionName) where T : ConfigurationSection
		{
			return registrationBlock.ExportInstance((scope, context) => (T)ConfigurationManager.GetSection(sectionName)).
											 UsingLifestyle(new SingletonLifestyle());
		}

		/// <summary>
		/// Export the app setting elements from the App.Config by name
		/// </summary>
		/// <param name="registrationBlock">registration block</param>
	   public static void ExportAppSettingsByName(this IExportRegistrationBlock registrationBlock)
	   {
			foreach (string appSettingKey in ConfigurationManager.AppSettings.AllKeys)
			{
				registrationBlock.ExportInstance(ConfigurationManager.AppSettings[appSettingKey]).
									   AsName(appSettingKey).
										UsingLifestyle(new SingletonLifestyle());
			}
	   }

		/// <summary>
		/// Export the connection strings from the app.config
		/// </summary>
		/// <param name="registrationBlock">registration block</param>
	   public static void ExportConnectionStrings(this IExportRegistrationBlock registrationBlock)
	   {
			foreach (ConnectionStringSettings connectionString in ConfigurationManager.ConnectionStrings)
			{
				registrationBlock.ExportInstance(connectionString.ConnectionString).
										AsName(connectionString.Name).
										UsingLifestyle(new SingletonLifestyle());
			}
	   }
	}
}
