using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.ExampleApp.DependencyInjection.Attributes
{
	public class AttributeSubModule : IExampleSubModule<DependencyInjectionExampleModule>
	{
		private readonly IEnumerable<IExample<AttributeSubModule>> examples;

		public AttributeSubModule(IEnumerable<IExample<AttributeSubModule>> examples)
		{
			this.examples = examples;
		}

		public void Execute()
		{
			foreach (IExample<AttributeSubModule> example in examples)
			{
				example.ExecuteExample();
			}
		}
	}
}
