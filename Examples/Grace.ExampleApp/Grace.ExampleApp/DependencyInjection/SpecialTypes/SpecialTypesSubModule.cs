using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.ExampleApp.DependencyInjection.SpecialTypes
{
	public class SpecialTypesSubModule : IExampleSubModule<DependencyInjectionExampleModule>
	{
		private List<IExample<SpecialTypesSubModule>> examples;

		public SpecialTypesSubModule(List<IExample<SpecialTypesSubModule>> examples)
		{
			this.examples = examples;

			this.examples.Sort((x, y) => string.Compare(x.GetType().Name, y.GetType().Name));
		}

		public void Execute()
		{
			foreach (IExample<SpecialTypesSubModule> example in examples)
			{
				example.ExecuteExample();
			}
		}
	}
}
