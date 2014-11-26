using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Grace.Data.Immutable;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Exceptions;
using Grace.DependencyInjection.Impl;
using Grace.UnitTests.Classes.FauxClasses;
using Grace.UnitTests.Classes.Simple;
using Xunit;

namespace Grace.UnitTests.DependencyInjection.Impl
{
	public class InjectionKernelTests
	{
		private const string TEST_VALUE_STRING = "TestValue";
		private const string NEW_VALUE = "NewValue";

		#region Extra data test

		[Fact]
		public void ExtraDataTest()
		{
			InjectionKernelManager kernelManager =
				new InjectionKernelManager(null, DependencyInjectionContainer.CompareExportStrategies, new BlackList());

			InjectionKernel injectionKernel = new InjectionKernel(kernelManager,
				null,
				null,
				string.Empty,
				DependencyInjectionContainer.CompareExportStrategies);

			Assert.Null(injectionKernel.GetExtraData(TEST_VALUE_STRING));

			injectionKernel.SetExtraData(TEST_VALUE_STRING, NEW_VALUE);

			Assert.Equal(NEW_VALUE, injectionKernel.GetExtraData(TEST_VALUE_STRING));
		}

		#endregion

		#region Clone Tests

		[Fact]
		public void RootCloneExceptionTest()
		{
			InjectionKernelManager kernelManager =
				new InjectionKernelManager(null, DependencyInjectionContainer.CompareExportStrategies, new BlackList());

			InjectionKernel injectionKernel = new InjectionKernel(kernelManager,
				null,
				null,
				string.Empty,
				DependencyInjectionContainer.CompareExportStrategies);

			try
			{
				injectionKernel.Clone(null, null, null);

				throw new Exception("Should have thrown an exception when trying to clone root scope");
			}
			catch (RootScopeCloneException exp)
			{
			}
		}

		[Fact]
		public void CloneTest()
		{
			InjectionKernelManager kernelManager =
				new InjectionKernelManager(null, DependencyInjectionContainer.CompareExportStrategies, new BlackList());

			InjectionKernel injectionKernel = new InjectionKernel(kernelManager,
				new FauxInjectionScope(),
				null,
				string.Empty,
				DependencyInjectionContainer.CompareExportStrategies);

			FauxExportStrategy strategy = new FauxExportStrategy(() => new BasicService())
			                              {
				                              ExportTypes = new[] { typeof(IBasicService) }
			                              };

			injectionKernel.AddStrategy(strategy);

			IInjectionScope clone =
				injectionKernel.Clone(new FauxInjectionScope { ScopeName = "FauxParent" }, null, null);

			IExportStrategy exportStrategy =
				clone.GetStrategy(typeof(IBasicService), injectionContext: injectionKernel.CreateContext());

			Assert.True(ReferenceEquals(exportStrategy, strategy));
		}

		#endregion

		#region Add Remove Exports Tests

		[Fact]
		public void AddExportTest()
		{
			InjectionKernelManager kernelManager =
				new InjectionKernelManager(null, DependencyInjectionContainer.CompareExportStrategies, new BlackList());

			InjectionKernel injectionKernel = new InjectionKernel(kernelManager,
				null,
				null,
				string.Empty,
				DependencyInjectionContainer.CompareExportStrategies);

			FauxExportStrategy strategy = new FauxExportStrategy(() => new BasicService())
			                              {
				                              ExportTypes = new[] { typeof(IBasicService) }
			                              };

			injectionKernel.AddStrategy(strategy);

			IExportStrategy exportStrategy =
				injectionKernel.GetStrategy(typeof(IBasicService), injectionContext: injectionKernel.CreateContext());

			Assert.True(ReferenceEquals(exportStrategy, strategy));
		}

		[Fact]
		public void RemoveExportTest()
		{
			InjectionKernelManager kernelManager =
				new InjectionKernelManager(null, DependencyInjectionContainer.CompareExportStrategies, new BlackList());

			InjectionKernel injectionKernel = new InjectionKernel(kernelManager,
				null,
				null,
				string.Empty,
				DependencyInjectionContainer.CompareExportStrategies);

			FauxExportStrategy strategy = new FauxExportStrategy(() => new BasicService())
			                              {
				                              ExportTypes = new[] { typeof(IBasicService) }
			                              };

			injectionKernel.AddStrategy(strategy);

			IExportStrategy exportStrategy =
				injectionKernel.GetStrategy(typeof(IBasicService), injectionContext: injectionKernel.CreateContext());

			Assert.True(ReferenceEquals(exportStrategy, strategy));

			injectionKernel.RemoveStrategy(strategy);

			exportStrategy =
				injectionKernel.GetStrategy(typeof(IBasicService), injectionContext: injectionKernel.CreateContext());

			Assert.Null(exportStrategy);
		}

		#endregion

		#region Child Scope Tests

		[Fact]
		public void ChildScopeTest()
		{
			InjectionKernelManager kernelManager =
				new InjectionKernelManager(null, DependencyInjectionContainer.CompareExportStrategies, new BlackList());

			InjectionKernel injectionKernel = new InjectionKernel(kernelManager,
				null,
				null,
				string.Empty,
				DependencyInjectionContainer.CompareExportStrategies);

			FauxExportStrategy strategy = new FauxExportStrategy(() => new BasicService())
			                              {
				                              ExportTypes = new[] { typeof(IBasicService) }
			                              };

			injectionKernel.AddStrategy(strategy);

			IInjectionScope child = injectionKernel.CreateChildScope();

			IBasicService basicService = child.Locate<IBasicService>();

			Assert.NotNull(basicService);
			Assert.IsType(typeof(BasicService), basicService);
		}

		#endregion

		#region Basic Tests

		
		[Fact]
		public void BasicInstanceExportTest()
		{
			InjectionKernelManager manager = new InjectionKernelManager(null,
				DependencyInjectionContainer.CompareExportStrategies,
				new BlackList());
			InjectionKernel injectionKernel = new InjectionKernel(manager,
				null,
				null,
				null,
				DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(
				ioc =>
				{
					ioc.Export<BasicService>().As<IBasicService>();
					ioc.Export<ConstructorImportService>().As<IConstructorImportService>();
				});

			IConstructorImportService importPropertyService = injectionKernel.Locate<IConstructorImportService>();

			Assert.NotNull(importPropertyService);
			Assert.NotNull(importPropertyService.BasicService);
		}

		#endregion

		#region Dispose Tests

		[Fact]
		public void DisposeInjectionKernel()
		{
			InjectionKernelManager manager = new InjectionKernelManager(null,
				DependencyInjectionContainer.CompareExportStrategies,
				new BlackList());
			InjectionKernel injectionKernel = new InjectionKernel(manager,
				null,
				null,
				null,
				DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(
				ioc => ioc.Export<DisposableService>().As<IDisposableService>());

			IDisposableService disposableService = injectionKernel.Locate<IDisposableService>();
			bool eventFired = false;

			disposableService.Disposing += (sender, args) => eventFired = true;

			injectionKernel.Dispose();

			Assert.True(eventFired);

			eventFired = false;

			injectionKernel.Dispose();

			Assert.False(eventFired);
		}

		[Fact]
		public void DisposeInjectionKernelWithParent()
		{
			InjectionKernelManager manager = new InjectionKernelManager(null,
				DependencyInjectionContainer.CompareExportStrategies,
				new BlackList());
			InjectionKernel injectionKernel = new InjectionKernel(manager,
				null,
				null,
				null,
				DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(
				ioc => ioc.Export<DisposableService>().As<IDisposableService>());

			IInjectionScope newInjectionScope = injectionKernel.CreateChildScope();

			IDisposableService disposableService = newInjectionScope.Locate<IDisposableService>();
			bool eventFired = false;

			disposableService.Disposing += (sender, args) => eventFired = true;

			newInjectionScope.Dispose();

			Assert.True(eventFired);
		}

		[Fact]
		public void DisposeInjectionKernelAndLifestyle()
		{
			InjectionKernelManager manager = new InjectionKernelManager(null,
				DependencyInjectionContainer.CompareExportStrategies,
				new BlackList());
			InjectionKernel injectionKernel = new InjectionKernel(manager,
				null,
				null,
				null,
				DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(
				ioc => ioc.Export<DisposableService>().As<IDisposableService>().Lifestyle.Singleton());

			IDisposableService disposableService = injectionKernel.Locate<IDisposableService>();
			bool eventFired = false;

			disposableService.Disposing += (sender, args) => eventFired = true;

			injectionKernel.Dispose();

			Assert.True(eventFired);
		}

		[Fact]
		public void DisposalScopeProvider()
		{
			DisposalScope disposalScope = new DisposalScope();

			InjectionKernelManager manager = new InjectionKernelManager(null,
				DependencyInjectionContainer.CompareExportStrategies,
				new BlackList());

			InjectionKernel injectionKernel =
				new InjectionKernel(manager,
					null,
					new FauxDisposalScopeProvider(() => disposalScope),
					"RootScope",
					DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c => c.Export<DisposableService>().As<IDisposableService>());

			IDisposableService disposableService = injectionKernel.Locate<IDisposableService>();

			bool disposed = false;

			disposableService.Disposing += (sender, args) => disposed = true;

			disposalScope.Dispose();

			Assert.True(disposed);
		}

		#endregion

		#region Locate Tests

		[Fact]
		public void LocateByNameInParent()
		{
			InjectionKernelManager manager = new InjectionKernelManager(null,
				DependencyInjectionContainer.CompareExportStrategies,
				new BlackList());
			InjectionKernel injectionKernel = new InjectionKernel(manager,
				null,
				null,
				null,
				DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c => c.Export(typeof(BasicService)).AsName("BasicService"));

			IInjectionScope childScope = injectionKernel.CreateChildScope();

			BasicService basicService = (BasicService)childScope.Locate("BasicService");

			Assert.NotNull(basicService);
		}

		[Fact]
		public void LocateByNameWithContextInParent()
		{
			InjectionKernelManager manager = new InjectionKernelManager(null,
				DependencyInjectionContainer.CompareExportStrategies,
				new BlackList());
			InjectionKernel injectionKernel = new InjectionKernel(manager,
				null,
				null,
				"RootScope",
				DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c => c.Export(typeof(BasicService)).AsName("BasicService"));

			IInjectionScope childScope = injectionKernel.CreateChildScope();

			BasicService basicService = (BasicService)childScope.Locate("BasicService", injectionContext: childScope.CreateContext());

			Assert.NotNull(basicService);
		}

		[Fact]
		public void LocateAllTest()
		{
			InjectionKernelManager manager = new InjectionKernelManager(null,
				DependencyInjectionContainer.CompareExportStrategies,
				new BlackList());
			InjectionKernel injectionKernel = new InjectionKernel(manager,
				null,
				null,
				"RootScope",
				DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c =>
			                          {
				                          c.Export<SimpleObjectA>().AsName("ISimpleObject");
				                          c.Export<SimpleObjectB>().AsName("ISimpleObject");
				                          c.Export<SimpleObjectC>().AsName("ISimpleObject");
				                          c.Export<SimpleObjectD>().AsName("ISimpleObject");
				                          c.Export<SimpleObjectE>().AsName("ISimpleObject");
			                          });

			IEnumerable<object> simpleObjects = injectionKernel.LocateAll("ISimpleObject");

			Assert.NotNull(simpleObjects);
			Assert.Equal(5, simpleObjects.Count());
		}

		[Fact]
		public void LocateAllWithFilterTest()
		{
			InjectionKernelManager manager = new InjectionKernelManager(null,
				DependencyInjectionContainer.CompareExportStrategies,
				new BlackList());
			InjectionKernel injectionKernel = new InjectionKernel(manager,
				null,
				null,
				"RootScope",
				DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c =>
			                          {
				                          c.Export<SimpleObjectA>().AsName("ISimpleObject");
				                          c.Export<SimpleObjectB>().AsName("ISimpleObject");
				                          c.Export<SimpleObjectC>().AsName("ISimpleObject");
				                          c.Export<SimpleObjectD>().AsName("ISimpleObject");
				                          c.Export<SimpleObjectE>().AsName("ISimpleObject");
			                          });

			IEnumerable<object> simpleObjects =
				injectionKernel.LocateAll("ISimpleObject",
					consider: (context, strategy) =>
						strategy.ActivationName.EndsWith("A") || strategy.ActivationName.EndsWith("C"));

			Assert.NotNull(simpleObjects);
			Assert.Equal(2, simpleObjects.Count());
		}

		#endregion

		#region Locate Generic Tests

		[Fact]
		public void LocateAllGenericTest()
		{
			InjectionKernelManager manager = new InjectionKernelManager(null,
				DependencyInjectionContainer.CompareExportStrategies,
				new BlackList());
			InjectionKernel injectionKernel = new InjectionKernel(manager,
				null,
				null,
				"RootScope",
				DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c =>
			                          {
				                          c.Export<SimpleObjectA>().As<ISimpleObject>();
				                          c.Export<SimpleObjectB>().As<ISimpleObject>();
				                          c.Export<SimpleObjectC>().As<ISimpleObject>();
				                          c.Export<SimpleObjectD>().As<ISimpleObject>();
				                          c.Export<SimpleObjectE>().As<ISimpleObject>();
			                          });

			IEnumerable<ISimpleObject> simpleObjects = injectionKernel.LocateAll<ISimpleObject>();

			Assert.NotNull(simpleObjects);
			Assert.Equal(5, simpleObjects.Count());
		}

		[Fact]
		public void LocateAllGenericFromParentTest()
		{
			InjectionKernelManager manager = new InjectionKernelManager(null,
				DependencyInjectionContainer.CompareExportStrategies,
				new BlackList());
			InjectionKernel injectionKernel = new InjectionKernel(manager,
				null,
				null,
				"RootScope",
				DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c =>
			                          {
				                          c.Export<SimpleObjectA>().As<ISimpleObject>();
				                          c.Export<SimpleObjectB>().As<ISimpleObject>();
				                          c.Export<SimpleObjectC>().As<ISimpleObject>();
				                          c.Export<SimpleObjectD>().As<ISimpleObject>();
				                          c.Export<SimpleObjectE>().As<ISimpleObject>();
			                          });

			IInjectionScope childScope = injectionKernel.CreateChildScope();

			IEnumerable<ISimpleObject> simpleObjects = childScope.LocateAll<ISimpleObject>();

			Assert.NotNull(simpleObjects);
			Assert.Equal(5, simpleObjects.Count());
		}

		[Fact]
		public void LocateWithKeyTest()
		{
			InjectionKernelManager manager = new InjectionKernelManager(null,
				DependencyInjectionContainer.CompareExportStrategies,
				new BlackList());
			InjectionKernel injectionKernel = new InjectionKernel(manager,
				null,
				null,
				"RootScope",
				DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c =>
			                          {
				                          c.Export<SimpleObjectA>().As<ISimpleObject>().WithKey(5);
				                          c.Export<SimpleObjectB>().As<ISimpleObject>().WithKey(10);
				                          c.Export<SimpleObjectC>().As<ISimpleObject>().WithKey(20);
			                          });

            Assert.IsType(typeof(SimpleObjectA), injectionKernel.Locate<ISimpleObject>(locateKey: 5));
            Assert.IsType(typeof(SimpleObjectB), injectionKernel.Locate<ISimpleObject>(locateKey: 10));
            Assert.IsType(typeof(SimpleObjectC), injectionKernel.Locate<ISimpleObject>(locateKey: 20));
		}

		[Fact]
		public void LocateObservableCollection()
		{
			InjectionKernelManager manager = new InjectionKernelManager(null,
				DependencyInjectionContainer.CompareExportStrategies,
				new BlackList());
			InjectionKernel injectionKernel = new InjectionKernel(manager,
				null,
				null,
				"RootScope",
				DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c =>
			                          {
				                          c.Export<SimpleObjectA>().As<ISimpleObject>();
				                          c.Export<SimpleObjectB>().As<ISimpleObject>();
				                          c.Export<SimpleObjectC>().As<ISimpleObject>();
				                          c.Export<SimpleObjectD>().As<ISimpleObject>();
				                          c.Export<SimpleObjectE>().As<ISimpleObject>();
				                          c.Export<ImportReadOnlyCollectionService>();
			                          });

			ObservableCollection<ISimpleObject> observableCollection =
				injectionKernel.Locate<ObservableCollection<ISimpleObject>>();

			Assert.NotNull(observableCollection);
			Assert.Equal(5, observableCollection.Count);
		}

		[Fact]
		public void LocateAllGenericFromParentCombinedTest()
		{
			InjectionKernelManager manager = new InjectionKernelManager(null,
				DependencyInjectionContainer.CompareExportStrategies,
				new BlackList());
			InjectionKernel injectionKernel = new InjectionKernel(manager,
				null,
				null,
				"RootScope",
				DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c =>
			                          {
				                          c.Export<SimpleObjectA>().As<ISimpleObject>();
				                          c.Export<SimpleObjectB>().As<ISimpleObject>();
			                          });

			IInjectionScope childScope = injectionKernel.CreateChildScope(
				c =>
				{
					c.Export<SimpleObjectC>().As<ISimpleObject>();
					c.Export<SimpleObjectD>().As<ISimpleObject>();
					c.Export<SimpleObjectE>().As<ISimpleObject>();
				});

			IEnumerable<ISimpleObject> simpleObjects = childScope.LocateAll<ISimpleObject>();

			Assert.NotNull(simpleObjects);
			Assert.Equal(5, simpleObjects.Count());
		}

		[Fact]
		public void LocateAllGenericWithFilterTest()
		{
			InjectionKernelManager manager = new InjectionKernelManager(null,
				DependencyInjectionContainer.CompareExportStrategies,
				new BlackList());
			InjectionKernel injectionKernel = new InjectionKernel(manager,
				null,
				null,
				"RootScope",
				DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c =>
			                          {
				                          c.Export<SimpleObjectA>().As<ISimpleObject>();
				                          c.Export<SimpleObjectB>().As<ISimpleObject>();
				                          c.Export<SimpleObjectC>().As<ISimpleObject>();
				                          c.Export<SimpleObjectD>().As<ISimpleObject>();
				                          c.Export<SimpleObjectE>().As<ISimpleObject>();
			                          });

			IEnumerable<object> simpleObjects =
				injectionKernel.LocateAll<ISimpleObject>(consider: (context, strategy) => strategy.ActivationName.EndsWith("A") ||
				                                                                          strategy.ActivationName.EndsWith("C"));

			Assert.NotNull(simpleObjects);
			Assert.Equal(2, simpleObjects.Count());
		}

		[Fact]
		public void ConstrainedTest()
		{
			InjectionKernelManager manager = new InjectionKernelManager(null,
				DependencyInjectionContainer.CompareExportStrategies,
				new BlackList());
			InjectionKernel injectionKernel = new InjectionKernel(manager,
				null,
				null,
				"RootScope",
				DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c =>
			                          {
				                          c.Export<BasicService>().As<IBasicService>();
				                          c.Export(typeof(GenericService<>)).As(typeof(IGenericService<>));
				                          c.Export(typeof(ConstrainedService<>)).As(typeof(IGenericService<>)).WithPriority(10);
			                          });

			IGenericService<int> genericService = injectionKernel.Locate<IGenericService<int>>();

			Assert.NotNull(genericService);
			Assert.IsType(typeof(GenericService<int>), genericService);

			IGenericService<IBasicService> basicGenericService = injectionKernel.Locate<IGenericService<IBasicService>>();

			Assert.NotNull(basicGenericService);
			Assert.IsType(typeof(ConstrainedService<IBasicService>), basicGenericService);
		}

		[Fact]
		public void MultipleImportConstructor()
		{
			InjectionKernelManager manager = new InjectionKernelManager(null,
				DependencyInjectionContainer.CompareExportStrategies,
				new BlackList());
			InjectionKernel injectionKernel = new InjectionKernel(manager,
				null,
				null,
				"RootScope",
				DependencyInjectionContainer.CompareExportStrategies);
			injectionKernel.Configure(c =>
			                          {
				                          c.Export<BasicService>().As<IBasicService>();
				                          c.Export<ConstructorImportService>().As<IConstructorImportService>();
				                          c.Export<MultipleConstructorImport>();
			                          });

			MultipleConstructorImport import = injectionKernel.Locate<MultipleConstructorImport>();

			Assert.NotNull(import);
			Assert.NotNull(import.BasicService);
			Assert.NotNull(import.ConstructorImportService);
		}

		[Fact]
		public void ImportIEnumerable()
		{
			InjectionKernelManager manager = new InjectionKernelManager(null,
				DependencyInjectionContainer.CompareExportStrategies,
				new BlackList());
			InjectionKernel injectionKernel = new InjectionKernel(manager,
				null,
				null,
				"RootScope",
				DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c =>
			                          {
				                          c.Export<SimpleObjectA>().As<ISimpleObject>();
				                          c.Export<SimpleObjectB>().As<ISimpleObject>();
				                          c.Export<SimpleObjectC>().As<ISimpleObject>();
				                          c.Export<SimpleObjectD>().As<ISimpleObject>();
				                          c.Export<SimpleObjectE>().As<ISimpleObject>();
				                          c.Export<ImportIEnumerableService>();
			                          });

			ImportIEnumerableService service = injectionKernel.Locate<ImportIEnumerableService>();

			Assert.NotNull(service);
			Assert.Equal(5, service.Count);
		}

		[Fact]
		public void ImportIReadOnlyCollection()
		{
			InjectionKernelManager manager = new InjectionKernelManager(null,
				DependencyInjectionContainer.CompareExportStrategies,
				new BlackList());
			InjectionKernel injectionKernel = new InjectionKernel(manager,
				null,
				null,
				"RootScope",
				DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c =>
			                          {
				                          c.Export<SimpleObjectA>().As<ISimpleObject>();
				                          c.Export<SimpleObjectB>().As<ISimpleObject>();
				                          c.Export<SimpleObjectC>().As<ISimpleObject>();
				                          c.Export<SimpleObjectD>().As<ISimpleObject>();
				                          c.Export<SimpleObjectE>().As<ISimpleObject>();
				                          c.Export<ImportReadOnlyCollectionService>();
			                          });

			ImportReadOnlyCollectionService service = injectionKernel.Locate<ImportReadOnlyCollectionService>();

			Assert.NotNull(service);
		}

		[Fact]
		public void ImportObservableCollection()
		{
			InjectionKernelManager manager = new InjectionKernelManager(null,
				DependencyInjectionContainer.CompareExportStrategies,
				new BlackList());
			InjectionKernel injectionKernel = new InjectionKernel(manager,
				null,
				null,
				"RootScope",
				DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c =>
			                          {
				                          c.Export<SimpleObjectA>().As<ISimpleObject>();
				                          c.Export<SimpleObjectB>().As<ISimpleObject>();
				                          c.Export<SimpleObjectC>().As<ISimpleObject>();
				                          c.Export<SimpleObjectD>().As<ISimpleObject>();
				                          c.Export<SimpleObjectE>().As<ISimpleObject>();
				                          c.Export<ImportObservableCollectionService>();
			                          });

			ImportObservableCollectionService service =
				injectionKernel.Locate<ImportObservableCollectionService>();

			Assert.NotNull(service);
			Assert.Equal(5, service.Count);
		}

		[Fact]
		public void ImportOwnedTest()
		{
			InjectionKernelManager manager = new InjectionKernelManager(null,
				DependencyInjectionContainer.CompareExportStrategies,
				new BlackList());
			InjectionKernel injectionKernel = new InjectionKernel(manager,
				null,
				null,
				"RootScope",
				DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c =>
			                          {
				                          c.Export<DisposableService>().As<IDisposableService>();
				                          c.Export<ImportOwnedService>();
			                          });
		}

		[Fact]
		public void ImportLazyTest()
		{
			InjectionKernelManager manager = new InjectionKernelManager(null,
				DependencyInjectionContainer.CompareExportStrategies,
				new BlackList());

			InjectionKernel injectionKernel = new InjectionKernel(manager,
				null,
				null,
				"RootScope",
				DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c =>
			                          {
				                          c.Export<BasicService>().As<IBasicService>();
				                          c.Export<LazyImportService>();
			                          });

			LazyImportService importService = injectionKernel.Locate<LazyImportService>();

			Assert.NotNull(importService);

			IBasicService basicService = importService.BasicService;

			Assert.NotNull(basicService);
		}

		[Fact]
		public void ImportDisposalScope()
		{
			InjectionKernelManager manager = new InjectionKernelManager(null,
				DependencyInjectionContainer.CompareExportStrategies,
				new BlackList());
			InjectionKernel injectionKernel = new InjectionKernel(manager,
				null,
				null,
				"RootScope",
				DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c => c.Export<DisposalScopeInjectionService>());

			DisposalScopeInjectionService service = injectionKernel.Locate<DisposalScopeInjectionService>();

			Assert.NotNull(service);
			Assert.NotNull(service.DisposalScope);
			Assert.True(ReferenceEquals(service.DisposalScope, injectionKernel));
		}

		[Fact]
		public void ImportFunc()
		{
			InjectionKernelManager manager = new InjectionKernelManager(null,
				DependencyInjectionContainer.CompareExportStrategies,
				new BlackList());
			InjectionKernel injectionKernel = new InjectionKernel(manager,
				null,
				null,
				"RootScope",
				DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c =>
			                          {
				                          c.Export<BasicService>().As<IBasicService>();
				                          c.Export<FuncImportService>().As<IFuncImportService>();
			                          });

			IFuncImportService funcImportService = injectionKernel.Locate<IFuncImportService>();

			Assert.NotNull(funcImportService);

			IBasicService basicService = funcImportService.GetBasicService();

			Assert.NotNull(basicService);
		}

		#endregion

		#region Get Strategies Tests

		[Fact]
		public void GetAllStrategiesByType()
		{
			InjectionKernelManager manager = new InjectionKernelManager(null,
				DependencyInjectionContainer.CompareExportStrategies,
				new BlackList());
			InjectionKernel injectionKernel = new InjectionKernel(manager,
				null,
				null,
				"RootScope",
				DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c =>
			                          {
				                          c.Export<SimpleObjectA>().As<ISimpleObject>();
				                          c.Export<SimpleObjectB>().As<ISimpleObject>();
				                          c.Export<SimpleObjectC>().As<ISimpleObject>();
				                          c.Export<SimpleObjectD>().As<ISimpleObject>();
				                          c.Export<SimpleObjectE>().As<ISimpleObject>();
			                          });

			IEnumerable<IExportStrategy> strategies =
				injectionKernel.GetStrategies(typeof(ISimpleObject), injectionKernel.CreateContext());

			Assert.NotNull(strategies);
			Assert.Equal(5, strategies.Count());
		}

		[Fact]
		public void GetAllStrategiesByName()
		{
			InjectionKernelManager manager = new InjectionKernelManager(null,
				DependencyInjectionContainer.CompareExportStrategies,
				new BlackList());
			InjectionKernel injectionKernel = new InjectionKernel(manager,
				null,
				null,
				"RootScope",
				DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c =>
			                          {
				                          c.Export<SimpleObjectA>().AsName("ISimpleObject");
				                          c.Export<SimpleObjectB>().AsName("ISimpleObject");
				                          c.Export<SimpleObjectC>().AsName("ISimpleObject");
				                          c.Export<SimpleObjectD>().AsName("ISimpleObject");
				                          c.Export<SimpleObjectE>().AsName("ISimpleObject");
			                          });

			IEnumerable<IExportStrategy> strategies =
				injectionKernel.GetStrategies("ISimpleObject", injectionKernel.CreateContext());

			Assert.NotNull(strategies);
			Assert.Equal(5, strategies.Count());
		}

		[Fact]
		public void GetAllStrategiesByTypeFiltered()
		{
			InjectionKernelManager manager = new InjectionKernelManager(null,
				DependencyInjectionContainer.CompareExportStrategies,
				new BlackList());
			InjectionKernel injectionKernel = new InjectionKernel(manager,
				null,
				null,
				"RootScope",
				DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c =>
			                          {
				                          c.Export<SimpleObjectA>().As<ISimpleObject>();
				                          c.Export<SimpleObjectB>().As<ISimpleObject>();
				                          c.Export<SimpleObjectC>().As<ISimpleObject>();
				                          c.Export<SimpleObjectD>().As<ISimpleObject>();
				                          c.Export<SimpleObjectE>().As<ISimpleObject>();
			                          });

			IEnumerable<IExportStrategy> strategies =
				injectionKernel.GetStrategies(typeof(ISimpleObject),
					injectionKernel.CreateContext(),
					(context, strategy) =>
						strategy.ActivationName.EndsWith("A") || strategy.ActivationName.EndsWith("C"));

			Assert.NotNull(strategies);
			Assert.Equal(2, strategies.Count());
		}

		[Fact]
		public void GetAllStrategiesByNameFiltered()
		{
			InjectionKernelManager manager = new InjectionKernelManager(null,
				DependencyInjectionContainer.CompareExportStrategies,
				new BlackList());
			InjectionKernel injectionKernel = new InjectionKernel(manager,
				null,
				null,
				"RootScope",
				DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c =>
			                          {
				                          c.Export<SimpleObjectA>().AsName("ISimpleObject");
				                          c.Export<SimpleObjectB>().AsName("ISimpleObject");
				                          c.Export<SimpleObjectC>().AsName("ISimpleObject");
				                          c.Export<SimpleObjectD>().AsName("ISimpleObject");
				                          c.Export<SimpleObjectE>().AsName("ISimpleObject");
			                          });

			IEnumerable<IExportStrategy> strategies =
				injectionKernel.GetStrategies("ISimpleObject",
					injectionKernel.CreateContext(),
					(context, strategy) =>
						strategy.ActivationName.EndsWith("A") || strategy.ActivationName.EndsWith("C"));

			Assert.NotNull(strategies);
			Assert.Equal(2, strategies.Count());
		}

		#endregion

		#region Misc Config Tests

		[Fact]
		public void ConfigurationModuleTest()
		{
			InjectionKernelManager manager = new InjectionKernelManager(null,
				DependencyInjectionContainer.CompareExportStrategies,
				new BlackList());
			InjectionKernel injectionKernel = new InjectionKernel(manager,
				null,
				null,
				null,
				DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(new SimpleModule());

			IBasicService basicService = injectionKernel.Locate<IBasicService>();

			Assert.NotNull(basicService);
		}

		[Fact]
		public void AddExportStrategy()
		{
			InjectionKernelManager manager = new InjectionKernelManager(null,
				DependencyInjectionContainer.CompareExportStrategies,
				new BlackList());
			InjectionKernel injectionKernel = new InjectionKernel(manager,
				null,
				null,
				null,
				DependencyInjectionContainer.CompareExportStrategies);

			FauxExportStrategy basicExport = new FauxExportStrategy(() => new BasicService());

			basicExport.ExportNames = ImmutableArray<string>.Empty;
			basicExport.ExportTypes = new[] { typeof(IBasicService) };

			injectionKernel.Configure(c => c.AddExportStrategy(basicExport));

			IBasicService basicService =
				injectionKernel.Locate<IBasicService>();

			Assert.NotNull(basicService);
		}

		#endregion

		#region Special Types

		[Fact]
		public void OwnedResolve()
		{
			InjectionKernelManager manager = new InjectionKernelManager(null,
				DependencyInjectionContainer.CompareExportStrategies,
				new BlackList());
			InjectionKernel injectionKernel = new InjectionKernel(manager,
				null,
				null,
				"RootScope",
				DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c => c.Export<DisposableService>().As<IDisposableService>());

			Owned<IDisposableService> owned = injectionKernel.Locate<Owned<IDisposableService>>();

			Assert.NotNull(owned);
			Assert.NotNull(owned.Value);

			bool eventCalled = false;

			owned.Value.Disposing += (sender, args) => eventCalled = true;

			owned.Dispose();

			Assert.True(eventCalled);
		}

		[Fact]
		public void FuncResolve()
		{
			InjectionKernelManager manager = new InjectionKernelManager(null,
				DependencyInjectionContainer.CompareExportStrategies,
				new BlackList());
			InjectionKernel injectionKernel = new InjectionKernel(manager,
				null,
				null,
				"RootScope",
				DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c => c.Export<BasicService>().As<IBasicService>());

			Func<IBasicService> basicServiceFunc = injectionKernel.Locate<Func<IBasicService>>();

			Assert.NotNull(basicServiceFunc);

			IBasicService basicService = basicServiceFunc();

			Assert.NotNull(basicService);
		}

		[Fact]
		public void GenericFuncImport()
		{
			InjectionKernelManager manager = new InjectionKernelManager(null,
				DependencyInjectionContainer.CompareExportStrategies,
				new BlackList());
			InjectionKernel injectionKernel = new InjectionKernel(manager,
				null,
				null,
				"RootScope",
				DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c => c.Export<MultiplePropertyImportService>().AutoWireProperties(true));

			Func<IImportConstructorService, IImportMethodService, IImportPropertyService, MultiplePropertyImportService> func =
				injectionKernel
					.Locate
					<Func<IImportConstructorService, IImportMethodService, IImportPropertyService, MultiplePropertyImportService>>();

			MultiplePropertyImportService importService =
				func(new ImportConstructorService(new BasicService()), new ImportMethodService(), new ImportPropertyService());

			Assert.NotNull(importService);
			Assert.NotNull(importService.ConstructorService);
			Assert.NotNull(importService.MethodService);
			Assert.NotNull(importService.PropertyService);
		}

		[Fact]
		public void OwnedFuncResolve()
		{
			InjectionKernelManager manager = new InjectionKernelManager(null,
				DependencyInjectionContainer.CompareExportStrategies,
				new BlackList());
			InjectionKernel injectionKernel = new InjectionKernel(manager,
				null,
				null,
				"RootScope",
				DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c => c.Export<DisposableService>().As<IDisposableService>());

			Owned<Func<IDisposableService>> ownedFunc = injectionKernel.Locate<Owned<Func<IDisposableService>>>();

			Assert.NotNull(ownedFunc);
			Assert.NotNull(ownedFunc.Value);

			int count = 0;

			IDisposableService service1 = ownedFunc.Value();

			service1.Disposing += (sender, args) => count++;

			IDisposableService service2 = ownedFunc.Value();

			service2.Disposing += (sender, args) => count++;

			IDisposableService service3 = ownedFunc.Value();

			service3.Disposing += (sender, args) => count++;

			ownedFunc.Dispose();

			Assert.Equal(3, count);
		}

		[Fact]
		public void FuncOwnedResolve()
		{
			InjectionKernelManager manager = new InjectionKernelManager(null,
				DependencyInjectionContainer.CompareExportStrategies,
				new BlackList());
			InjectionKernel injectionKernel = new InjectionKernel(manager,
				null,
				null,
				"RootScope",
				DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c => c.Export<DisposableService>().As<IDisposableService>());

			Func<Owned<IDisposableService>> ownedFunc = injectionKernel.Locate<Func<Owned<IDisposableService>>>();

			Assert.NotNull(ownedFunc);

			Owned<IDisposableService> owned = ownedFunc();

			Assert.NotNull(owned);
			Assert.NotNull(owned.Value);

			bool eventCalled = false;

			owned.Value.Disposing += (sender, args) => eventCalled = true;

			owned.Dispose();

			Assert.True(eventCalled);

			owned = ownedFunc();

			Assert.NotNull(owned);
			Assert.NotNull(owned.Value);

			eventCalled = false;

			owned.Value.Disposing += (sender, args) => eventCalled = true;

			owned.Dispose();

			Assert.True(eventCalled);
		}

		[Fact]
		public void LazyResolve()
		{
			InjectionKernelManager manager = new InjectionKernelManager(null,
				DependencyInjectionContainer.CompareExportStrategies,
				new BlackList());
			InjectionKernel injectionKernel = new InjectionKernel(manager,
				null,
				null,
				"RootScope",
				DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c => c.Export<BasicService>().As<IBasicService>());

			Lazy<IBasicService> lazyBasic = injectionKernel.Locate<Lazy<IBasicService>>();

			Assert.NotNull(lazyBasic);
			Assert.False(lazyBasic.IsValueCreated);
			Assert.NotNull(lazyBasic.Value);
		}

		[Fact]
		public void ReadOnlyCollectionResolve()
		{
			InjectionKernelManager manager = new InjectionKernelManager(null,
				DependencyInjectionContainer.CompareExportStrategies,
				new BlackList());
			InjectionKernel injectionKernel = new InjectionKernel(manager,
				null,
				null,
				"RootScope",
				DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c =>
			                          {
				                          c.Export<SimpleObjectA>().As<ISimpleObject>();
				                          c.Export<SimpleObjectB>().As<ISimpleObject>();
				                          c.Export<SimpleObjectC>().As<ISimpleObject>();
				                          c.Export<SimpleObjectD>().As<ISimpleObject>();
				                          c.Export<SimpleObjectE>().As<ISimpleObject>();
			                          });

			ReadOnlyCollection<ISimpleObject> readOnlyCollection =
				injectionKernel.Locate<ReadOnlyCollection<ISimpleObject>>();

			Assert.NotNull(readOnlyCollection);
			Assert.Equal(5, readOnlyCollection.Count);
		}

		[Fact]
		public void IReadOnlyCollectionResolve()
		{
			InjectionKernelManager manager = new InjectionKernelManager(null,
				DependencyInjectionContainer.CompareExportStrategies,
				new BlackList());
			InjectionKernel injectionKernel = new InjectionKernel(manager,
				null,
				null,
				"RootScope",
				DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c =>
			                          {
				                          c.Export<SimpleObjectA>().As<ISimpleObject>();
				                          c.Export<SimpleObjectB>().As<ISimpleObject>();
				                          c.Export<SimpleObjectC>().As<ISimpleObject>();
				                          c.Export<SimpleObjectD>().As<ISimpleObject>();
				                          c.Export<SimpleObjectE>().As<ISimpleObject>();
			                          });

			IReadOnlyCollection<ISimpleObject> readOnlyCollection =
				injectionKernel.Locate<IReadOnlyCollection<ISimpleObject>>();

			Assert.NotNull(readOnlyCollection);
			Assert.Equal(5, readOnlyCollection.Count);
		}

		[Fact]
		public void IReadOnlyListResolve()
		{
			InjectionKernelManager manager = new InjectionKernelManager(null,
				DependencyInjectionContainer.CompareExportStrategies,
				new BlackList());
			InjectionKernel injectionKernel = new InjectionKernel(manager,
				null,
				null,
				"RootScope",
				DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c =>
			                          {
				                          c.Export<SimpleObjectA>().As<ISimpleObject>();
				                          c.Export<SimpleObjectB>().As<ISimpleObject>();
				                          c.Export<SimpleObjectC>().As<ISimpleObject>();
				                          c.Export<SimpleObjectD>().As<ISimpleObject>();
				                          c.Export<SimpleObjectE>().As<ISimpleObject>();
			                          });

			IReadOnlyList<ISimpleObject> readOnlyCollection =
				injectionKernel.Locate<IReadOnlyList<ISimpleObject>>();

			Assert.NotNull(readOnlyCollection);
			Assert.Equal(5, readOnlyCollection.Count);
		}

		[Fact]
		public void ArrayResolve()
		{
			InjectionKernelManager manager = new InjectionKernelManager(null,
				DependencyInjectionContainer.CompareExportStrategies,
				new BlackList());
			InjectionKernel injectionKernel = new InjectionKernel(manager,
				null,
				null,
				"RootScope",
				DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c =>
			                          {
				                          c.Export<SimpleObjectA>().As<ISimpleObject>();
				                          c.Export<SimpleObjectB>().As<ISimpleObject>();
				                          c.Export<SimpleObjectC>().As<ISimpleObject>();
				                          c.Export<SimpleObjectD>().As<ISimpleObject>();
				                          c.Export<SimpleObjectE>().As<ISimpleObject>();
			                          });

			ISimpleObject[] simpleObjects = injectionKernel.Locate<ISimpleObject[]>();

			Assert.NotNull(simpleObjects);
			Assert.Equal(5, simpleObjects.Length);
		}

		[Fact]
		public void DisposalScopeImportTest()
		{
			InjectionKernelManager manager = new InjectionKernelManager(null,
				DependencyInjectionContainer.CompareExportStrategies,
				new BlackList());
			InjectionKernel injectionKernel = new InjectionKernel(manager,
				null,
				null,
				"RootScope",
				DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c => c.Export<DisposalScopeInjectionService>());

			DisposalScopeInjectionService disposableService = injectionKernel.Locate<DisposalScopeInjectionService>();

			Assert.NotNull(disposableService);
			Assert.NotNull(disposableService.DisposalScope);
			Assert.Same(injectionKernel, disposableService.DisposalScope);
		}

		[Fact]
		public void InjectionContextImportTest()
		{
			InjectionKernelManager manager = new InjectionKernelManager(null,
				DependencyInjectionContainer.CompareExportStrategies,
				new BlackList());
			InjectionKernel injectionKernel = new InjectionKernel(manager,
				null,
				null,
				"RootScope",
				DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c => c.Export<InjectionScopeImportService>());

			InjectionScopeImportService importService = injectionKernel.Locate<InjectionScopeImportService>();

			Assert.NotNull(importService);
			Assert.NotNull(importService.InjectionScope);
			Assert.Same(injectionKernel, importService.InjectionScope);
		}

		[Fact]
		public void TransientInjectionContextImportTest()
		{
			InjectionKernelManager manager = new InjectionKernelManager(null,
				DependencyInjectionContainer.CompareExportStrategies,
				new BlackList());
			InjectionKernel injectionKernel = new InjectionKernel(manager,
				null,
				null,
				"RootScope",
				DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c => c.Export<InjectionScopeImportService>());

			IInjectionScope childScope = injectionKernel.CreateChildScope();

			InjectionScopeImportService importService = childScope.Locate<InjectionScopeImportService>();

			Assert.NotNull(importService);
			Assert.NotNull(importService.InjectionScope);
			Assert.Same(childScope, importService.InjectionScope);
		}

		[Fact]
		public void NonTransientInjectionContextImportTest()
		{
			InjectionKernelManager manager = new InjectionKernelManager(null,
				DependencyInjectionContainer.CompareExportStrategies,
				new BlackList());
			InjectionKernel injectionKernel = new InjectionKernel(manager,
				null,
				null,
				"RootScope",
				DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c => c.Export<InjectionScopeImportService>().Lifestyle.Singleton());

			IInjectionScope childScope = injectionKernel.CreateChildScope();

			InjectionScopeImportService importService = childScope.Locate<InjectionScopeImportService>();

			Assert.NotNull(importService);
			Assert.NotNull(importService.InjectionScope);
			Assert.Same(injectionKernel, importService.InjectionScope);
		}

		[Fact]
		public void ImportFuncType()
		{
			InjectionKernelManager manager = new InjectionKernelManager(null,
				DependencyInjectionContainer.CompareExportStrategies,
				new BlackList());
			InjectionKernel injectionKernel = new InjectionKernel(manager,
				null,
				null,
				"RootScope",
				DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c =>
			                          {
				                          c.Export<BasicService>().As<IBasicService>();
				                          c.Export<FuncTypeImportService>();
			                          });

			FuncTypeImportService importService = injectionKernel.Locate<FuncTypeImportService>();

			Assert.NotNull(importService);
			Assert.IsType(typeof(BasicService), importService.Create(typeof(IBasicService)));
		}

		[Fact]
		public void CreateFuncType()
		{
			InjectionKernelManager manager = new InjectionKernelManager(null,
				DependencyInjectionContainer.CompareExportStrategies,
				new BlackList());
			InjectionKernel injectionKernel = new InjectionKernel(manager,
				null,
				null,
				"RootScope",
				DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c => { c.Export<BasicService>().As<IBasicService>(); });

			Func<Type, object> importService = injectionKernel.Locate<Func<Type, object>>();

			Assert.NotNull(importService);
			Assert.IsType(typeof(BasicService), importService(typeof(IBasicService)));
		}

		#endregion

		#region Register Assembly

		[Fact]
		public void AssemblyExportInterfaceTest()
		{
			InjectionKernelManager manager = new InjectionKernelManager(null,
				DependencyInjectionContainer.CompareExportStrategies,
				new BlackList());
			InjectionKernel injectionKernel = new InjectionKernel(manager,
				null,
				null,
				"RootScope",
				DependencyInjectionContainer.CompareExportStrategies);

			Assembly assembly = GetType().GetTypeInfo().Assembly;

			injectionKernel.Configure(c => c.ExportAssembly(assembly).ByInterface(typeof(ISimpleObject)));

			IEnumerable<ISimpleObject> simpleObjects =
				injectionKernel.LocateAll<ISimpleObject>();

			Assert.NotNull(simpleObjects);
			Assert.Equal(5, simpleObjects.Count());
		}

		#endregion
	}
}