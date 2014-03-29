using Grace.DependencyInjection.Configuration;

namespace Grace.DependencyInjection
{
	/// <summary>
	/// Export locator extensions
	/// </summary>
   // ReSharper disable once InconsistentNaming
	public static class IExportLocatorExtensions
	{
		/// <summary>
		/// Configure a scope or container using App.config
		/// </summary>
		/// <param name="exportLocator"></param>
		/// <param name="sectionName"></param>
		public static void ConfigureWithXml(this IExportLocator exportLocator, string sectionName = "grace")
		{
			exportLocator.Configure(new AppConfigModule { SectionName = sectionName });
		}
	}
}