using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection.Configuration;

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
