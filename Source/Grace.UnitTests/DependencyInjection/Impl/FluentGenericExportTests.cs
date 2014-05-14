using System.Collections.Generic;
using System.Linq;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Conditions;
using Grace.DependencyInjection.Impl;
using Grace.DependencyInjection.Lifestyle;
using Grace.UnitTests.Classes.Simple;
using Xunit;

namespace Grace.UnitTests.DependencyInjection.Impl
{
	public class FluentGenericExportTests
	{
		[Fact]
		public void RegisterWithoutAs()
		{
			InjectionKernelManager injectionKernelManager =
				new InjectionKernelManager(null, DependencyInjectionContainer.CompareExportStrategies, new BlackList());
			InjectionKernel injectionKernel =
				new InjectionKernel(injectionKernelManager,
					null,
					null,
					"RootScope",
					DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c => c.Export<BasicService>());

			BasicService basicService = injectionKernel.Locate<BasicService>();

			Assert.NotNull(basicService);
		}

		[Fact]
		public void RegisterAsType()
		{
			InjectionKernelManager injectionKernelManager =
				new InjectionKernelManager(null, DependencyInjectionContainer.CompareExportStrategies, new BlackList());
			InjectionKernel injectionKernel =
				new InjectionKernel(injectionKernelManager,
					null,
					null,
					"RootScope",
					DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c => c.Export<BasicService>().As<IBasicService>());

			IBasicService basicService = injectionKernel.Locate<IBasicService>();

			Assert.NotNull(basicService);
		}

		[Fact]
		public void RegisterAsName()
		{
			InjectionKernelManager injectionKernelManager =
				new InjectionKernelManager(null, DependencyInjectionContainer.CompareExportStrategies, new BlackList());
			InjectionKernel injectionKernel =
				new InjectionKernel(injectionKernelManager,
					null,
					null,
					"RootScope",
					DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c => c.Export<BasicService>().As<IBasicService>());

			IBasicService basicService = injectionKernel.Locate<IBasicService>();

			Assert.NotNull(basicService);
		}

		[Fact]
		public void RegisterAsTypeAndName()
		{
			InjectionKernelManager injectionKernelManager =
				new InjectionKernelManager(null, DependencyInjectionContainer.CompareExportStrategies, new BlackList());
			InjectionKernel injectionKernel =
				new InjectionKernel(injectionKernelManager,
					null,
					null,
					"RootScope",
					DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c => c.Export<BasicService>().AsName("IBasicService").As<IBasicService>());

			IBasicService basicService = injectionKernel.Locate("IBasicService") as IBasicService;

			Assert.NotNull(basicService);

			basicService = injectionKernel.Locate<IBasicService>();

			Assert.NotNull(basicService);
		}

		[Fact]
		public void RegisterAsTypeAndNameAndSingleton()
		{
			InjectionKernelManager injectionKernelManager =
				new InjectionKernelManager(null, DependencyInjectionContainer.CompareExportStrategies, new BlackList());
			InjectionKernel injectionKernel =
				new InjectionKernel(injectionKernelManager,
					null,
					null,
					"RootScope",
					DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(
				c => c.Export<BasicService>().AsName("IBasicService").As<IBasicService>().AndSingleton());

			IBasicService basicService = injectionKernel.Locate("IBasicService") as IBasicService;

			Assert.NotNull(basicService);

			IBasicService basicService2 = injectionKernel.Locate<IBasicService>();

			Assert.NotNull(basicService2);

			Assert.True(ReferenceEquals(basicService, basicService2));
		}

		[Fact]
		public void RegisterConstructorImport()
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
				                          c.Export<BasicService>().As<IBasicService>();
				                          c.Export<ImportConstructorService>().As<IImportConstructorService>();
			                          });

			IImportConstructorService constructorService = injectionKernel.Locate<IImportConstructorService>();

			Assert.NotNull(constructorService);
		}

		[Fact]
		public void RegisterImportProperty()
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
				                          c.Export<BasicService>().As<IBasicService>();
				                          c.Export<ImportPropertyService>()
					                          .As<IImportPropertyService>()
					                          .ImportProperty(o => o.BasicService);
			                          });

			IImportPropertyService importPropertyService = injectionKernel.Locate<IImportPropertyService>();

			Assert.NotNull(importPropertyService);
			Assert.NotNull(importPropertyService.BasicService);
		}

		[Fact]
		public void RegisterImportPropertyWithFilter()
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
				                          c.ExportAssembly(GetType().Assembly).ByInterface(typeof(ISimpleObject));
				                          c.Export<ImportPropertySimpleObject>().
					                         ImportProperty(x => x.SimpleObject).Consider(ExportsThat.EndWith("C"));
			                          });

			ImportPropertySimpleObject import = injectionKernel.Locate<ImportPropertySimpleObject>();

			Assert.NotNull(import);
			//Assert.NotNull(import.SimpleObject);
			Assert.IsType(typeof(SimpleObjectC), import.SimpleObject);
		}

		[Fact]
		public void RegisterImportPropertyWithFuncVale()
		{
			InjectionKernelManager injectionKernelManager =
				new InjectionKernelManager(null, DependencyInjectionContainer.CompareExportStrategies, new BlackList());
			InjectionKernel injectionKernel =
				new InjectionKernel(injectionKernelManager,
					null,
					null,
					"RootScope",
					DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c => c.Export<ImportPropertyService>()
				.As<IImportPropertyService>()
				.ImportProperty(o => o.BasicService).UsingValue(() => new BasicService()));

			IImportPropertyService importPropertyService = injectionKernel.Locate<IImportPropertyService>();

			Assert.NotNull(importPropertyService);
			Assert.NotNull(importPropertyService.BasicService);
		}

		[Fact]
		public void RegisterImportMethodWithDefaults()
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
				                          c.Export<BasicService>().As<IBasicService>();
				                          c.Export<ImportMethodWithArgs>().
					                          ImportMethod(x => x.ImportMethod(null, null, 0)).
					                          WithMethodParam(() => "Hello").WithMethodParam(() => 7);
			                          });

			ImportMethodWithArgs import = injectionKernel.Locate<ImportMethodWithArgs>();

			Assert.NotNull(import);
			Assert.NotNull(import.BasicService);
			Assert.NotNull(import.StringParam);
			Assert.Equal("Hello", import.StringParam);
			Assert.Equal(7, import.IntParam);
		}

		[Fact]
		public void RegisterImportCollectionWithSort()
		{
			InjectionKernelManager injectionKernelManager =
				new InjectionKernelManager(null, DependencyInjectionContainer.CompareExportStrategies, new BlackList());
			InjectionKernel injectionKernel =
				new InjectionKernel(injectionKernelManager,
					null,
					null,
					"RootScope",
					DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c => c.Export<ImportPropertyCollection>().ImportCollectionProperty(p => p.SimpleObjects));
		}

		[Fact]
		public void RegisterImportMethodWithDefaultsDictionary()
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
				                          c.Export<BasicService>().As<IBasicService>();
				                          c.Export<ImportMethodWithArgs>()
					                          .ImportMethod(x => x.ImportMethod(null, null, 0))
					                          .WithMethodParam(() => "Hello").WithMethodParam(() => 7);
			                          });

			ImportMethodWithArgs import = injectionKernel.Locate<ImportMethodWithArgs>();

			Assert.NotNull(import);
			Assert.NotNull(import.BasicService);
			Assert.NotNull(import.StringParam);
			Assert.Equal("Hello", import.StringParam);
			Assert.Equal(7, import.IntParam);
		}

		[Fact]
		public void RegisterImportPropertyWithExportProvider()
		{
			InjectionKernelManager injectionKernelManager =
				new InjectionKernelManager(null, DependencyInjectionContainer.CompareExportStrategies, new BlackList());
			InjectionKernel injectionKernel =
				new InjectionKernel(injectionKernelManager,
					null,
					null,
					"RootScope",
					DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c => c.Export<ImportPropertyService>()
				.As<IImportPropertyService>()
				.ImportProperty(o => o.BasicService).UsingValueProvider(
					new FuncValueProvider<BasicService>(() => new BasicService())));

			IImportPropertyService importPropertyService = injectionKernel.Locate<IImportPropertyService>();

			Assert.NotNull(importPropertyService);
			Assert.NotNull(importPropertyService.BasicService);
		}

		[Fact]
		public void AutoWireAllProperties()
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
				                          c.Export<BasicService>().As<IBasicService>();
				                          c.Export<ImportConstructorService>().As<IImportConstructorService>();
				                          c.Export<ImportPropertyService>()
					                          .As<IImportPropertyService>()
					                          .ImportProperty(x => x.BasicService);
				                          c.Export<ImportMethodService>()
					                          .As<IImportMethodService>()
					                          .ImportMethod(o => o.ImportMethod(null));
				                          c.Export<MultiplePropertyImportService>()
					                          .As<IMultiplePropertyImportService>()
					                          .AutoWireProperties();
			                          });

			IMultiplePropertyImportService multiplePropertyImportService =
				injectionKernel.Locate<IMultiplePropertyImportService>();

			Assert.NotNull(multiplePropertyImportService);
			Assert.NotNull(multiplePropertyImportService.ConstructorService);
			Assert.NotNull(multiplePropertyImportService.MethodService);
			Assert.NotNull(multiplePropertyImportService.PropertyService);
		}

		[Fact]
		public void ActivateMethods()
		{
			InjectionKernelManager injectionKernelManager =
				new InjectionKernelManager(null, DependencyInjectionContainer.CompareExportStrategies, new BlackList());

			InjectionKernel injectionKernel =
				new InjectionKernel(injectionKernelManager,
					null,
					null,
					"RootScope",
					DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c => c.Export<ActivateService>()
				.As<IActiveService>()
				.ActivationMethod(o => o.SimpleActivate())
				.ActivationMethod(o => o.InjectionContextActivate(null)));

			IActiveService activeService = injectionKernel.Locate<IActiveService>();

			Assert.NotNull(activeService);
			Assert.True(activeService.SimpleActivateCalled);
			Assert.True(activeService.InjectionContextActivateCalled);
		}

		[Fact]
		public void RegisterImportMethod()
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
				                          c.Export<BasicService>().As<IBasicService>();
				                          c.Export<ImportMethodService>()
					                          .As<IImportMethodService>()
					                          .ImportMethod(o => o.ImportMethod(null));
			                          });

			IImportMethodService importMethodService = injectionKernel.Locate<IImportMethodService>();

			Assert.NotNull(importMethodService);
			Assert.NotNull(importMethodService.BasicService);
		}

		[Fact]
		public void RegisterMultipleImportType()
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
				                          c.Export<BasicService>().As<IBasicService>();
				                          c.Export<ImportConstructorService>().As<IImportConstructorService>();
				                          c.Export<ImportPropertyService>()
					                          .As<IImportPropertyService>()
					                          .ImportProperty(o => o.BasicService);
				                          c.Export<ImportAllTypes>()
					                          .ImportProperty(o => o.PropertyService)
					                          .ImportMethod(o => o.ImportMethod(null));
			                          });

			ImportAllTypes importAllTypes = injectionKernel.Locate<ImportAllTypes>();

			Assert.NotNull(importAllTypes);
			Assert.NotNull(importAllTypes.PropertyService);
			Assert.NotNull(importAllTypes.ImportConstructorService);
		}

		[Fact]
		public void RegisterArrayImport()
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
				                          c.Export<SimpleObjectA>().As<ISimpleObject>();
				                          c.Export<SimpleObjectB>().As<ISimpleObject>();
				                          c.Export<SimpleObjectC>().As<ISimpleObject>();
				                          c.Export<SimpleObjectD>().As<ISimpleObject>();
				                          c.Export<SimpleObjectE>().As<ISimpleObject>();
				                          c.Export<ImportArrayService>().As<IImportArrayService>();
			                          });

			IImportArrayService arrayService = injectionKernel.Locate<IImportArrayService>();

			Assert.NotNull(arrayService);
		}

		[Fact]
		public void RegisterInstancePerScope()
		{
			InjectionKernelManager injectionKernelManager =
				new InjectionKernelManager(null, DependencyInjectionContainer.CompareExportStrategies, new BlackList());
			InjectionKernel injectionKernel =
				new InjectionKernel(injectionKernelManager,
					null,
					null,
					"RootScope",
					DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c => c.Export<BasicService>().As<IBasicService>().AndSingletonPerScope());

			IBasicService basicService = injectionKernel.Locate<IBasicService>();

			Assert.NotNull(basicService);

			Assert.True(ReferenceEquals(basicService, injectionKernel.Locate<IBasicService>()));

			IInjectionScope childScope = injectionKernel.CreateChildScope();

			IBasicService secondService = childScope.Locate<IBasicService>();

			Assert.NotNull(secondService);
			Assert.False(ReferenceEquals(basicService, secondService));
			Assert.True(ReferenceEquals(secondService, childScope.Locate<IBasicService>()));
		}

		[Fact]
		public void RegisterWeakSingleton()
		{
			InjectionKernelManager injectionKernelManager =
				new InjectionKernelManager(null, DependencyInjectionContainer.CompareExportStrategies, new BlackList());
			InjectionKernel injectionKernel =
				new InjectionKernel(injectionKernelManager,
					null,
					null,
					"RootScope",
					DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c => c.Export<BasicService>().As<IBasicService>().AndWeakSingleton());

			IBasicService basicService = injectionKernel.Locate<IBasicService>();

			Assert.NotNull(basicService);

			Assert.True(ReferenceEquals(basicService, injectionKernel.Locate<IBasicService>()));

			IInjectionScope childScope = injectionKernel.CreateChildScope();

			IBasicService secondService = childScope.Locate<IBasicService>();

			Assert.NotNull(secondService);
			Assert.True(ReferenceEquals(basicService, secondService));
			Assert.True(ReferenceEquals(secondService, childScope.Locate<IBasicService>()));
		}

		[Fact]
		public void EnrichWithDelegate()
		{
			InjectionKernelManager injectionKernelManager =
				new InjectionKernelManager(null, DependencyInjectionContainer.CompareExportStrategies, new BlackList());
			InjectionKernel injectionKernel =
				new InjectionKernel(injectionKernelManager,
					null,
					null,
					"RootScope",
					DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c => c.Export<BasicService>().As<IBasicService>().
				EnrichWith(
					(scope, context, enrichObject) =>
						new BasicServiceWrapper(enrichObject as IBasicService)));

			IBasicService basicService = injectionKernel.Locate<IBasicService>();

			Assert.NotNull(basicService);
			Assert.IsType(typeof(BasicServiceWrapper), basicService);

			BasicServiceWrapper wrapper = basicService as BasicServiceWrapper;

			Assert.NotNull(wrapper.BasicService);
			Assert.IsType(typeof(BasicService), wrapper.BasicService);
		}

		[Fact]
		public void PriorityTest()
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
				                          c.Export<BasicService>().As<IBasicService>();
				                          c.Export<FauxBasicService>().As<IBasicService>().WithPriority(1);
			                          });

			IBasicService basicService = injectionKernel.Locate<IBasicService>();

			Assert.NotNull(basicService);
			Assert.IsType(typeof(FauxBasicService), basicService);
		}

		[Fact]
		public void UsingLifestyleContainerTest()
		{
			InjectionKernelManager injectionKernelManager =
				new InjectionKernelManager(null, DependencyInjectionContainer.CompareExportStrategies, new BlackList());
			InjectionKernel injectionKernel =
				new InjectionKernel(injectionKernelManager,
					null,
					null,
					"RootScope",
					DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(
				c => c.Export<BasicService>().As<IBasicService>().UsingLifestyleContainer(new SingletonLifestyle()));

			IBasicService basicService = injectionKernel.Locate<IBasicService>();

			Assert.NotNull(basicService);

			Assert.True(ReferenceEquals(basicService, injectionKernel.Locate<IBasicService>()));
		}

		[Fact]
		public void AndConditionTest()
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
				                          c.Export<ImportPropertyService>()
					                          .As<IImportPropertyService>()
					                          .ImportProperty(o => o.BasicService);

				                          c.Export<ImportMethodService>()
					                          .As<IImportMethodService>()
					                          .ImportMethod(o => o.ImportMethod(null));

				                          c.Export<BasicService>()
					                          .As<IBasicService>()
					                          .AndCondition(
						                          new WhenCondition(
							                          (x, y, z) => y.TargetInfo.InjectionType == typeof(ImportPropertyService)));

				                          c.Export<FauxBasicService>()
					                          .As<IBasicService>()
					                          .AndCondition(
						                          new WhenCondition(
							                          (x, y, z) => y.TargetInfo.InjectionType == typeof(ImportMethodService)));
			                          });

			IImportPropertyService propertyService = injectionKernel.Locate<IImportPropertyService>();

			Assert.NotNull(propertyService);
			Assert.NotNull(propertyService.BasicService);
			Assert.IsType(typeof(BasicService), propertyService.BasicService);

			IImportMethodService methodService = injectionKernel.Locate<IImportMethodService>();

			Assert.NotNull(methodService);
			Assert.NotNull(methodService.BasicService);
			Assert.IsType(typeof(FauxBasicService), methodService.BasicService);
		}

		[Fact]
		public void WhenConditionTest()
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
				                          c.Export<ImportPropertyService>()
					                          .As<IImportPropertyService>()
					                          .ImportProperty(o => o.BasicService);

				                          c.Export<ImportMethodService>()
					                          .As<IImportMethodService>()
					                          .ImportMethod(o => o.ImportMethod(null));

				                          c.Export<BasicService>()
					                          .As<IBasicService>()
					                          .When((x, y, z) => y.TargetInfo.InjectionType == typeof(ImportPropertyService));

				                          c.Export<FauxBasicService>()
					                          .As<IBasicService>()
					                          .When((x, y, z) => y.TargetInfo.InjectionType == typeof(ImportMethodService));
			                          });

			IImportPropertyService propertyService = injectionKernel.Locate<IImportPropertyService>();

			Assert.NotNull(propertyService);
			Assert.NotNull(propertyService.BasicService);
			Assert.IsType(typeof(BasicService), propertyService.BasicService);

			IImportMethodService methodService = injectionKernel.Locate<IImportMethodService>();

			Assert.NotNull(methodService);
			Assert.NotNull(methodService.BasicService);
			Assert.IsType(typeof(FauxBasicService), methodService.BasicService);
		}

		[Fact]
		public void UnlessConditionTest()
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
				                          c.Export<ImportPropertyService>()
					                          .As<IImportPropertyService>()
					                          .ImportProperty(o => o.BasicService);

				                          c.Export<ImportMethodService>()
					                          .As<IImportMethodService>()
					                          .ImportMethod(o => o.ImportMethod(null));

				                          c.Export<BasicService>()
					                          .As<IBasicService>()
					                          .Unless((x, y, z) => y.TargetInfo.InjectionType == typeof(ImportPropertyService));

				                          c.Export<FauxBasicService>()
					                          .As<IBasicService>()
					                          .Unless((x, y, z) => y.TargetInfo.InjectionType == typeof(ImportMethodService));
			                          });

			IImportPropertyService propertyService = injectionKernel.Locate<IImportPropertyService>();

			Assert.NotNull(propertyService);
			Assert.NotNull(propertyService.BasicService);
			Assert.IsType(typeof(FauxBasicService), propertyService.BasicService);

			IImportMethodService methodService = injectionKernel.Locate<IImportMethodService>();

			Assert.NotNull(methodService);
			Assert.NotNull(methodService.BasicService);
			Assert.IsType(typeof(BasicService), methodService.BasicService);
		}

		[Fact]
		public void WhenClassHas()
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
				                          c.Export<SimpleObjectA>().As<ISimpleObject>();
				                          c.Export<SimpleObjectA>().As<ISimpleObject>();
				                          c.Export<SimpleObjectA>().As<ISimpleObject>();
				                          c.Export<SimpleObjectA>().As<ISimpleObject>().WhenClassHas<SomeTestAttribute>();
				                          c.Export<SimpleObjectA>().As<ISimpleObject>().WhenClassHas<SomeTestAttribute>();
				                          c.Export<AttributedClassMultipleImport>();
				                          c.Export<ImportIEnumerableService>();
			                          });

			ImportIEnumerableService iEnumerableService = injectionKernel.Locate<ImportIEnumerableService>();

			Assert.NotNull(iEnumerableService);
			Assert.Equal(3, iEnumerableService.Count);

			AttributedClassMultipleImport multipleImport = injectionKernel.Locate<AttributedClassMultipleImport>();

			Assert.NotNull(multipleImport);
			Assert.Equal(5, multipleImport.SimpleObjects.Count());
		}

		[Fact]
		public void WhenMemberHas()
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
				                          c.Export<SimpleObjectA>().As<ISimpleObject>();
				                          c.Export<SimpleObjectA>().As<ISimpleObject>();
				                          c.Export<SimpleObjectA>().As<ISimpleObject>();
				                          c.Export<SimpleObjectA>().As<ISimpleObject>().WhenMemberHas<SomeTestAttribute>();
				                          c.Export<SimpleObjectA>().As<ISimpleObject>().WhenMemberHas<SomeTestAttribute>();
				                          c.Export<AttributedMethodMultipleImport>().ImportMethod(x => x.ImportMethod(null));
				                          c.Export<AttributedPropertyMultipleImport>().ImportProperty(x => x.SimpleObjects);
				                          c.Export<ImportIEnumerableService>();
			                          });

			ImportIEnumerableService iEnumerableService = injectionKernel.Locate<ImportIEnumerableService>();

			Assert.NotNull(iEnumerableService);
			Assert.Equal(3, iEnumerableService.Count);

			AttributedMethodMultipleImport multipleImport = injectionKernel.Locate<AttributedMethodMultipleImport>();

			Assert.NotNull(multipleImport);
			Assert.Equal(5, multipleImport.Count);

			AttributedPropertyMultipleImport propertyMultipleImport = injectionKernel.Locate<AttributedPropertyMultipleImport>();

			Assert.NotNull(propertyMultipleImport);
			Assert.Equal(5, propertyMultipleImport.SimpleObjects.Length);
		}

		[Fact]
		public void LocateWithKey()
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
				                          c.Export<SimpleObjectA>().AsName("ISimpleObject").WithKey(1);
				                          c.Export<SimpleObjectB>().AsName("ISimpleObject").WithKey(2);
				                          c.Export<SimpleObjectC>().AsName("ISimpleObject").WithKey(3);
				                          c.Export<SimpleObjectD>().AsName("ISimpleObject").WithKey(4);
				                          c.Export<SimpleObjectE>().AsName("ISimpleObject").WithKey(5);
			                          });

			Assert.IsType(typeof(SimpleObjectA), injectionKernel.LocateByKey("ISimpleObject", 1));
			Assert.IsType(typeof(SimpleObjectB), injectionKernel.LocateByKey("ISimpleObject", 2));
			Assert.IsType(typeof(SimpleObjectC), injectionKernel.LocateByKey("ISimpleObject", 3));
			Assert.IsType(typeof(SimpleObjectD), injectionKernel.LocateByKey("ISimpleObject", 4));
			Assert.IsType(typeof(SimpleObjectE), injectionKernel.LocateByKey("ISimpleObject", 5));
		}

		[Fact]
		public void LocateAllInEnvironment()
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
				                          c.Export<SimpleObjectA>()
					                          .AsName("ISimpleObject")
					                          .InEnvironment(ExportEnvironment.RunTime);
				                          c.Export<SimpleObjectB>()
					                          .AsName("ISimpleObject")
					                          .InEnvironment(ExportEnvironment.UnitTest);
				                          c.Export<SimpleObjectC>()
					                          .AsName("ISimpleObject")
					                          .InEnvironment(ExportEnvironment.DesignTime);
			                          });

			IEnumerable<object> all = injectionKernel.LocateAll("ISimpleObject");

			Assert.NotNull(all);
			Assert.Equal(3, all.Count());
		}

		[Fact]
		public void Metadata()
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
				                          c.Export<SimpleObjectA>()
					                          .AsName("ISimpleObject")
					                          .WithMetadata("Test", 0)
					                          .WithMetadata("Blah", 9);
				                          c.Export<SimpleObjectB>()
					                          .AsName("ISimpleObject")
					                          .WithMetadata("Test", 1)
					                          .WithMetadata("Blah", 8);
				                          c.Export<SimpleObjectC>()
					                          .AsName("ISimpleObject")
					                          .WithMetadata("Test", 2)
					                          .WithMetadata("Blah", 7);
				                          c.Export<SimpleObjectD>()
					                          .AsName("ISimpleObject")
					                          .WithMetadata("Test", 3)
					                          .WithMetadata("Blah", 6);
				                          c.Export<SimpleObjectE>()
					                          .AsName("ISimpleObject")
					                          .WithMetadata("Test", 4)
					                          .WithMetadata("Blah", 5);
			                          });

			IEnumerable<object> simpleObjects =
				injectionKernel.LocateAll("ISimpleObject",
					consider: (context, strategy) =>
						(int)strategy.Metadata["Test"] == 0 || (int)strategy.Metadata["Test"] == 2);

			Assert.Equal(2, simpleObjects.Count());
		}

		[Fact]
		public void WithCtorParamValueTest()
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
				                          c.Export<BasicService>().As<IBasicService>();
				                          c.Export<WithCtorParamClass>().
					                          WithCtorParam(() => "Hello").
					                          WithCtorParam(() => 5);
			                          });

			WithCtorParamClass paramClass = injectionKernel.Locate<WithCtorParamClass>();

			Assert.NotNull(paramClass);
			Assert.NotNull(paramClass.BasicService);
			Assert.NotNull(paramClass.StringParam);
			Assert.Equal("Hello", paramClass.StringParam);
			Assert.Equal(5, paramClass.IntParam);
		}

		[Fact]
		public void WithCtorParamValueWithNameTest()
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
				                          c.Export<BasicService>().As<IBasicService>();
				                          c.Export<WithCtorParamClass>()
					                          .WithCtorParam(() => "Hello").Named("stringParam")
					                          .WithCtorParam(() => 5).Named("intParam");
			                          });

			WithCtorParamClass paramClass = injectionKernel.Locate<WithCtorParamClass>();

			Assert.NotNull(paramClass);
			Assert.NotNull(paramClass.BasicService);
			Assert.NotNull(paramClass.StringParam);
			Assert.Equal("Hello", paramClass.StringParam);
			Assert.Equal(5, paramClass.IntParam);
		}

		[Fact]
		public void WithCtorParamFiltered()
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
				                          c.ExportAssembly(GetType().Assembly).ByInterface(typeof(ISimpleObject));
				                          c.Export<ImportIEnumerableService>()
					                          .WithCtorParam<IEnumerable<ISimpleObject>>()
					                          .Consider((context, strategy) => strategy.ActivationName.EndsWith("C"));
			                          });

			ImportIEnumerableService import = injectionKernel.Locate<ImportIEnumerableService>();

			Assert.NotNull(import);
			Assert.Equal(1, import.Count);
		}

		[Fact]
		public void WithCtorParamFuncTest()
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
				                          c.Export<BasicService>().As<IBasicService>();
				                          c.Export<WithCtorParamClass>().WithCtorParam(() => "Hello").WithCtorParam(() => 5);
			                          });

			WithCtorParamClass paramClass = injectionKernel.Locate<WithCtorParamClass>();

			Assert.NotNull(paramClass);
			Assert.NotNull(paramClass.BasicService);
			Assert.NotNull(paramClass.StringParam);
			Assert.Equal("Hello", paramClass.StringParam);
			Assert.Equal(5, paramClass.IntParam);
		}

		[Fact]
		public void WithCtorParamFuncWithNameTest()
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
				                          c.Export<BasicService>().As<IBasicService>();
				                          c.Export<WithCtorParamClass>()
					                          .WithCtorParam(() => "Hello").Named("stringParam")
					                          .WithCtorParam(() => 5).Named("intParam");
			                          });

			WithCtorParamClass paramClass = injectionKernel.Locate<WithCtorParamClass>();

			Assert.NotNull(paramClass);
			Assert.NotNull(paramClass.BasicService);
			Assert.NotNull(paramClass.StringParam);
			Assert.Equal("Hello", paramClass.StringParam);
			Assert.Equal(5, paramClass.IntParam);
		}

		[Fact]
		public void WithCtorParamCollection()
		{
			InjectionKernelManager injectionKernelManager =
				new InjectionKernelManager(null, DependencyInjectionContainer.CompareExportStrategies, new BlackList());
			InjectionKernel injectionKernel =
				new InjectionKernel(injectionKernelManager,
					null,
					null,
					"RootScope",
					DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c => c.Export(Types.FromThisAssembly()).ByInterface(typeof(ISimpleObject)));
			injectionKernel.Configure(c =>
				c.Export<ImportListService>()
					.WithCtorCollectionParam<List<ISimpleObject>, ISimpleObject>().
					SortByProperty(p => p.TestString));

			ImportListService importListService = injectionKernel.Locate<ImportListService>();

			Assert.NotNull(importListService);
			Assert.Equal(5, importListService.SimpleObjects.Count);
			Assert.Equal("A", importListService.SimpleObjects[0].TestString);
			Assert.Equal("B", importListService.SimpleObjects[1].TestString);
			Assert.Equal("C", importListService.SimpleObjects[2].TestString);
			Assert.Equal("D", importListService.SimpleObjects[3].TestString);
			Assert.Equal("E", importListService.SimpleObjects[4].TestString);
		}

		[Fact]
		public void ImportConstructor()
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
				                          c.Export<BasicService>().As<IBasicService>();
				                          c.Export<MultipleConstructorImport>()
					                          .ImportConstructor(() => new MultipleConstructorImport(null));
				                          c.Export<ConstructorImportService>().As<IConstructorImportService>();
			                          });

			MultipleConstructorImport import = injectionKernel.Locate<MultipleConstructorImport>();

			Assert.NotNull(import);
			Assert.NotNull(import.BasicService);
			Assert.Null(import.ConstructorImportService);
		}

		[Fact]
		public void ExportPropertyTest()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(
				c => c.Export<ExportPropertyService>().As<IExportPropertyService>().ExportProperty(p => p.BasicService));

			IBasicService basicService = container.Locate<IBasicService>();

			Assert.NotNull(basicService);
		}

		[Fact]
		public void ExportPropertySingletonTest()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c => c.Export<ExportPropertyService>().
				As<IExportPropertyService>().
				ExportProperty(p => p.BasicService).
				AndSingleton());

			IBasicService basicService = container.Locate<IBasicService>();
			IBasicService secondService = container.Locate<IBasicService>();

			Assert.NotNull(basicService);
			Assert.NotNull(secondService);

			Assert.True(ReferenceEquals(basicService, secondService));
		}

		[Fact]
		public void CleanupDelegateTest()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			bool cleanupFired = false;
			bool disposed = false;

			container.Configure(
				c => c.Export<DisposableService>().As<IDisposableService>().DisposalCleanupDelegate(x => cleanupFired = true));

			IDisposableService disposableService = container.Locate<IDisposableService>();

			disposableService.Disposing += (sender, args) => disposed = true;

			container.Dispose();

			Assert.True(cleanupFired);
			Assert.True(disposed);
		}
	}
}