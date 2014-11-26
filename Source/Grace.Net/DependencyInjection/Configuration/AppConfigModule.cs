using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using Grace.Data.Immutable;
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

			graceSection = ConfigurationManager.GetSection(SectionName) as GraceConfigurationSection;

			if (graceSection != null)
			{
				AssemblyElementCollection assemblies = graceSection.Assemblies;

				foreach (AssemblyElement assemblyElement in assemblies)
				{
					ProcessAssembly(registrationBlock, assemblyElement);
				}

				AssemblyDirectoryElementCollection directories = graceSection.Plugins;

				foreach (AssemblyDirectoryElement assemblyDirectoryElement in directories)
				{
					ProcessDirecotry(registrationBlock, assemblyDirectoryElement);
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
				}
			}
		}

		private void ProcessAssembly(IExportRegistrationBlock registrationBlock, AssemblyElement assemblyElement)
		{
			AssemblyName assemblyName = AssemblyName.GetAssemblyName(assemblyElement.Path);

			Assembly newAssembly = Assembly.Load(assemblyName);

			if (assemblyElement.ScanForAttributes)
			{
				registrationBlock.Export(Types.FromAssembly(newAssembly));
			}
			else
			{
				ExportInterfaceElementCollection exportInterfaces = assemblyElement.ExportInterfaces;

				if (exportInterfaces != null && exportInterfaces.Count > 0)
				{
					ProcessExportInterfaces(registrationBlock, exportInterfaces, Types.FromAssembly(newAssembly));
				}
			}
		}

		private void ProcessDirecotry(IExportRegistrationBlock registrationBlock, AssemblyDirectoryElement assemblyDirectoryElement)
		{
			var assemblyFiles = Directory.GetFiles(assemblyDirectoryElement.Path, "*.dll");

			foreach (string assemblyFile in assemblyFiles)
			{
				AssemblyName assemblyName = AssemblyName.GetAssemblyName(assemblyFile);

				Assembly newAssembly = Assembly.Load(assemblyName);

				if (assemblyDirectoryElement.ScanForAttributes)
				{
					registrationBlock.Export(Types.FromAssembly(newAssembly));
				}
				else
				{
					ExportInterfaceElementCollection exportInterfaces = assemblyDirectoryElement.ExportInterfaces;

					if (exportInterfaces != null && exportInterfaces.Count > 0)
					{
						ProcessExportInterfaces(registrationBlock, exportInterfaces, Types.FromAssembly(newAssembly));
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

				ILifestyle lifeStyle = ConvertStringToLifestyle(exportElement.LifeStyle);

				if (lifeStyle != null)
				{
					config.UsingLifestyle(lifeStyle);
				}
			}
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
					AppDomain.CurrentDomain.GetAssemblies().
													SortEnumerable(SortAssemblies).
													ReverseEnumerable().
													SelectMany(x => x.IsDynamic ? ImmutableArray<Type>.Empty : x.ExportedTypes). 
													FirstOrDefault(x => x.Name.ToLowerInvariant() == lower);
			}

			return returnType;
		}

		private static int SortAssemblies(Assembly x, Assembly y)
		{
			if (x.GlobalAssemblyCache && y.GlobalAssemblyCache ||
				!x.GlobalAssemblyCache && !y.GlobalAssemblyCache)
			{
				bool xSystemLib = IsSystemLibrary(x.FullName);
				bool ySystemLib = IsSystemLibrary(y.FullName);

				if (xSystemLib && ySystemLib ||
					 !xSystemLib && !ySystemLib)
				{
					return string.Compare(y.FullName, x.FullName, StringComparison.CurrentCultureIgnoreCase);
				}

				if (xSystemLib)
				{
					return -1;
				}

				return 1;
			}

			if (x.GlobalAssemblyCache)
			{
				return -1;
			}

			return 1;
		}

		private void ProcessExportInterfaces(IExportRegistrationBlock registrationBlock,
														 ExportInterfaceElementCollection exportInterfaces,
														 IEnumerable<Type> exportTypes)
		{
			foreach (ExportInterfaceElement exportInterfaceElement in exportInterfaces)
			{
				Type interfaceType = ConvertStringToType(exportInterfaceElement.Type);
				ILifestyle lifestyle = ConvertStringToLifestyle(exportInterfaceElement.LifeStyle);

				var config = registrationBlock.Export(exportTypes).
														 ByInterface(interfaceType);

				if (lifestyle != null)
				{
					config.UsingLifestyle(lifestyle);
				}

				if (exportInterfaceElement.ExternallyOwned)
				{
					config.ExternallyOwned();
				}
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
						finalValue = Convert.ChangeType(propertyElement.Value, propertyInfo.PropertyType);
					}

					if (finalValue != null)
					{
						propertyInfo.SetValue(configurationModule, finalValue);
					}
				}
			}

			configurationModule.Configure(registrationBlock);
		}

		private static bool IsSystemLibrary(string assemblyName)
		{
			return assemblyName == "mscorlib" || assemblyName.StartsWith("System.");
		}

		private ILifestyle ConvertStringToLifestyle(string lifeStyle)
		{
			if (string.IsNullOrEmpty(lifeStyle))
			{
				switch (lifeStyle)
				{
					case "Singleton":
						return new SingletonLifestyle();

					case "WeakSingleton":
						return new WeakSingletonLifestyle();

					case "SingletonPerInjection":
					case "PerInjection":
						return new SingletonPerInjectionContextLifestyle();

					case "SingletonPerScope":
					case "PerScope":
						return new SingletonPerScopeLifestyle();

					case "SingletonPerRequest":
					case "PerRequest":
						return new SingletonPerRequestLifestyle();

					default:
						Type lifeStyleType = ConvertStringToType(lifeStyle);

						if (lifeStyleType != null)
						{
							return Activator.CreateInstance(lifeStyleType) as ILifestyle;
						}
						break;
				}
			}

			return null;
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
	}
}