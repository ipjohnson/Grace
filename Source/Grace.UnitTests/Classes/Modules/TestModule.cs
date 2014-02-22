using Grace.DependencyInjection;
using Grace.UnitTests.Classes.Simple;

namespace Grace.UnitTests.Classes.Modules
{
	public class TestModule : IConfigurationModule
	{
		public string StringProperty { get; set; }

		public int IntProperty { get; set; }

		public void Configure(IExportRegistrationBlock registrationBlock)
		{
			registrationBlock.Export<BasicService>().As<IBasicService>();

			registrationBlock.ExportInstance(IntProperty).AsName("IntProperty");
		}
	}
}