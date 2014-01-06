using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.ExampleApp.DependencyInjection.ExampleClasses
{
	public class ExportProperty
	{
		private IBasicService basicService = new BasicService();

		public IBasicService BasicService
		{
			get { return basicService; }
		}
	}
}
