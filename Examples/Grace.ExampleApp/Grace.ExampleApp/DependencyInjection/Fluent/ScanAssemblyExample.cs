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
	/// This example shows how to scan an assembly for types
	/// </summary>
	public class SimpleScanExample : IExample<FluentSubModule>
	{
		public void ExecuteExample()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c => c.Export(Types.FromThisAssembly()).ExportInterfaces());

			IImportCollectionConstructor importCollection = container.Locate<IImportCollectionConstructor>();

			if (importCollection == null)
			{
				throw new Exception("importMultiple is null");
			}

			if (importCollection.SimpleCount != 5)
			{
				throw new Exception("simple count should be 5");
			}
		}
	}

	/// <summary>
	/// This example shows how to export only interfaces that start with a particular name
	/// </summary>
	public class FilterExportInterfacesExample : IExample<FluentSubModule>
	{
		public void ExecuteExample()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c =>
										  {
											  c.Export(Types.FromThisAssembly())
												.ExportInterfaces(TypesThat.StartWith("ISimple"));
											  c.Export<ImportCollectionConstructor>().As<IImportCollectionConstructor>();
										  });

			IImportCollectionConstructor importCollection = container.Locate<IImportCollectionConstructor>();

			if (importCollection == null)
			{
				throw new Exception("importMultiple is null");
			}

			if (importCollection.SimpleCount != 5)
			{
				throw new Exception("simple count should be 5");
			}
		}
	}

	/// <summary>
	/// This example shows how you might limit your exported type to only attributed types
	/// </summary>
	public class ExportAttributedClasses : IExample<FluentSubModule>
	{
		public void ExecuteExample()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c =>
										  {
											  c.Export(Types.FromThisAssembly())
												.ExportInterfaces()
												.Select(TypesThat.HaveAttribute<SomeTestAttribute>());
											  c.Export<ImportCollectionConstructor>().As<IImportCollectionConstructor>();
										  });

			IImportCollectionConstructor importCollection = container.Locate<IImportCollectionConstructor>();

			if (importCollection == null)
			{
				throw new Exception("importMultiple is null");
			}

			if (importCollection.SimpleCount != 3)
			{
				throw new Exception("simple count should be 3");
			}
		}
	}
}
