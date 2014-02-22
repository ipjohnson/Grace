using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl;
using Grace.UnitTests.Classes.Attributed;
using Xunit;

namespace Grace.UnitTests.DependencyInjection.Attributes
{
	public class OpenGenericTests
	{
		[Fact]
		public void AttributedOpenGenericTest()
		{
			InjectionKernelManager manager = new InjectionKernelManager(null,
				DependencyInjectionContainer.CompareExportStrategies,
				new BlackList());

			InjectionKernel injectionKernel = new InjectionKernel(manager,
				null,
				null,
				"RootScope",
				DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c => c.ExportAssembly(GetType().Assembly));

			IAttributedOpenGenericTransient<int> transient =
				injectionKernel.Locate<IAttributedOpenGenericTransient<int>>();

			Assert.NotNull(transient);
		}
	}
}