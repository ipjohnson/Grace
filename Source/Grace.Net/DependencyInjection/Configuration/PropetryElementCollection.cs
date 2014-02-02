using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection.Configuration
{
	public class PropetryElementCollection : BaseElementCollection<PropetryElement>
	{
		public PropetryElementCollection() : base("property")
		{
		}
	}
}
