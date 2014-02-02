using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection.Configuration
{
	public class ModuleElement : BaseElement
	{
		[ConfigurationProperty("type", IsRequired = true)]
		public string Type
		{
			get { return PropertyValue<string>(); }
		}

		[ConfigurationProperty("propetries", IsRequired = false)]
		public PropetryElementCollection Propetries
		{
			get { return PropertyValue<PropetryElementCollection>(); }
		}
	}
}
