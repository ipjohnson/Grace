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
	/// This example shows that when there are multiple constructors and one is not specified to be used.
	/// Then the constructor with the most args will be chosen
	/// </summary>
	public class DefaultMultipleConstructorExample : IExample<FluentSubModule>
	{
		public void ExecuteExample()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c =>
			                    {
				                    c.Export<BasicService>().As<IBasicService>();
				                    c.Export<SimpleObjectA>().As<ISimpleObject>();
				                    c.Export<MultipleConstructor>();
			                    });

			MultipleConstructor constructor = container.Locate<MultipleConstructor>();

			if (constructor == null)
			{
				throw new Exception("constructor should not be null");
			}

			if (constructor.BasicService == null)
			{
				throw new Exception("BasicService should not be null");
			}

			if (constructor.SimpleObject == null)
			{
				throw new Exception("SimpleObject should not be null");
			}
		}

		/// <summary>
		/// In this example the constructor to use is being specified.
		/// </summary>
		public class UseSpecificConstructorExample : IExample<FluentSubModule>
		{
			public void ExecuteExample()
			{
				DependencyInjectionContainer container = new DependencyInjectionContainer();

				container.Configure(c =>
				{
					c.Export<BasicService>().As<IBasicService>();
					c.Export<SimpleObjectA>().As<ISimpleObject>();
					c.Export<MultipleConstructor>().ImportConstructor(() => new MultipleConstructor(null));
				});

				MultipleConstructor constructor = container.Locate<MultipleConstructor>();

				if (constructor == null)
				{
					throw new Exception("constructor should not be null");
				}

				if (constructor.BasicService == null)
				{
					throw new Exception("BasicService should not be null");
				}

				if (constructor.SimpleObject != null)
				{
					throw new Exception("SimpleObject should be null");
				}	
			}
		}
	}
}
