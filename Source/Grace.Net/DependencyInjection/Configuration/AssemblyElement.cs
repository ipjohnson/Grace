using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection.Configuration
{
	public class AssemblyElement : BaseElement
	{
		[ConfigurationProperty("scanForAttributes",DefaultValue = false,IsRequired = false)]
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
