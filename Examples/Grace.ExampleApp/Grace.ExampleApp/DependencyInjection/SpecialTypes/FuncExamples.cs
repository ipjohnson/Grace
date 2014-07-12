using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.ExampleApp.DependencyInjection.ExampleClasses;

namespace Grace.ExampleApp.DependencyInjection.SpecialTypes
{
	/// <summary>
	/// This example shows how to import a Func(T)
	/// </summary>
	public class FuncExample : IExample<SpecialTypesSubModule>
	{
		public void ExecuteExample()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c => c.Export<BasicService>().As<IBasicService>());

			Func<IBasicService> basicServiceFunc = container.Locate<Func<IBasicService>>();

			IBasicService basicService = basicServiceFunc();

			if (basicService == null)
			{
				throw new Exception("basicService should not be null");
			}
		}
	}

}
