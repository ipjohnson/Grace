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
				null);

			InjectionKernel injectionKernel = new InjectionKernel(manager,
                null,
                "RootScope",
                new KernelConfiguration());

            injectionKernel.Configure(c => c.ExportAssembly(GetType().Assembly));

			IAttributedOpenGenericTransient<int> transient =
				injectionKernel.Locate<IAttributedOpenGenericTransient<int>>();

			Assert.NotNull(transient);
		}
	}
}