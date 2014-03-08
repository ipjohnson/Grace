using System.Configuration;

namespace Grace.DependencyInjection.Configuration
{
	/// <summary>
	/// Represents a module in an app.config or web.config
	/// </summary>
	public class ModuleElement : BaseElementCollection<PropetryElement>
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		public ModuleElement() : base("property")
		{
		}

		/// <summary>
		/// Type of module to load
		/// </summary>
		[ConfigurationProperty("type", IsRequired = true)]
		public string Type
		{
			get { return PropertyValue<string>(); }
		}
	}
}