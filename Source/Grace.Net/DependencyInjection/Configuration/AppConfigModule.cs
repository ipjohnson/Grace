using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using Grace.DependencyInjection.Lifestyle;
using Grace.LanguageExtensions;
using Grace.Logging;

namespace Grace.DependencyInjection.Configuration
{
	/// <summary>
	/// This class can be used to configure a container or a scope using app.config (or web.config)
	/// usage container.Configure(new AppConfigModule());
	/// </summary>
	public class AppConfigModule : IConfigurationModule
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		public AppConfigModule()
		{
			SectionName = "grace";
		}

		/// <summary>
		/// Section name in the app.config 
		/// By default section name is grace
		/// </summary>
		public string SectionName { get; set; }

		/// <summary>
		/// Configure the module
		/// </summary>
		/// <param name="registrationBlock">registration block</param>
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
				AssemblyElementCollection assemblies = graceSection.Assemblies;

				foreach (AssemblyElement assemblyElement in assemblies)
				{
					ProcessAssembly(registrationBlock, assemblyElement);
				}

				ExportElementCollection exports = graceSection.Exports;

				if (exports != null)
				{
					foreach (ExportElement exportElement in exports)
					{
						ProcessExportElement(registrationBlock, exportElement);
					}
				}

				ModuleElementCollection modules = graceSection.Modules;

				if (modules != null)
				{
					foreach (ModuleElement moduleElement in modules)
					{
						try
						{
							Type moduleType = ConvertStringToType(moduleElement.Type);

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

		private void ProcessExportElement(IExportRegistrationBlock registrationBlock, ExportElement exportElement)
		{
			Type exportType = ConvertStringToType(exportElement.Type);

			if (exportType != null)
			{
				IFluentExportStrategyConfiguration config = registrationBlock.Export(exportType);

				if (exportElement.ExternallyOwned)
				{
					config = config.ExternallyOwned();
				}

				config.InEnvironment(exportElement.Environment);

				if (exportElement.AutoWireProperties)
				{
					config.AutoWireProperties();
				}

				foreach (AsElement asElement in exportElement)
				{
					if (asElement.Type != null)
					{
						Type asType = ConvertStringToType(asElement.Type);

						if (asType != null)
						{
							config = config.As(asType);
						}
					}
					else if (asElement.Name != null)
					{
						config.AsName(asElement.Name);
					}
				}

				if (string.IsNullOrEmpty(exportElement.LifeStyle))
				{
					switch (exportElement.LifeStyle)
					{
						case "Singleton":
							config = config.AndSingleton();
							break;
						case "WeakSingleton":
							config = config.AndWeakSingleton();
							break;
						case "SingletonPerInjection":
						case "PerInjection":
							config = config.UsingLifestyle(new SingletonPerInjectionContextLifestyle());
							break;
						case "SingletonPerScope":
						case "PerScope":
							config = config.AndSingletonPerScope();
							break;
						case "SingletonPerRequest":
						case "PerRequest":
							config = config.UsingLifestyle(new SingletonPerRequestLifestyle());
							break;

						default:
							Type lifeStyleType = ConvertStringToType(exportElement.LifeStyle);

							if (lifeStyleType != null)
							{
								try
								{
									ILifestyle lifestyle = Activator.CreateInstance(lifeStyleType) as ILifestyle;

									config = config.UsingLifestyle(lifestyle);
								}
								catch (Exception exp)
								{
									Logger.Error("Exception thrown while creating lifestyle container: " + lifeStyleType, "AppConfig", exp);
								}
							}
							break;
					}
				}
			}
		}

		private Type ConvertStringToType(string typeString)
		{
			if (typeString.IndexOf('.') < 0)
			{
				Type returnType = null;
				string toLowerType = typeString.ToLowerInvariant();
				
				switch (toLowerType)
				{
					case "int":
					case "int32":
					case "integer":
						returnType = typeof(int);
						break;
						
					case "uint":
					case "uint32":
					case "unsignedinteger":
						returnType = typeof(uint);
						break;
						
					case "long":
					case "int64":
						returnType = typeof(long);
						break;

					case "ulong":
					case "uint64":
					case "unsignedlong":
						returnType = typeof(ulong);
						break;

					case "double":
						returnType = typeof(double);
						break;

					case "decimal":
						returnType = typeof(decimal);
						break;

					case "float":
						returnType = typeof(float);
						break;

					case "datetime":
						returnType = typeof(DateTime);
						break;

					case "timespan":
						returnType = typeof(TimeSpan);
						break;

					case "string":
						returnType = typeof(string);
						break;

					default:
						returnType = ScanLoadedAssembliesForType(typeString);
						break;
				}

				return returnType;
			}

			return Type.GetType(typeString);
		}

		private Type ScanLoadedAssembliesForType(string typeString)
		{
			string lower = typeString.ToLowerInvariant();
			Assembly entryAssembly = Assembly.GetEntryAssembly();
			Type returnType = null;

			if (entryAssembly != null)
			{
				returnType = entryAssembly.ExportedTypes.FirstOrDefault(x => x.Name == typeString);
			}

			if (returnType == null)
			{
				returnType =
					AppDomain.CurrentDomain.GetAssemblies()
												  .SelectMany(x => x.IsDynamic ? new Type[0] : x.ExportedTypes)
												  .FirstOrDefault(x => x.Name.ToLowerInvariant() == lower);
			}

			return returnType;
		}

		private void ProcessAssembly(IExportRegistrationBlock registrationBlock, AssemblyElement assemblyElement)
		{
			try
			{
				Assembly newAssembly = Assembly.Load(assemblyElement.Path);

				if (assemblyElement.ScanForAttributes)
				{
					registrationBlock.Export(Types.FromAssembly(newAssembly));
				}
			}
			catch (Exception exp)
			{
				Logger.Error("Exception thrown while loading assembly", "AppConfig", exp);
			}
		}

		private void ConfigureModule(IExportRegistrationBlock registrationBlock,
			IConfigurationModule configurationModule,
			IEnumerable<PropetryElement> element)
		{
			foreach (PropetryElement propertyElement in element)
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