using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection.Configuration;

namespace Grace.DependencyInjection
{
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
