using System;
using Grace.DependencyInjection;
using Grace.ExampleApp.DependencyInjection.ExampleClasses;

namespace Grace.ExampleApp.DependencyInjection.FluentConfiguration
{
	/// <summary>
	/// This example shows how to register a transient type as a particular interface
	/// </summary>
	public class ExportByTypeExample : IExample<FluentConfigurationSubModule>
	{
		public void ExecuteExample()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c => c.Export<BasicService>().As<IBasicService>());

			IBasicService basicService = container.Locate<IBasicService>();

			if (basicService == null)
			{
				throw new Exception("Basic service should not be null");
			}
		}
	}

	/// <summary>
	/// This example shows how to export a type by a particular name
	/// </summary>
	public class ExportWithNameExample : IExample<FluentConfigurationSubModule>
	{
		public void ExecuteExample()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c => c.Export<BasicService>().AsName("IBasicService"));

			IBasicService basicService = container.Locate("IBasicService") as IBasicService;

			if (basicService == null)
			{
				throw new Exception("Basic service should not be null");
			}
		}
	}

	/// <summary>
	/// In this example two types are being exported with a type and a key
	/// </summary>
	public class RegisterWithKeyExample : IExample<FluentConfigurationSubModule>
	{
		public void ExecuteExample()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c =>
									  {
										  c.Export<SimpleObjectA>().As<ISimpleObject>().WithKey(5);
										  c.Export<SimpleObjectB>().As<ISimpleObject>().WithKey(10);
									  });

			ISimpleObject simpleA = container.LocateByKey<ISimpleObject, int>(5);

			if (simpleA == null)
			{
				throw new Exception("simpleA should not be null");
			}

			if (!(simpleA is SimpleObjectA))
			{
				throw new Exception("simple should be SimpleObjectA");
			}

			ISimpleObject simpleB = container.LocateByKey<ISimpleObject, int>(10);

			if (simpleB == null)
			{
				throw new Exception("simpleB should not be null");
			}

			if (!(simpleB is SimpleObjectB))
			{
				throw new Exception("simple should be SimpleObjectB");
			}
		}
	}
}
