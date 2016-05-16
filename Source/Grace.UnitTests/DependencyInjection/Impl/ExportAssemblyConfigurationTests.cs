using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl;
using Grace.UnitTests.Classes.Simple;
using Xunit;

namespace Grace.UnitTests.DependencyInjection.Impl
{
	public class ExportAssemblyConfigurationTests
	{
		[Fact]
		public void SingletonTest()
		{
			InjectionKernelManager injectionKernelManager =
				new InjectionKernelManager(null, DependencyInjectionContainer.CompareExportStrategies);
			InjectionKernel injectionKernel =
				new InjectionKernel(injectionKernelManager,
					null,
					null,
					"RootScope",
					DependencyInjectionContainer.CompareExportStrategies);

			Assembly assembly = GetType().Assembly;

			injectionKernel.Configure(c => c.ExportAssembly(assembly).ByInterface(typeof(ISimpleObject)).Lifestyle.Singleton());

			ISimpleObject simpleObject = injectionKernel.Locate<ISimpleObject>();

			Assert.NotNull(simpleObject);

			Assert.True(ReferenceEquals(simpleObject, injectionKernel.Locate<ISimpleObject>()));
		}

		[Fact]
		public void WeakSingletonTest()
		{
			InjectionKernelManager injectionKernelManager =
				new InjectionKernelManager(null, DependencyInjectionContainer.CompareExportStrategies);
			InjectionKernel injectionKernel =
				new InjectionKernel(injectionKernelManager,
					null,
					null,
					"RootScope",
					DependencyInjectionContainer.CompareExportStrategies);

			Assembly assembly = GetType().Assembly;

			injectionKernel.Configure(c => c.ExportAssembly(assembly).ByInterface(typeof(ISimpleObject)).Lifestyle.WeakSingleton());

			ISimpleObject simpleObject = injectionKernel.Locate<ISimpleObject>();

			Assert.NotNull(simpleObject);

			Assert.True(ReferenceEquals(simpleObject, injectionKernel.Locate<ISimpleObject>()));
		}
        
		[Fact]
		public void SelectTypes()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(
				c => c.ExportAssembly(GetType().Assembly).ByInterface(typeof(ISimpleObject)).Select(TypesThat.EndWith("C")));

			IEnumerable<ISimpleObject> simpleObjects = container.LocateAll<ISimpleObject>();

			Assert.NotNull(simpleObjects);
			Assert.Equal(1, simpleObjects.Count());
		}

		[Fact]
		public void ExportInterfaces()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(
				c => c.ExportAssembly(GetType().Assembly).ByInterfaces(t => t.Name.StartsWith("ISimple")));

			IEnumerable<ISimpleObject> simpleObjects = container.LocateAll<ISimpleObject>();

			Assert.NotNull(simpleObjects);
			Assert.Equal(5, simpleObjects.Count());
		}

		[Fact]
		public void ExportInterfacesAndWhere()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(
				c => c.Export(Types.FromThisAssembly()).
					ByInterfaces(TypesThat.StartWith("ISimple")).
					Select(TypesThat.EndWith("C")));

			IEnumerable<ISimpleObject> simpleObjects = container.LocateAll<ISimpleObject>();

			Assert.NotNull(simpleObjects);
			Assert.Equal(1, simpleObjects.Count());
		}
	}
}