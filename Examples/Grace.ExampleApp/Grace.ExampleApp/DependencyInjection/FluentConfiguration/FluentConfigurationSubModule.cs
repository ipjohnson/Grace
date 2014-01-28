using System.Collections.Generic;

namespace Grace.ExampleApp.DependencyInjection.FluentConfiguration
{
	public class FluentConfigurationSubModule : IExampleSubModule<DependencyInjectionExampleModule>
	{
		private List<IExample<FluentConfigurationSubModule>> examples;

		public FluentConfigurationSubModule(List<IExample<FluentConfigurationSubModule>> examples)
		{
			this.examples = examples;
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
