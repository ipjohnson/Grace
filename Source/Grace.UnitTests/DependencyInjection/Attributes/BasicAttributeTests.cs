using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl;
using Grace.UnitTests.Classes.Attributed;
using Xunit;

namespace Grace.UnitTests.DependencyInjection.Attributes
{
	public class BasicAttributeTests
	{
		[Fact]
		public void BasicServiceTest()
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

			IAttributedExportService basicService =
				injectionKernel.Locate<IAttributedExportService>();

			Assert.NotNull(basicService);
		}

		[Fact]
		public void ImportCostructorTest()
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

			IAttributeImportConstructorService basicService =
				injectionKernel.Locate<IAttributeImportConstructorService>();

			Assert.NotNull(basicService);
		}

		[Fact]
		public void ImportPropertyTest()
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

			IAttributedImportPropertyService propertyService = injectionKernel.Locate<IAttributedImportPropertyService>();

			Assert.NotNull(propertyService);
			Assert.NotNull(propertyService.BasicService);
		}

		[Fact]
		public void ImportMethodTest()
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

			IAttributedImportMethodService attributedImport =
				injectionKernel.Locate<IAttributedImportMethodService>();

			Assert.NotNull(attributedImport);
			Assert.NotNull(attributedImport.BasicService);
		}

		[Fact]
		public void ActivationMethodTest()
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

			IAttributedActivationService activationService =
				injectionKernel.Locate<IAttributedActivationService>();

			Assert.NotNull(activationService);
			Assert.True(activationService.ContextActivationCalled);
			Assert.True(activationService.SimpleActivationCalled);
		}

		[Fact]
		public void ComplexTest()
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

			IComplexService complexService =
				injectionKernel.Locate<IComplexService>();

			complexService.Validate();
		}
	}
}