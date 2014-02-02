using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection.Configuration
{
	public class PropetryElement : BaseElement
	{
		[ConfigurationProperty("name",IsRequired = true)]
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
