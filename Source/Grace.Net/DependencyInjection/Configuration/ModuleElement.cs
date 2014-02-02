using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection.Configuration
{
	public class ModuleElement : BaseElementCollection<PropetryElement>
	{
		public ModuleElement() : base("property")
		{
		}

		[ConfigurationProperty("type", IsRequired = true)]
		public string Type
		{
			get { return PropertyValue<string>(); }
		}
	}
}
