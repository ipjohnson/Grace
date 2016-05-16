using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl;
using Grace.UnitTests.Classes.Simple;
using Xunit;

namespace Grace.UnitTests.DependencyInjection.Impl
{
	public class InjectionKernelManagerTests
	{
		[Fact]
		public void NamedCloneTest()
		{
			InjectionKernelManager manager = new InjectionKernelManager(null,
				DependencyInjectionContainer.CompareExportStrategies);
			InjectionKernel kernel = new InjectionKernel(manager,
				null,
				null,
				"RootScope",
				DependencyInjectionContainer.CompareExportStrategies);

			manager.SetRootScope(kernel);

			manager.Configure("TestKernel", c => c.Export<BasicService>().As<IBasicService>());

			IInjectionScope injectionScope = manager.CreateNewKernel(kernel, "TestKernel", null, null, null);

			IBasicService basicService = injectionScope.Locate<IBasicService>();

			Assert.NotNull(basicService);
		}

		[Fact]
		public void NamedCloneWithRegistrationTest()
		{
			InjectionKernelManager manager = new InjectionKernelManager(null,
				DependencyInjectionContainer.CompareExportStrategies);
			InjectionKernel kernel = new InjectionKernel(manager,
				null,
				null,
				"RootScope",
				DependencyInjectionContainer.CompareExportStrategies);

			manager.SetRootScope(kernel);

			manager.Configure("TestKernel", c => c.Export<BasicService>().As<IBasicService>());

			IInjectionScope injectionScope =
				manager.CreateNewKernel(kernel,
					"TestKernel",
					c => c.Export<ImportConstructorService>().As<IImportConstructorService>(),
					null,
					null);

			IImportConstructorService importService = injectionScope.Locate<IImportConstructorService>();

			Assert.NotNull(importService);
		}

		[Fact]
		public void NonNamedCloneTest()
		{
			InjectionKernelManager manager = new InjectionKernelManager(null,
				DependencyInjectionContainer.CompareExportStrategies);
			InjectionKernel kernel = new InjectionKernel(manager,
				null,
				null,
				"RootScope",
				DependencyInjectionContainer.CompareExportStrategies);

			kernel.Configure(c => c.Export<BasicService>().As<IBasicService>());

			IInjectionScope injectionScope = manager.CreateNewKernel(kernel, null, null, null, null);

			IBasicService basicService = injectionScope.Locate<IBasicService>();

			Assert.NotNull(basicService);
		}

		[Fact]
		public void NonNamedCloneWithRegistrationTest()
		{
            InjectionKernelManager manager = new InjectionKernelManager(null,
                DependencyInjectionContainer.CompareExportStrategies);
			InjectionKernel kernel = new InjectionKernel(manager,
				null,
				null,
				"RootScope",
				DependencyInjectionContainer.CompareExportStrategies);

			kernel.Configure(c => c.Export<BasicService>().As<IBasicService>());

			IInjectionScope injectionScope =
				manager.CreateNewKernel(kernel,
					null,
					c => c.Export<ImportConstructorService>().As<IImportConstructorService>(),
					null,
					null);

			IImportConstructorService importService = injectionScope.Locate<IImportConstructorService>();

			Assert.NotNull(importService);
		}
	}
}