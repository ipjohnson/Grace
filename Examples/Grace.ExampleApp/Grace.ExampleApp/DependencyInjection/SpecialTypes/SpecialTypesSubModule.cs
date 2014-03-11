using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.ExampleApp.DependencyInjection.SpecialTypes
{
	public class SpecialTypesSubModule : IExampleSubModule<DependencyInjectionExampleModule>
	{
		private IEnumerable<IExample<SpecialTypesSubModule>> examples;

		public SpecialTypesSubModule(IEnumerable<IExample<SpecialTypesSubModule>> examples)
		{
			this.examples = examples;
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
