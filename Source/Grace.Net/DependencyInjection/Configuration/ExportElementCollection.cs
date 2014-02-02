using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection.Configuration
{
	public class ExportElementCollection : BaseElementCollection<ExportElement>
	{
		public ExportElementCollection() : base("export")
		{
		}
	}
}
