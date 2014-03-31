using System;
using System.Configuration;
using System.Runtime.CompilerServices;

namespace Grace.DependencyInjection.Configuration
{
	/// <summary>
	/// Configuration section, used in app.config or web.config
	/// </summary>
	public class GraceConfigurationSection : ConfigurationSection
	{
		/// <summary>
		/// Assemblies to export
		/// </summary>
		[ConfigurationProperty("assemblies", IsRequired = false, IsDefaultCollection = true)]
		public AssemblyElementCollection Assemblies
		{
			get { return PropertyValue<AssemblyElementCollection>(); }
		}

		/// <summary>
		/// Plugins scans directories for exports
		/// </summary>
		[ConfigurationProperty("plugins", IsRequired = false, IsDefaultCollection = true)]
		public AssemblyDirectoryElementCollection Plugins
		{
			get { return PropertyValue<AssemblyDirectoryElementCollection>(); }
		}

		/// <summary>
		/// Modules to load
		/// </summary>
		[ConfigurationProperty("modules", IsRequired = false, IsDefaultCollection = true)]
		public ModuleElementCollection Modules
		{
			get { return PropertyValue<ModuleElementCollection>(); }
		}

		/// <summary>
		/// List of exports
		/// </summary>
		[ConfigurationProperty("exports", IsRequired = false, IsDefaultCollection = true)]
		public ExportElementCollection Exports
		{
			get { return PropertyValue<ExportElementCollection>(); }
		}

		/// <summary>
		/// Gets a property value
		/// </summary>
		/// <typeparam name="T">type to get</typeparam>
		/// <param name="propertyName">property name</param>
		/// <returns>T value</returns>
		protected T PropertyValue<T>([CallerMemberName] String propertyName = null)
		{
			if (propertyName == null)
			{
				throw new ArgumentNullException("propertyName");
			}

			string attributeName = Char.ToLowerInvariant(propertyName[0]) + propertyName.Substring(1);

			return (T)this[attributeName];
		}
	}
}