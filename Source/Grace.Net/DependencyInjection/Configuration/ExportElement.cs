using System.Configuration;

namespace Grace.DependencyInjection.Configuration
{
	public class ExportElement : BaseElementCollection<AsElement>
	{
		public ExportElement() : base("as")
		{
		}

		[ConfigurationProperty("type", IsRequired = true)]
		public string Type
		{
			get { return PropertyValue<string>(); }
		}

		[ConfigurationProperty("externallyOwned", IsRequired = false, DefaultValue = false)]
		public bool ExternallyOwned
		{
			get { return PropertyValue<bool>(); }
		}

		[ConfigurationProperty("lifeStyle", IsRequired = false, DefaultValue = null)]
		public string LifeStyle
		{
			get { return PropertyValue<string>(); }
		}

		[ConfigurationProperty("environment", IsRequired = false, DefaultValue = ExportEnvironment.Any)]
		public ExportEnvironment Environment
		{
			get { return PropertyValue<ExportEnvironment>(); }
		}

		[ConfigurationProperty("autoWireProperties", IsRequired = false, DefaultValue = false)]
		public bool AutoWireProperties
		{
			get { return PropertyValue<bool>(); }
		}
	}
}