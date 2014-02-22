using System.Configuration;

namespace Grace.DependencyInjection.Configuration
{
	public class ModuleElement : BaseElementCollection<PropetryElement>
	{
		public ModuleElement() : base("property")
		{
		}

		[ConfigurationProperty("type", IsRequired = true)]
		public string Type
		{
			get { return PropertyValue<string>(); }
		}
	}
}