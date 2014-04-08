using System.Configuration;

namespace Grace.DependencyInjection.Configuration
{
	/// <summary>
	/// app.config element for loading an assembly
	/// </summary>
	public class AssemblyElement : BaseElement
	{
		/// <summary>
		/// Scan for types that have been attributed in assembly
		/// </summary>
		[ConfigurationProperty("scanForAttributes", DefaultValue = false, IsRequired = false)]
		public bool ScanForAttributes
		{
			get { return PropertyValue<bool>(); }
		}

		/// <summary>
		/// Path to assembly file
		/// </summary>
		[ConfigurationProperty("path", IsRequired = true)]
		public string Path
		{
			get { return PropertyValue<string>(); }
		}

		/// <summary>
		/// Interfaces to export
		/// </summary>
		[ConfigurationProperty("exportInterfaces", IsRequired = false)]
		public ExportInterfaceElementCollection ExportInterfaces
		{
			get { return PropertyValue<ExportInterfaceElementCollection>(); }
		}
	}
}