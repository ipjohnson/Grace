using System.Configuration;

namespace Grace.DependencyInjection.Configuration
{
	/// <summary>
	/// App config element
	/// </summary>
	public class AsElement : BaseElement
	{
		/// <summary>
		/// export as a type
		/// </summary>
		[ConfigurationProperty("type", IsRequired = false)]
		public string Type
		{
			get { return PropertyValue<string>(); }
		}

		/// <summary>
		/// export as a name
		/// </summary>
		[ConfigurationProperty("name", IsRequired = false)]
		public string Name
		{
			get { return PropertyValue<string>(); }
		}
	}
}