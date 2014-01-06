using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.ExampleApp.DependencyInjection.Fluent
{
	public class FluentSubModule : IExampleSubModule<DependencyInjectionExampleModule>
	{
		private List<IExample<FluentSubModule>> examples;

		public FluentSubModule(List<IExample<FluentSubModule>> examples)
		{
			this.examples = examples;

			this.examples.Sort((x,y) => string.Compare(x.GetType().Name,y.GetType().Name));
		}

		public void Execute()
		{
			foreach (IExample<FluentSubModule> example in examples)
			{
				example.ExecuteExample();
			}
		}
	}
}
