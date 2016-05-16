using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Conditions;
using Grace.DependencyInjection.Impl;
using Grace.DependencyInjection.Lifestyle;
using Grace.UnitTests.Classes.Simple;
using Xunit;

namespace Grace.UnitTests.DependencyInjection.Impl
{
	public class FluentExportTests
	{
		[Fact]
		public void RegisterWithoutAs()
		{
			InjectionKernelManager injectionKernelManager =
				new InjectionKernelManager(null, DependencyInjectionContainer.CompareExportStrategies);
			InjectionKernel injectionKernel =
				new InjectionKernel(injectionKernelManager,
					null,
					null,
					"RootScope",
					DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c => c.Export(typeof(BasicService)));

			BasicService basicService = injectionKernel.Locate<BasicService>();

			Assert.NotNull(basicService);
		}

		[Fact]
		public void RegisterAsType()
		{
			InjectionKernelManager injectionKernelManager =
				new InjectionKernelManager(null, DependencyInjectionContainer.CompareExportStrategies);
			InjectionKernel injectionKernel =
				new InjectionKernel(injectionKernelManager,
					null,
					null,
					"RootScope",
					DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c => c.Export(typeof(BasicService)).As(typeof(IBasicService)));

			IBasicService basicService = injectionKernel.Locate<IBasicService>();

			Assert.NotNull(basicService);
		}

		[Fact]
		public void RegisterAsName()
		{
			InjectionKernelManager injectionKernelManager =
				new InjectionKernelManager(null, DependencyInjectionContainer.CompareExportStrategies);
			InjectionKernel injectionKernel =
				new InjectionKernel(injectionKernelManager,
					null,
					null,
					"RootScope",
					DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c => c.Export(typeof(BasicService)).AsName("IBasicService"));

			IBasicService basicService = injectionKernel.Locate("IBasicService") as IBasicService;

			Assert.NotNull(basicService);
		}

		[Fact]
		public void RegisterAsTypeAndName()
		{
			InjectionKernelManager injectionKernelManager =
				new InjectionKernelManager(null, DependencyInjectionContainer.CompareExportStrategies);
			InjectionKernel injectionKernel =
				new InjectionKernel(injectionKernelManager,
					null,
					null,
					"RootScope",
					DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c => c.Export(typeof(BasicService)).AsName("IBasicService").As(typeof(IBasicService)));

			IBasicService basicService = injectionKernel.Locate("IBasicService") as IBasicService;

			Assert.NotNull(basicService);

			basicService = injectionKernel.Locate<IBasicService>();

			Assert.NotNull(basicService);
		}

		[Fact]
		public void RegisterAsTypeAndNameLifestyleSingleton()
		{
			InjectionKernelManager injectionKernelManager =
				new InjectionKernelManager(null, DependencyInjectionContainer.CompareExportStrategies);
			InjectionKernel injectionKernel =
				new InjectionKernel(injectionKernelManager,
					null,
					null,
					"RootScope",
					DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(
				c => c.Export(typeof(BasicService)).AsName("IBasicService").As(typeof(IBasicService)).Lifestyle.Singleton());

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
				new InjectionKernelManager(null, DependencyInjectionContainer.CompareExportStrategies);
			InjectionKernel injectionKernel =
				new InjectionKernel(injectionKernelManager,
					null,
					null,
					"RootScope",
					DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c =>
			                          {
				                          c.Export(typeof(BasicService)).As(typeof(IBasicService));
				                          c.Export(typeof(ImportConstructorService)).As(typeof(IImportConstructorService));
			                          });

			IImportConstructorService constructorService = injectionKernel.Locate<IImportConstructorService>();

			Assert.NotNull(constructorService);
		}

		[Fact]
		public void RegisterImportProperty()
		{
			InjectionKernelManager injectionKernelManager =
				new InjectionKernelManager(null, DependencyInjectionContainer.CompareExportStrategies);
			InjectionKernel injectionKernel =
				new InjectionKernel(injectionKernelManager,
					null,
					null,
					"RootScope",
					DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c =>
			                          {
				                          c.Export(typeof(BasicService)).As(typeof(IBasicService));
				                          c.Export(typeof(ImportPropertyService))
					                          .As(typeof(IImportPropertyService))
					                          .ImportProperty("BasicService");
			                          });

			IImportPropertyService importPropertyService = injectionKernel.Locate<IImportPropertyService>();

			Assert.NotNull(importPropertyService);
			Assert.NotNull(importPropertyService.BasicService);
		}

		[Fact]
		public void RegisterImportPropertyWithFuncVale()
		{
			InjectionKernelManager injectionKernelManager =
				new InjectionKernelManager(null, DependencyInjectionContainer.CompareExportStrategies);
			InjectionKernel injectionKernel =
				new InjectionKernel(injectionKernelManager,
					null,
					null,
					"RootScope",
					DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c => c.Export(typeof(ImportPropertyService))
				.As(typeof(IImportPropertyService))
				.ImportProperty("BasicService").UsingValue(() => new BasicService()));

			IImportPropertyService importPropertyService = injectionKernel.Locate<IImportPropertyService>();

			Assert.NotNull(importPropertyService);
			Assert.NotNull(importPropertyService.BasicService);
		}

		[Fact]
		public void RegisterImportPropertyWithExportProvider()
		{
			InjectionKernelManager injectionKernelManager =
				new InjectionKernelManager(null, DependencyInjectionContainer.CompareExportStrategies);
			InjectionKernel injectionKernel =
				new InjectionKernel(injectionKernelManager,
					null,
					null,
					"RootScope",
					DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c => c.Export(typeof(ImportPropertyService))
				.As(typeof(IImportPropertyService))
				.ImportProperty("BasicService").UsingValueProvider(
					new FuncValueProvider<BasicService>(
						() => new BasicService())));

			IImportPropertyService importPropertyService = injectionKernel.Locate<IImportPropertyService>();

			Assert.NotNull(importPropertyService);
			Assert.NotNull(importPropertyService.BasicService);
		}

		[Fact]
		public void AutoWireAllProperties()
		{
			InjectionKernelManager injectionKernelManager =
				new InjectionKernelManager(null, DependencyInjectionContainer.CompareExportStrategies);
			InjectionKernel injectionKernel =
				new InjectionKernel(injectionKernelManager,
					null,
					null,
					"RootScope",
					DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c =>
			                          {
				                          c.Export(typeof(BasicService)).As(typeof(IBasicService));
				                          c.Export(typeof(ImportConstructorService)).As(typeof(IImportConstructorService));
				                          c.Export(typeof(ImportPropertyService))
					                          .As(typeof(IImportPropertyService))
					                          .ImportProperty("BasicService");
				                          c.Export(typeof(ImportMethodService))
					                          .As(typeof(IImportMethodService))
					                          .ImportMethod("ImportMethod");
				                          c.Export(typeof(MultiplePropertyImportService))
					                          .As(typeof(IMultiplePropertyImportService))
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
				new InjectionKernelManager(null, DependencyInjectionContainer.CompareExportStrategies);

			InjectionKernel injectionKernel =
				new InjectionKernel(injectionKernelManager,
					null,
					null,
					"RootScope",
					DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c => c.Export(typeof(ActivateService))
				.As(typeof(IActiveService))
				.ActivationMethod("SimpleActivate")
				.ActivationMethod("InjectionContextActivate"));

			IActiveService activeService = injectionKernel.Locate<IActiveService>();

			Assert.NotNull(activeService);
			Assert.True(activeService.SimpleActivateCalled);
			Assert.True(activeService.InjectionContextActivateCalled);
		}

		[Fact]
		public void RegisterImportMethod()
		{
			InjectionKernelManager injectionKernelManager =
				new InjectionKernelManager(null, DependencyInjectionContainer.CompareExportStrategies);
			InjectionKernel injectionKernel =
				new InjectionKernel(injectionKernelManager,
					null,
					null,
					"RootScope",
					DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c =>
			                          {
				                          c.Export(typeof(BasicService)).As(typeof(IBasicService));
				                          c.Export(typeof(ImportMethodService))
					                          .As(typeof(IImportMethodService))
					                          .ImportMethod("ImportMethod");
			                          });

			IImportMethodService importMethodService = injectionKernel.Locate<IImportMethodService>();

			Assert.NotNull(importMethodService);
			Assert.NotNull(importMethodService.BasicService);
		}

		[Fact]
		public void RegisterMultipleImportType()
		{
			InjectionKernelManager injectionKernelManager =
				new InjectionKernelManager(null, DependencyInjectionContainer.CompareExportStrategies);
			InjectionKernel injectionKernel =
				new InjectionKernel(injectionKernelManager,
					null,
					null,
					"RootScope",
					DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c =>
			                          {
				                          c.Export(typeof(BasicService)).As(typeof(IBasicService));
				                          c.Export(typeof(ImportConstructorService)).As(typeof(IImportConstructorService));
				                          c.Export(typeof(ImportPropertyService))
					                          .As(typeof(IImportPropertyService))
					                          .ImportProperty("BasicService");
				                          c.Export(typeof(ImportAllTypes))
					                          .ImportProperty("PropertyService")
					                          .ImportMethod("ImportMethod");
			                          });

			ImportAllTypes importAllTypes = injectionKernel.Locate<ImportAllTypes>();

			Assert.NotNull(importAllTypes);
			Assert.NotNull(importAllTypes.PropertyService);
			Assert.NotNull(importAllTypes.ImportConstructorService);
		}

		[Fact]
		public void RegisterInstancePerScope()
		{
			InjectionKernelManager injectionKernelManager =
				new InjectionKernelManager(null, DependencyInjectionContainer.CompareExportStrategies);
			InjectionKernel injectionKernel =
				new InjectionKernel(injectionKernelManager,
					null,
					null,
					"RootScope",
					DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c => c.Export(typeof(BasicService)).As(typeof(IBasicService)).Lifestyle.SingletonPerScope());

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
				new InjectionKernelManager(null, DependencyInjectionContainer.CompareExportStrategies);
			InjectionKernel injectionKernel =
				new InjectionKernel(injectionKernelManager,
					null,
					null,
					"RootScope",
					DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c => c.Export(typeof(BasicService)).As(typeof(IBasicService)).Lifestyle.WeakSingleton());

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
				new InjectionKernelManager(null, DependencyInjectionContainer.CompareExportStrategies);
			InjectionKernel injectionKernel =
				new InjectionKernel(injectionKernelManager,
					null,
					null,
					"RootScope",
					DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c => c.Export(typeof(BasicService)).As(typeof(IBasicService)).
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
				new InjectionKernelManager(null, DependencyInjectionContainer.CompareExportStrategies);
			InjectionKernel injectionKernel =
				new InjectionKernel(injectionKernelManager,
					null,
					null,
					"RootScope",
					DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c =>
			                          {
				                          c.Export(typeof(BasicService)).As(typeof(IBasicService));
				                          c.Export(typeof(FauxBasicService)).As(typeof(IBasicService)).WithPriority(1);
			                          });

			IBasicService basicService = injectionKernel.Locate<IBasicService>();

			Assert.NotNull(basicService);
			Assert.IsType(typeof(FauxBasicService), basicService);
		}

		[Fact]
		public void UsingLifestyleContainerTest()
		{
			InjectionKernelManager injectionKernelManager =
				new InjectionKernelManager(null, DependencyInjectionContainer.CompareExportStrategies);
			InjectionKernel injectionKernel =
				new InjectionKernel(injectionKernelManager,
					null,
					null,
					"RootScope",
					DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(
				c => c.Export(typeof(BasicService)).As(typeof(IBasicService)).UsingLifestyle(new SingletonLifestyle()));

			IBasicService basicService = injectionKernel.Locate<IBasicService>();

			Assert.NotNull(basicService);

			Assert.True(ReferenceEquals(basicService, injectionKernel.Locate<IBasicService>()));
		}

		[Fact]
		public void AndConditionTest()
		{
			InjectionKernelManager injectionKernelManager =
				new InjectionKernelManager(null, DependencyInjectionContainer.CompareExportStrategies);
			InjectionKernel injectionKernel =
				new InjectionKernel(injectionKernelManager,
					null,
					null,
					"RootScope",
					DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c =>
			                          {
				                          c.Export(typeof(ImportPropertyService))
					                          .As(typeof(IImportPropertyService))
					                          .ImportProperty("BasicService");

				                          c.Export(typeof(ImportMethodService))
					                          .As(typeof(IImportMethodService))
					                          .ImportMethod("ImportMethod");

				                          c.Export(typeof(BasicService))
					                          .As(typeof(IBasicService))
					                          .AndCondition(
						                          new WhenCondition(
							                          (x, y, z) => y.TargetInfo.InjectionType == typeof(ImportPropertyService)));

				                          c.Export(typeof(FauxBasicService))
					                          .As(typeof(IBasicService))
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
				new InjectionKernelManager(null, DependencyInjectionContainer.CompareExportStrategies);
			InjectionKernel injectionKernel =
				new InjectionKernel(injectionKernelManager,
					null,
					null,
					"RootScope",
					DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c =>
			                          {
				                          c.Export(typeof(ImportPropertyService))
					                          .As(typeof(IImportPropertyService))
					                          .ImportProperty("BasicService");

				                          c.Export(typeof(ImportMethodService))
					                          .As(typeof(IImportMethodService))
					                          .ImportMethod("ImportMethod");

				                          c.Export(typeof(BasicService))
					                          .As(typeof(IBasicService))
					                          .When((x, y, z) => y.TargetInfo.InjectionType == typeof(ImportPropertyService));

				                          c.Export(typeof(FauxBasicService))
					                          .As(typeof(IBasicService))
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
				new InjectionKernelManager(null, DependencyInjectionContainer.CompareExportStrategies);
			InjectionKernel injectionKernel =
				new InjectionKernel(injectionKernelManager,
					null,
					null,
					"RootScope",
					DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c =>
			                          {
				                          c.Export(typeof(ImportPropertyService))
					                          .As(typeof(IImportPropertyService))
					                          .ImportProperty("BasicService");

				                          c.Export(typeof(ImportMethodService))
					                          .As(typeof(IImportMethodService))
					                          .ImportMethod("ImportMethod");

				                          c.Export(typeof(BasicService))
					                          .As(typeof(IBasicService))
					                          .Unless((x, y, z) => y.TargetInfo.InjectionType == typeof(ImportPropertyService));

				                          c.Export(typeof(FauxBasicService))
					                          .As(typeof(IBasicService))
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
		public void Metadata()
		{
			InjectionKernelManager injectionKernelManager =
				new InjectionKernelManager(null, DependencyInjectionContainer.CompareExportStrategies);
			InjectionKernel injectionKernel =
				new InjectionKernel(injectionKernelManager,
					null,
					null,
					"RootScope",
					DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c =>
			                          {
				                          c.Export(typeof(SimpleObjectA))
					                          .AsName("ISimpleObject")
					                          .WithMetadata("Test", 0)
					                          .WithMetadata("Blah", 9);
				                          c.Export(typeof(SimpleObjectB))
					                          .AsName("ISimpleObject")
					                          .WithMetadata("Test", 1)
					                          .WithMetadata("Blah", 8);
				                          c.Export(typeof(SimpleObjectC))
					                          .AsName("ISimpleObject")
					                          .WithMetadata("Test", 2)
					                          .WithMetadata("Blah", 7);
				                          c.Export(typeof(SimpleObjectD))
					                          .AsName("ISimpleObject")
					                          .WithMetadata("Test", 3)
					                          .WithMetadata("Blah", 6);
				                          c.Export(typeof(SimpleObjectE))
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
				new InjectionKernelManager(null, DependencyInjectionContainer.CompareExportStrategies);
			InjectionKernel injectionKernel =
				new InjectionKernel(injectionKernelManager,
					null,
					null,
					"RootScope",
					DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c =>
			                          {
				                          c.Export(typeof(BasicService)).As(typeof(IBasicService));
				                          c.Export(typeof(WithCtorParamClass)).WithCtorParam(() => "Hello").WithCtorParam(() => 5);
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
				new InjectionKernelManager(null, DependencyInjectionContainer.CompareExportStrategies);
			InjectionKernel injectionKernel =
				new InjectionKernel(injectionKernelManager,
					null,
					null,
					"RootScope",
					DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c =>
			                          {
				                          c.Export(typeof(BasicService)).As(typeof(IBasicService));
				                          c.Export(typeof(WithCtorParamClass))
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
		public void WithCtorParamFuncTest()
		{
			InjectionKernelManager injectionKernelManager =
				new InjectionKernelManager(null, DependencyInjectionContainer.CompareExportStrategies);
			InjectionKernel injectionKernel =
				new InjectionKernel(injectionKernelManager,
					null,
					null,
					"RootScope",
					DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c =>
			                          {
				                          c.Export(typeof(BasicService)).As(typeof(IBasicService));
				                          c.Export(typeof(WithCtorParamClass))
					                          .WithCtorParam(() => "Hello")
					                          .WithCtorParam(() => 5);
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
				new InjectionKernelManager(null, DependencyInjectionContainer.CompareExportStrategies);
			InjectionKernel injectionKernel =
				new InjectionKernel(injectionKernelManager,
					null,
					null,
					"RootScope",
					DependencyInjectionContainer.CompareExportStrategies);

			injectionKernel.Configure(c =>
			                          {
				                          c.Export(typeof(BasicService)).As(typeof(IBasicService));
				                          c.Export(typeof(WithCtorParamClass))
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
		public void ImportConstructor()
		{
			InjectionKernelManager injectionKernelManager =
				new InjectionKernelManager(null, DependencyInjectionContainer.CompareExportStrategies);
			InjectionKernel injectionKernel =
				new InjectionKernel(injectionKernelManager,
					null,
					null,
					"RootScope",
					DependencyInjectionContainer.CompareExportStrategies);

			ConstructorInfo constructor =
				typeof(MultipleConstructorImport).GetTypeInfo().DeclaredConstructors.First(x => x.GetParameters().Length == 1);

			injectionKernel.Configure(c =>
			                          {
				                          c.Export(typeof(BasicService)).As(typeof(IBasicService));
				                          c.Export(typeof(MultipleConstructorImport)).ImportConstructor(constructor);
				                          c.Export(typeof(ConstructorImportService)).As(typeof(IConstructorImportService));
			                          });

			MultipleConstructorImport import = injectionKernel.Locate<MultipleConstructorImport>();

			Assert.NotNull(import);
			Assert.NotNull(import.BasicService);
			Assert.Null(import.ConstructorImportService);
		}

		[Fact]
		public void CleanupDelegateTest()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			bool cleanupFired = false;
			bool disposed = false;

			container.Configure(
				c =>
					c.Export(typeof(DisposableService))
						.As(typeof(IDisposableService))
						.DisposalCleanupDelegate(x => cleanupFired = true));

			IDisposableService disposableService = container.Locate<IDisposableService>();

			disposableService.Disposing += (sender, args) => disposed = true;

			container.Dispose();

			Assert.True(cleanupFired);
			Assert.True(disposed);
		}
	}
}