using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Grace.Logging;

namespace Grace.DependencyInjection.Configuration
{
	public class AppConfigModule : IConfigurationModule
	{
		public AppConfigModule()
		{
			SectionName = "grace";
		}

		public string SectionName { get; set; }

		public void Configure(IExportRegistrationBlock registrationBlock)
		{
			GraceConfigurationSection graceSection = null;

			try
			{
				graceSection = ConfigurationManager.GetSection(SectionName) as GraceConfigurationSection;
			}
			catch (Exception exp)
			{
				Logger.Error("Exception throw while trying to fetch configuration section: " + SectionName, "AppConfig", exp);
			}

			if (graceSection != null)
			{
				var exports = graceSection.Exports;

				if (exports != null)
				{
					foreach (ExportElement exportElement in exports)
					{

					}
				}

				var modules = graceSection.Modules;

				if (modules != null)
				{
					foreach (ModuleElement moduleElement in modules)
					{
						try
						{
							Type moduleType = Type.GetType(moduleElement.Type);

							if (moduleType != null)
							{
								IConfigurationModule configurationModule =
									(IConfigurationModule)Activator.CreateInstance(moduleType);

								ConfigureModule(registrationBlock, configurationModule, moduleElement);
							}
							else
							{
								Logger.Error("Could not locate type: " + moduleElement.Type, "AppConfig");
							}
						}
						catch (Exception exp)
						{
							Logger.Error("Exception thrown while trying to load modue: " + moduleElement.Type, "AppConfig", exp);
						}
					}
				}
			}
		}

		private void ConfigureModule(IExportRegistrationBlock registrationBlock, IConfigurationModule configurationModule, ModuleElement element)
		{
			foreach (PropetryElement propertyElement in element.Propetries)
			{
				PropertyInfo propertyInfo =
					configurationModule.GetType().GetRuntimeProperty(propertyElement.Name);

				if (propertyInfo != null && propertyInfo.CanWrite)
				{
					object finalValue = null;

					if (propertyInfo.PropertyType == typeof(string))
					{
						finalValue = propertyElement.Value;
					}
					else
					{
						try
						{
							finalValue = Convert.ChangeType(propertyElement.Value, propertyInfo.PropertyType);
						}
						catch (Exception exp)
						{
							Logger.Error("Exceptuion thrown while converting property: " + propertyElement.Name, "AppConfig", exp);
						}
					}

					if (finalValue != null)
					{
						propertyInfo.SetValue(configurationModule, finalValue);
					}
				}
			}

			configurationModule.Configure(registrationBlock);
		}
	}
}
