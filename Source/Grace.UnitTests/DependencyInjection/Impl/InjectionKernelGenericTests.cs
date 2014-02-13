using System.Collections.Generic;
using System.Linq;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl;
using Grace.UnitTests.Classes.Simple;
using Xunit;

namespace Grace.UnitTests.DependencyInjection.Impl
{
	public class InjectionKernelGenericTests
	{
		[Fact]
		public void BasicGenericTest()
		{
			InjectionKernelManager injectionKernelManager =
				new InjectionKernelManager(null, DependencyInjectionContainer.CompareExportStrategies, new BlackList());
			InjectionKernel injectionKernel =
				new InjectionKernel(injectionKernelManager,
					null,
					null,
					"RootScope",
					DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c => c.Export(typeof(GenericService<>)).As(typeof(IGenericService<>)));

			IGenericService<int> service = injectionKernel.Locate<IGenericService<int>>();

			Assert.NotNull(service);
		}

		[Fact]
		public void ImportGenericTest()
		{
			InjectionKernelManager injectionKernelManager =
				new InjectionKernelManager(null, DependencyInjectionContainer.CompareExportStrategies, new BlackList());
			InjectionKernel injectionKernel =
				new InjectionKernel(injectionKernelManager,
					null,
					null,
					"RootScope",
					DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c =>
			                          {
				                          c.Export(typeof(GenericService<>)).As(typeof(IGenericService<>));
				                          c.Export(typeof(GenericTransient<>)).As(typeof(IGenericTransient<>));
			                          });

			IGenericTransient<int> transient = injectionKernel.Locate<IGenericTransient<int>>();

			Assert.NotNull(transient);
			IGenericService<int> service = injectionKernel.Locate<IGenericService<int>>();

			Assert.NotNull(service);
		}

		[Fact]
		public void SingletonGenericTest()
		{
			InjectionKernelManager injectionKernelManager =
				new InjectionKernelManager(null, DependencyInjectionContainer.CompareExportStrategies, new BlackList());
			InjectionKernel injectionKernel =
				new InjectionKernel(injectionKernelManager,
					null,
					null,
					"RootScope",
					DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c => c.Export(typeof(GenericService<>)).As(typeof(IGenericService<>)).AndSingleton());

			IGenericService<int> serviceA = injectionKernel.Locate<IGenericService<int>>();
			IGenericService<int> serviceB = injectionKernel.Locate<IGenericService<int>>();

			Assert.NotNull(serviceA);
			Assert.NotNull(serviceB);

			Assert.True(ReferenceEquals(serviceA, serviceB));
		}

		[Fact]
		public void ImportGenericList()
		{
			InjectionKernelManager injectionKernelManager =
				new InjectionKernelManager(null, DependencyInjectionContainer.CompareExportStrategies, new BlackList());
			InjectionKernel injectionKernel =
				new InjectionKernel(injectionKernelManager,
					null,
					null,
					"RootScope",
					DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c =>
			                          {
				                          c.Export(typeof(GenericService<>)).As(typeof(IGenericService<>));
				                          c.Export(typeof(ConstrainedService<>)).As(typeof(IGenericService<>));
			                          });

			IEnumerable<IGenericService<int>> services = injectionKernel.LocateAll<IGenericService<int>>();

			Assert.NotNull(services);
			Assert.Equal(1, services.Count());

			IEnumerable<IGenericService<IBasicService>> basicServices =
				injectionKernel.LocateAll<IGenericService<IBasicService>>();

			Assert.NotNull(basicServices);
			Assert.Equal(2, basicServices.Count());
		}

		[Fact]
		public void RecursiveGenericTest()
		{
			InjectionKernelManager injectionKernelManager =
				new InjectionKernelManager(null, DependencyInjectionContainer.CompareExportStrategies, new BlackList());
			InjectionKernel injectionKernel =
				new InjectionKernel(injectionKernelManager,
					null,
					null,
					"RootScope",
					DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c => c.Export(typeof(GenericEntityServuce<>)).As(typeof(IGenericEntityService<>)));

			IGenericEntityService<GenericEntity> service = 
				injectionKernel.Locate<IGenericEntityService<GenericEntity>>();

			Assert.NotNull(service);
		}
	}
}