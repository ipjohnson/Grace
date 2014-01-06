using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.ExampleApp.DependencyInjection.ExampleClasses;

namespace Grace.ExampleApp.DependencyInjection.Fluent
{
	/// <summary>
	/// This example shows how to export a property as a transient
	/// </summary>
	public class ExportPropertyExample : IExample<FluentSubModule>
	{
		public void ExecuteExample()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c => c.Export<ExportProperty>().ExportProperty(p => p.BasicService));

			IBasicService basicService = container.Locate<IBasicService>();

			if (basicService == null)
			{
				throw new Exception("Basic service should not be null");
			}
		}
	}

	/// <summary>
	/// This example shows how to export a property as a singleton
	/// </summary>
	public class ExportPropertySingletonExample : IExample<FluentSubModule>
	{
		public void ExecuteExample()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c => c.Export<ExportProperty>().ExportProperty(p => p.BasicService).AndSingleton());

			IBasicService basicService = container.Locate<IBasicService>();

			if (basicService == null)
			{
				throw new Exception("Basic service should not be null");
			}

			if (basicService != container.Locate<IBasicService>())
			{
				throw new Exception("Should be same instance");
			}
		}
	}
}
