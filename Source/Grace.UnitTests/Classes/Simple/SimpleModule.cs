using Grace.DependencyInjection;

namespace Grace.UnitTests.Classes.Simple
{
	public class SimpleModule : IConfigurationModule
	{
		public void Configure(IExportRegistrationBlock registrationBlock)
		{
			registrationBlock.Export<BasicService>().As<IBasicService>();
		}
	}
}