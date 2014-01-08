using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection;

namespace Grace.ExampleApp
{
	class Program
	{
		static void Main(string[] args)
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer(comparer: DependencyInjectionContainer.CompareExportStrategies);

			container.Configure(c => c.Export(Types.FromThisAssembly()).
											  ByInterface(typeof(IExampleModule)).
											  ByInterface(typeof(IExampleSubModule<>)).
											  ByInterface(typeof(IExample<>)));

			IEnumerable<IExampleModule> exampleModules = container.LocateAll<IExampleModule>();

			try
			{
				foreach (IExampleModule exampleModule in exampleModules)
				{
					exampleModule.Execute();
				}
			}
			catch (Exception exp)
			{
				Console.WriteLine("Exception thrown");
				Console.WriteLine(exp.Message);
				throw;
			}
		}
	}
}
