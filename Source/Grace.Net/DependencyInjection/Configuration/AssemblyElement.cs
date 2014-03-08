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
			get { return (bool)this["scanForAttributes"]; }
		}

		/// <summary>
		/// Path to assembly file
		/// </summary>
		[ConfigurationProperty("path", IsRequired = true)]
		public string Path
		{
			get { return (string)this["path"]; }
		}
	}
}