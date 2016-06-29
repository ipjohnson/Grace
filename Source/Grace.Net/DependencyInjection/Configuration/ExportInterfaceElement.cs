using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection.Configuration
{
	/// <summary>
	/// Configuration element that represents an interface to export
	/// </summary>
	public class ExportInterfaceElement : BaseElement
	{
		/// <summary>
		/// Type to export
		/// </summary>
		[ConfigurationProperty("type", IsRequired = true)]
		public string Type
		{
			get { return PropertyValue<string>(); }
		}

		/// <summary>
		/// Is the type externally owned
		/// </summary>
		[ConfigurationProperty("externallyOwned", IsRequired = false, DefaultValue = false)]
		public bool ExternallyOwned
		{
			get { return PropertyValue<bool>(); }
		}

		/// <summary>
		/// Lifestyle associated with this export
		/// </summary>
		[ConfigurationProperty("lifeStyle", IsRequired = false, DefaultValue = null)]
		public string LifeStyle
		{
			get { return PropertyValue<string>(); }
		}

		/// <summary>
		/// Autowire the objects properties
		/// </summary>
		[ConfigurationProperty("autoWireProperties", IsRequired = false, DefaultValue = false)]
		public bool AutoWireProperties
		{
			get { return PropertyValue<bool>(); }
		}
	}
}
