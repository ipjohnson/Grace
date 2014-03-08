using System.Configuration;

namespace Grace.DependencyInjection.Configuration
{
	/// <summary>
	/// Property element used in module
	/// </summary>
	public class PropetryElement : BaseElement
	{
		/// <summary>
		/// Name of property
		/// </summary>
		[ConfigurationProperty("name", IsRequired = true)]
		public string Name
		{
			get { return PropertyValue<string>(); }
		}

		/// <summary>
		/// property value
		/// </summary>
		[ConfigurationProperty("value", IsRequired = true)]
		public string Value
		{
			get { return PropertyValue<string>(); }
		}
	}
}