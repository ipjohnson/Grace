using System.Configuration;

namespace Grace.DependencyInjection.Configuration
{
	public class AsElement : BaseElement
	{
		[ConfigurationProperty("type", IsRequired = false)]
		public string Type
		{
			get { return PropertyValue<string>(); }
		}

		[ConfigurationProperty("name", IsRequired = false)]
		public string Name
		{
			get { return PropertyValue<string>(); }
		}
	}
}