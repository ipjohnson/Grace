using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection.Configuration
{
	public class ExportElement : BaseElementCollection<AsElement>
	{
		public ExportElement() : base("as")
		{
		}

		[ConfigurationProperty("type", IsRequired = true)]
		public string Type
		{
			get { return PropertyValue<string>(); }
		}

		[ConfigurationProperty("externallyOwned", IsRequired = false, DefaultValue = false)]
		public bool ExternallyOwned
		{
			get { return PropertyValue<bool>(); }
		}

		[ConfigurationProperty("lifeStyle", IsRequired = false)]
		public string LifeStyle
		{
			get { return PropertyValue<string>(); }
		}
	}
}
