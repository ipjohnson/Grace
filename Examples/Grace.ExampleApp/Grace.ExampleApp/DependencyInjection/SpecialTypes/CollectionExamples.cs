using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.ExampleApp.DependencyInjection.ExampleClasses;

namespace Grace.ExampleApp.DependencyInjection.SpecialTypes
{
	/// <summary>
	/// This example resolves a ReadOnlyCollection
	/// IReadOnlyList and IReadOnlyCollection are supported as well
	/// </summary>
	public class ReadOnlyCollectionExample : IExample<SpecialTypesSubModule>
	{
		public void ExecuteExample()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c => c.Export(Types.FromThisAssembly()).ByInterface(typeof(ISimpleObject)));

			ReadOnlyCollection<ISimpleObject> simpleObjects = container.Locate<ReadOnlyCollection<ISimpleObject>>();

			if (simpleObjects == null)
			{
				throw new Exception("simpleObjects should not be null");
			}

			if (simpleObjects.Count != 5)
			{
				throw new Exception("simpleObject should be 5");
			}
		}
	}

	/// <summary>
	/// This example shows how to import an array
	/// </summary>
	public class ArrayExample : IExample<SpecialTypesSubModule>
	{
		public void ExecuteExample()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c => c.Export(Types.FromThisAssembly()).ByInterface(typeof(ISimpleObject)));

			ISimpleObject[] simpleObjects = container.Locate<ISimpleObject[]>();

			if (simpleObjects == null)
			{
				throw new Exception("simpleObjects should not be null");
			}

			if (simpleObjects.Length != 5)
			{
				throw new Exception("simpleObjects.Length should be 5");
			}
		}
	}

	/// <summary>
	/// This example shows how you can locate an ObservableCollection
	/// In fact you can import any collection that implements ICollection(T), it must have an empty constructor
	/// </summary>
	public class ObservableCollectionExample : IExample<SpecialTypesSubModule>
	{
		public void ExecuteExample()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c => c.Export(Types.FromThisAssembly()).ByInterface(typeof(ISimpleObject)));

			ObservableCollection<ISimpleObject> simpleObjects = container.Locate<ObservableCollection<ISimpleObject>>();

			if (simpleObjects == null)
			{
				throw new Exception("simpleObjects should not be null");
			}

			if (simpleObjects.Count != 5)
			{
				throw new Exception("simpleObjects.Count should be 5");
			}
		}
	}

	/// <summary>
	/// In this example it's filtering the items that get resolved based on if the export has an attribute
	/// </summary>
	public class FilterCollectionExample : IExample<SpecialTypesSubModule>
	{
		public void ExecuteExample()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c => c.Export(Types.FromThisAssembly()).ByInterface(typeof(ISimpleObject)));

			ObservableCollection<ISimpleObject> simpleObjects = 
				container.Locate<ObservableCollection<ISimpleObject>>(consider: ExportsThat.HaveAttribute<SomeTestAttribute>());

			if (simpleObjects == null)
			{
				throw new Exception("simpleObjects should not be null");
			}

			if (simpleObjects.Count != 3)
			{
				throw new Exception("simpleObjects.Count should be 3");
			}
		}
	}
}
