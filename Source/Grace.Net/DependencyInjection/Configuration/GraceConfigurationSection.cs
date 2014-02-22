using System;
using System.Configuration;
using System.Runtime.CompilerServices;

namespace Grace.DependencyInjection.Configuration
{
	public class GraceConfigurationSection : ConfigurationSection
	{
		[ConfigurationProperty("assemblies", IsRequired = false, IsDefaultCollection = true)]
		public AssemblyElementCollection Assemblies
		{
			get { return PropertyValue<AssemblyElementCollection>(); }
		}

		[ConfigurationProperty("modules", IsRequired = false, IsDefaultCollection = true)]
		public ModuleElementCollection Modules
		{
			get { return PropertyValue<ModuleElementCollection>(); }
		}

		[ConfigurationProperty("exports", IsRequired = false, IsDefaultCollection = true)]
		public ExportElementCollection Exports
		{
			get { return PropertyValue<ExportElementCollection>(); }
		}

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