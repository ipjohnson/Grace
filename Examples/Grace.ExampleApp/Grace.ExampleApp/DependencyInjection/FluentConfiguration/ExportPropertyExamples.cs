using System;
using Grace.DependencyInjection;
using Grace.ExampleApp.DependencyInjection.ExampleClasses;

namespace Grace.ExampleApp.DependencyInjection.FluentConfiguration
{
	/// <summary>
	/// This example shows how to export a property as a transient
	/// </summary>
	public class ExportPropertyExample : IExample<FluentConfigurationSubModule>
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
	public class ExportPropertySingletonExample : IExample<FluentConfigurationSubModule>
	{
		public void ExecuteExample()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c => c.Export<ExportProperty>().ExportProperty(p => p.BasicService).Lifestyle.Singleton());

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
