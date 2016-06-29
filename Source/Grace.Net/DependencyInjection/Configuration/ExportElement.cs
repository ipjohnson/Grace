using System.Configuration;

namespace Grace.DependencyInjection.Configuration
{
	/// <summary>
	/// Exports a type as a particular interface or name
	/// </summary>
	public class ExportElement : BaseElementCollection<AsElement>
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		public ExportElement() : base("as")
		{
		}

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