using System.Linq;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl;
using Grace.UnitTests.Classes.FauxClasses;
using Grace.UnitTests.Classes.Simple;
using Xunit;

namespace Grace.UnitTests.DependencyInjection
{
	public class InjectionContextTests
	{
		private const string TEST_VALUE_STRING = "TestValue";
		private const string NEW_VALUE = "NewValue";

		[Fact]
		public void ExtraDataTest()
		{
			InjectionContext injectionContext = new InjectionContext(null, null);

			Assert.Null(injectionContext.GetExtraData(TEST_VALUE_STRING));

			injectionContext.SetExtraData(TEST_VALUE_STRING, NEW_VALUE);

			Assert.Equal(NEW_VALUE, injectionContext.GetExtraData(TEST_VALUE_STRING));
		}

		[Fact]
		public void RegisterTypeExportTest()
		{
			InjectionContext injectionContext = new InjectionContext(null, null);

			IBasicService testValue = injectionContext.Locate<IBasicService>();

			Assert.Null(testValue);

			IBasicService newValue = new BasicService();

			injectionContext.Export((x, y) => newValue);

			testValue = injectionContext.Locate<IBasicService>();

			Assert.NotNull(testValue);
			Assert.True(ReferenceEquals(newValue, testValue));
		}

		[Fact]
		public void LocateByName()
		{
			string exportName = "Test";
			InjectionContext injectionContext = new InjectionContext(null, null);

			Assert.Null(injectionContext.Locate(exportName));

			injectionContext.Export(exportName, (x, y) => new BasicService());

			// I'm getting by to lower because exporting by name is lower case
			object locatedObject = injectionContext.Locate(exportName.ToLowerInvariant());

			Assert.NotNull(locatedObject);
		}

		[Fact]
		public void LocateUnknownByName()
		{
			string exportName = "Test";
			InjectionContext injectionContext = new InjectionContext(null, null);

			injectionContext.Export(exportName, (x, y) => new BasicService());

			object testO = injectionContext.Locate("Test2");

			Assert.Null(testO);
		}

		[Fact]
		public void LocateUnknownType()
		{
			InjectionContext injectionContext = new InjectionContext(null, null);

			IBasicService testValue = injectionContext.Locate<IBasicService>();

			Assert.Null(testValue);

			IBasicService newValue = new BasicService();

			injectionContext.Export((x, y) => newValue);

			Assert.Null(injectionContext.Locate<IImportConstructorService>());
		}

		[Fact]
		public void PropertyTargetInfoTest()
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
					ioc.ExportFunc((scope, context) =>
					               {
						               IInjectionTargetInfo targetInfo = context.TargetInfo;

						               Assert.NotNull(targetInfo);

						               Assert.NotNull(targetInfo.InjectionType);
						               Assert.Equal(typeof(ImportPropertyService), targetInfo.InjectionType);

						               Assert.NotNull(targetInfo.InjectionTypeAttributes);
						               Assert.Equal(1, targetInfo.InjectionTypeAttributes.Count());

						               Assert.NotNull(targetInfo.InjectionTarget);

						               Assert.NotNull(targetInfo.InjectionTargetAttributes);
						               Assert.Equal(1, targetInfo.InjectionTargetAttributes.Count());

						               return new BasicService();
					               }).As<IBasicService>();

					ioc.ExportFunc((scope, context) => new ImportPropertyService())
						.As<IImportPropertyService>()
						.ImportProperty(x => x.BasicService);
				});

			IImportPropertyService importPropertyService = injectionKernel.Locate<IImportPropertyService>();

			Assert.NotNull(importPropertyService);
			Assert.NotNull(importPropertyService.BasicService);
		}

		[Fact]
		public void ConstructorTargetInfoTest()
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
					ioc.ExportFunc((scope, context) =>
					               {
						               IInjectionTargetInfo targetInfo = context.TargetInfo;

						               Assert.NotNull(targetInfo);

						               Assert.NotNull(targetInfo.InjectionType);
						               Assert.Equal(typeof(ImportConstructorService), targetInfo.InjectionType);

						               Assert.NotNull(targetInfo.InjectionTypeAttributes);
						               Assert.Equal(1, targetInfo.InjectionTypeAttributes.Count());

						               Assert.NotNull(targetInfo.InjectionTarget);

						               Assert.NotNull(targetInfo.InjectionTargetAttributes);
						               Assert.Equal(1, targetInfo.InjectionTargetAttributes.Count());

						               return new BasicService();
					               }).As<IBasicService>();

					ioc.Export<ImportConstructorService>().As<IImportConstructorService>();
				});

			IImportConstructorService importPropertyService = injectionKernel.Locate<IImportConstructorService>();

			Assert.NotNull(importPropertyService);
		}

		[Fact]
		public void ScopeContextInfoTest()
		{
			InjectionKernelManager manager = new InjectionKernelManager(null,
				DependencyInjectionContainer.CompareExportStrategies,
				new BlackList());
			InjectionKernel injectionKernel = new InjectionKernel(manager,
				null,
				null,
				null,
				DependencyInjectionContainer.CompareExportStrategies);

			IInjectionScope requestScope = injectionKernel.CreateChildScope();

			injectionKernel.Configure(
				ioc =>
				{
					ioc.ExportFunc((scope, context) =>
					               {
						               Assert.True(ReferenceEquals(injectionKernel, scope));

						               Assert.True(ReferenceEquals(requestScope, context.RequestingScope));

						               return new BasicService();
					               }).As<IBasicService>();

					ioc.Export<ImportConstructorService>().As<IImportConstructorService>();
				});

			IImportConstructorService importPropertyService = requestScope.Locate<IImportConstructorService>();

			Assert.NotNull(importPropertyService);
		}

		[Fact]
		public void CloneScopeInfoTest()
		{
			InjectionKernelManager manager = new InjectionKernelManager(null,
				DependencyInjectionContainer.CompareExportStrategies,
				new BlackList());
			InjectionKernel injectionKernel = new InjectionKernel(manager,
				new FauxInjectionScope(),
				null,
				null,
				DependencyInjectionContainer.CompareExportStrategies);

			IInjectionScope cloneScope = null;
			IInjectionScope requestScope = null;

			injectionKernel.Configure(
				ioc =>
				{
					ioc.ExportFunc((scope, context) =>
					               {
						               Assert.True(ReferenceEquals(requestScope, context.RequestingScope),
							               "Requesting scope incorrect");

						               return new BasicService();
					               }).As<IBasicService>();

					ioc.Export<ImportConstructorService>().As<IImportConstructorService>();
				});

			cloneScope = injectionKernel.Clone(null, null, null);
			requestScope = cloneScope.CreateChildScope();

			IImportConstructorService importPropertyService = requestScope.Locate<IImportConstructorService>();

			Assert.NotNull(importPropertyService);
		}
	}
}