using System.Configuration;

namespace Grace.DependencyInjection.Configuration
{
	public class AssemblyElement : BaseElement
	{
		[ConfigurationProperty("scanForAttributes", DefaultValue = false, IsRequired = false)]
		public bool ScanForAttributes
		{
			get { return (bool)this["scanForAttributes"]; }
		}

		[ConfigurationProperty("path", IsRequired = true)]
		public string Path
		{
			get { return (string)this["path"]; }
		}
	}
}