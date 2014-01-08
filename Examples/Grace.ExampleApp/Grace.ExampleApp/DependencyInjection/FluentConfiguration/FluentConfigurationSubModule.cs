using System.Collections.Generic;

namespace Grace.ExampleApp.DependencyInjection.FluentConfiguration
{
	public class FluentConfigurationSubModule : IExampleSubModule<DependencyInjectionExampleModule>
	{
		private List<IExample<FluentConfigurationSubModule>> examples;

		public FluentConfigurationSubModule(List<IExample<FluentConfigurationSubModule>> examples)
		{
			this.examples = examples;

			this.examples.Sort((x,y) => string.Compare(x.GetType().Name,y.GetType().Name));
		}

		public void Execute()
		{
			foreach (IExample<FluentConfigurationSubModule> example in examples)
			{
				example.ExecuteExample();
			}
		}
	}
}
