using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection.Configuration
{
	public class AssemblyElementCollection : BaseElementCollection<AssemblyElement>
	{
		public AssemblyElementCollection() : base("assembly")
		{
		}
	}
}
