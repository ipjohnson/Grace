using System;
using Grace.DependencyInjection;
using Grace.ExampleApp.DependencyInjection.AttributedExampleClasses;

namespace Grace.ExampleApp.DependencyInjection.Attributes
{
	public class ConditionalExportExample : IExample<AttributeSubModule>
	{
		public void ExecuteExample()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c => c.Export(Types.FromThisAssembly()));

			ISomeInterface someInterface = container.Locate<ISomeInterface>();

			Console.WriteLine("ISomeInterface implemented by: " + someInterface.GetType().FullName);
		}
	}
}
