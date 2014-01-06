using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection;

namespace Grace.ExampleApp.DependencyInjection.ExampleClasses
{
	public class DependencyInjectionContainerImportService
	{
		public DependencyInjectionContainerImportService(IDependencyInjectionContainer container)
		{
			Container = container;
		}

		public IDependencyInjectionContainer Container { get; private set; }
	}
}
