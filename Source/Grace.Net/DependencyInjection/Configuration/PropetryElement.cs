using System.Configuration;

namespace Grace.DependencyInjection.Configuration
{
	public class PropetryElement : BaseElement
	{
		[ConfigurationProperty("name", IsRequired = true)]
		public string Name
		{
			get { return PropertyValue<string>(); }
		}

		[ConfigurationProperty("value", IsRequired = true)]
		public string Value
		{
			get { return PropertyValue<string>(); }
		}
	}
}