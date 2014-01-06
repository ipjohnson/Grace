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
	/// This exmaple shows how the constructor will automatically figure out how to import your constructor
	/// </summary>
	public class SimpleImportConstructor : IExample<FluentSubModule>
	{
		public void ExecuteExample()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c =>
										  {
											  c.Export<BasicService>().As<IBasicService>();
											  c.Export<ImportConstructor>().As<IImportConstructor>();
										  });

			IImportConstructor constructor = container.Locate<IImportConstructor>();

			if (constructor == null)
			{
				throw new Exception("constructor is null");
			}

			if (constructor.BasicService == null)
			{
				throw new Exception("basic service is null");
			}
		}
	}


	/// <summary>
	/// This example shows how to specify a specific value to a constructor parameter
	/// </summary>
	public class MixedParamerExample : IExample<FluentSubModule>
	{
		public void ExecuteExample()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c =>
			{
				c.Export<BasicService>().As<IBasicService>();
				c.Export<MixedParameterConstructor>().WithCtorParam(() => "Hello").WithCtorParam(() => 5);
			});

			MixedParameterConstructor mixed = container.Locate<MixedParameterConstructor>();

			if (mixed == null)
			{
				throw new Exception("mixed is null");
			}

			if (mixed.BasicService == null)
			{
				throw new Exception("basicService is null");
			}

			if (mixed.StringParam != "Hello" || mixed.IntParam != 5)
			{
				throw new Exception("mixed values wrong");
			}
		}
	}

	/// <summary>
	/// This is an example of limit the exports that can be considered for import into a constructor
	/// </summary>
	public class LimitImportsByAttributesExample : IExample<FluentSubModule>
	{
		public void ExecuteExample()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c =>
				                    {
											  c.Export(Types.FromThisAssembly()).ExportInterface(typeof(ISimpleObject));
					                    c.Export<ImportCollectionConstructor>().
					                      As<IImportCollectionConstructor>().
					                      WithCtorParam<IEnumerable<ISimpleObject>>().Consider(ExportsThat.HaveAttribute<SomeTestAttribute>());
				                    });

			IImportCollectionConstructor collectionConstructor = container.Locate<IImportCollectionConstructor>();

			if (collectionConstructor == null)
			{
				throw new Exception("multipleConstructor is null");
			}

			if (collectionConstructor.SimpleCount != 3)
			{
				throw new Exception("simpleCount should be 3");
			}
		}
	}
}
