using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection.Configuration
{
	public class GraceConfigurationSection : ConfigurationSection
	{


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
