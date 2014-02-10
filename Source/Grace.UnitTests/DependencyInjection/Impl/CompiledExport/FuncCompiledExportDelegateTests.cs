using System;
using System.Reflection;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Exceptions;
using Grace.DependencyInjection.Impl;
using Grace.DependencyInjection.Impl.CompiledExport;
using Grace.UnitTests.Classes.FauxClasses;
using Grace.UnitTests.Classes.Simple;
using Xunit;

namespace Grace.UnitTests.DependencyInjection.Impl.CompiledExport
{
	public class FuncCompiledExportDelegateTests
	{
		[Fact]
		public void SimpleExportTest()
		{
			CompiledExportDelegateInfo info = new CompiledExportDelegateInfo
			                                  {
				                                  ActivationType = typeof(IBasicService),
				                                  Attributes = new Attribute[0]
			                                  };

			FuncCompiledExportDelegate compiledExport =
				new FuncCompiledExportDelegate(info, (x, y) => new BasicService(), new FauxInjectionScope());

			ExportActivationDelegate activationDelegate = compiledExport.CompileDelegate();

			Assert.NotNull(activationDelegate);
			Assert.IsType(typeof(BasicService), activationDelegate(new FauxInjectionScope(), new FauxInjectionContext()));
		}

		[Fact]
		public void ImportPropertyRootTransientTest()
		{
			FauxExportStrategy basicService = new FauxExportStrategy(() => new BasicService())
			                                  {
				                                  ExportTypes = new[] { typeof(IBasicService) }
			                                  };

			FauxInjectionScope injectionScope = new FauxInjectionScope();

			injectionScope.AddStrategy(basicService);

			CompiledExportDelegateInfo info = new CompiledExportDelegateInfo
			                                  {
				                                  ActivationType = typeof(ImportPropertyService),
				                                  IsTransient = true,
				                                  Attributes = new Attribute[0]
			                                  };

			info.ImportProperty(new ImportPropertyInfo
			                    {
				                    Property =
					                    typeof(ImportPropertyService).GetProperty("BasicService")
			                    });

			FuncCompiledExportDelegate compiledExport =
				new FuncCompiledExportDelegate(info, (x, y) => new ImportPropertyService(), injectionScope);

			ExportActivationDelegate activationDelegate = compiledExport.CompileDelegate();

			Assert.NotNull(activationDelegate);

			IImportPropertyService propertyService =
				(IImportPropertyService)activationDelegate(injectionScope, new InjectionContext(null, injectionScope));

			Assert.NotNull(propertyService);
			Assert.NotNull(propertyService.BasicService);
		}

		[Fact]
		public void ImportPropertyRootNonTransientTest()
		{
			FauxExportStrategy basicService = new FauxExportStrategy(() => new BasicService())
			                                  {
				                                  ExportTypes = new[] { typeof(IBasicService) }
			                                  };

			FauxInjectionScope injectionScope = new FauxInjectionScope();

			injectionScope.AddStrategy(basicService);

			CompiledExportDelegateInfo info = new CompiledExportDelegateInfo
			                                  {
				                                  ActivationType = typeof(ImportPropertyService),
				                                  Attributes = new Attribute[0]
			                                  };

			info.ImportProperty(new ImportPropertyInfo
			                    {
				                    Property =
					                    typeof(ImportPropertyService).GetProperty("BasicService")
			                    });

			FuncCompiledExportDelegate compiledExport =
				new FuncCompiledExportDelegate(info, (x, y) => new ImportPropertyService(), injectionScope);

			ExportActivationDelegate activationDelegate = compiledExport.CompileDelegate();

			Assert.NotNull(activationDelegate);

			IImportPropertyService propertyService =
				(IImportPropertyService)activationDelegate(injectionScope, new InjectionContext(null, injectionScope));

			Assert.NotNull(propertyService);
			Assert.NotNull(propertyService.BasicService);
		}

		[Fact]
		public void ImportPropertyNonRootTransienTest()
		{
			FauxExportStrategy basicService = new FauxExportStrategy(() => new BasicService())
			                                  {
				                                  ExportTypes = new[] { typeof(IBasicService) }
			                                  };

			InjectionKernelManager manager =
				new InjectionKernelManager(null, DependencyInjectionContainer.CompareExportStrategies, new BlackList());
			InjectionKernel injectionScope =
				new InjectionKernel(manager, null, null, "Root", DependencyInjectionContainer.CompareExportStrategies);

			injectionScope.AddStrategy(basicService);

			CompiledExportDelegateInfo info = new CompiledExportDelegateInfo
			                                  {
				                                  ActivationType = typeof(ImportPropertyService),
				                                  IsTransient = true,
				                                  Attributes = new Attribute[0]
			                                  };

			info.ImportProperty(new ImportPropertyInfo
			                    {
				                    Property =
					                    typeof(ImportPropertyService).GetProperty("BasicService")
			                    });

			FuncCompiledExportDelegate compiledExport =
				new FuncCompiledExportDelegate(info, (x, y) => new ImportPropertyService(), injectionScope);

			ExportActivationDelegate activationDelegate = compiledExport.CompileDelegate();

			Assert.NotNull(activationDelegate);

			IInjectionScope requestingScope = injectionScope.CreateChildScope();

			IImportPropertyService propertyService =
				(IImportPropertyService)activationDelegate(injectionScope, new InjectionContext(null, requestingScope));

			Assert.NotNull(propertyService);
			Assert.NotNull(propertyService.BasicService);
		}

		[Fact]
		public void ImportPropertyNonRootNonTransienTest()
		{
			FauxExportStrategy basicService = new FauxExportStrategy(() => new BasicService())
			                                  {
				                                  ExportTypes = new[] { typeof(IBasicService) }
			                                  };

			FauxInjectionScope injectionScope = new FauxInjectionScope();

			injectionScope.AddStrategy(basicService);

			CompiledExportDelegateInfo info = new CompiledExportDelegateInfo
			                                  {
				                                  ActivationType = typeof(ImportPropertyService),
				                                  Attributes = new Attribute[0]
			                                  };

			info.ImportProperty(new ImportPropertyInfo
			                    {
				                    Property =
					                    typeof(ImportPropertyService).GetProperty("BasicService")
			                    });

			FuncCompiledExportDelegate compiledExport =
				new FuncCompiledExportDelegate(info, (x, y) => new ImportPropertyService(), injectionScope);

			ExportActivationDelegate activationDelegate = compiledExport.CompileDelegate();

			Assert.NotNull(activationDelegate);

			IImportPropertyService propertyService =
				(IImportPropertyService)activationDelegate(injectionScope, new InjectionContext(null, injectionScope));

			Assert.NotNull(propertyService);
			Assert.NotNull(propertyService.BasicService);
		}

		[Fact]
		public void PropertyImportProviderTest()
		{
			FauxInjectionScope injectionScope = new FauxInjectionScope();

			CompiledExportDelegateInfo info = new CompiledExportDelegateInfo
			                                  {
				                                  ActivationType = typeof(ImportPropertyService),
				                                  Attributes = new Attribute[0]
			                                  };

			info.ImportProperty(new ImportPropertyInfo
			                    {
				                    Property = typeof(ImportPropertyService).GetProperty("BasicService"),
				                    ValueProvider = new FuncValueProvider<IBasicService>(() => new BasicService())
			                    });

			FuncCompiledExportDelegate compiledExport =
				new FuncCompiledExportDelegate(info, (x, y) => new ImportPropertyService(), injectionScope);

			ExportActivationDelegate activationDelegate = compiledExport.CompileDelegate();

			Assert.NotNull(activationDelegate);

			IImportPropertyService propertyService =
				(IImportPropertyService)activationDelegate(injectionScope, new InjectionContext(null, injectionScope));

			Assert.NotNull(propertyService);
			Assert.NotNull(propertyService.BasicService);
		}

		[Fact]
		public void ImportMethodTest()
		{
			FauxExportStrategy basicService = new FauxExportStrategy(() => new BasicService())
			                                  {
				                                  ExportTypes = new[] { typeof(IBasicService) }
			                                  };

			FauxInjectionScope injectionScope = new FauxInjectionScope();

			injectionScope.AddStrategy(basicService);

			CompiledExportDelegateInfo info = new CompiledExportDelegateInfo
			                                  {
				                                  ActivationType = typeof(ImportMethodService),
				                                  Attributes = new Attribute[0]
			                                  };

			info.ImportMethod(new ImportMethodInfo
			                  {
				                  MethodToImport =
					                  typeof(ImportMethodService).GetRuntimeMethod("ImportMethod", new[] { typeof(IBasicService) })
			                  });

			FuncCompiledExportDelegate compiledExport =
				new FuncCompiledExportDelegate(info, (x, y) => new ImportMethodService(), injectionScope);

			ExportActivationDelegate activationDelegate = compiledExport.CompileDelegate();

			Assert.NotNull(activationDelegate);

			ImportMethodService propertyService =
				(ImportMethodService)activationDelegate(injectionScope, new InjectionContext(null, injectionScope));

			Assert.NotNull(propertyService);
		}

		[Fact]
		public void SimpleActivationMethodTest()
		{
			CompiledExportDelegateInfo info = new CompiledExportDelegateInfo
			                                  {
				                                  ActivationType = typeof(ActivateService),
				                                  IsTransient = true,
				                                  TrackDisposable = true,
				                                  Attributes = new Attribute[0]
			                                  };

			info.ActivateMethod(typeof(ActivateService).GetMethod("SimpleActivate"));

			FuncCompiledExportDelegate compiledExport =
				new FuncCompiledExportDelegate(info, (x, y) => new ActivateService(), new FauxInjectionScope());

			ExportActivationDelegate activationDelegate = compiledExport.CompileDelegate();

			Assert.NotNull(activationDelegate);

			ActivateService activateService =
				(ActivateService)activationDelegate(new FauxInjectionScope(), new FauxInjectionContext());

			Assert.NotNull(activateService);
			Assert.True(activateService.SimpleActivateCalled);
			Assert.False(activateService.InjectionContextActivateCalled);
		}

		[Fact]
		public void InjectionContextActivationMethodTest()
		{
			CompiledExportDelegateInfo info = new CompiledExportDelegateInfo
			                                  {
				                                  ActivationType = typeof(ActivateService),
				                                  IsTransient = true,
				                                  TrackDisposable = true,
				                                  Attributes = new Attribute[0]
			                                  };

			info.ActivateMethod(typeof(ActivateService).GetMethod("InjectionContextActivate"));

			FuncCompiledExportDelegate compiledExport =
				new FuncCompiledExportDelegate(info, (x, y) => new ActivateService(), new FauxInjectionScope());

			ExportActivationDelegate activationDelegate = compiledExport.CompileDelegate();

			Assert.NotNull(activationDelegate);

			ActivateService activateService =
				(ActivateService)activationDelegate(new FauxInjectionScope(), new FauxInjectionContext());

			Assert.NotNull(activateService);
			Assert.False(activateService.SimpleActivateCalled);
			Assert.True(activateService.InjectionContextActivateCalled);
		}

		[Fact]
		public void MultipleActivateTest()
		{
			CompiledExportDelegateInfo info = new CompiledExportDelegateInfo
			                                  {
				                                  ActivationType = typeof(ActivateService),
				                                  IsTransient = true,
				                                  TrackDisposable = true,
				                                  Attributes = new Attribute[0]
			                                  };

			info.ActivateMethod(typeof(ActivateService).GetMethod("InjectionContextActivate"));
			info.ActivateMethod(typeof(ActivateService).GetMethod("SimpleActivate"));

			FuncCompiledExportDelegate compiledExport =
				new FuncCompiledExportDelegate(info, (x, y) => new ActivateService(), new FauxInjectionScope());

			ExportActivationDelegate activationDelegate = compiledExport.CompileDelegate();

			Assert.NotNull(activationDelegate);

			ActivateService activateService =
				(ActivateService)activationDelegate(new FauxInjectionScope(), new FauxInjectionContext());

			Assert.NotNull(activateService);
			Assert.True(activateService.SimpleActivateCalled);
			Assert.True(activateService.InjectionContextActivateCalled);
		}

		[Fact]
		public void EnrichWithTest()
		{
			CompiledExportDelegateInfo info = new CompiledExportDelegateInfo
			                                  {
				                                  ActivationType = typeof(IBasicService),
				                                  Attributes = new Attribute[0]
			                                  };

			info.EnrichWithDelegate((scope, context, injectObject) => new EnrichContainer(injectObject));

			FuncCompiledExportDelegate compiledExport =
				new FuncCompiledExportDelegate(info, (x, y) => new BasicService(), new FauxInjectionScope());

			ExportActivationDelegate activationDelegate = compiledExport.CompileDelegate();

			Assert.NotNull(activationDelegate);
			EnrichContainer container =
				(EnrichContainer)activationDelegate(new FauxInjectionScope(), new FauxInjectionContext());

			Assert.NotNull(container);
			Assert.NotNull(container.EnrichedObject);
			Assert.IsType(typeof(BasicService), container.EnrichedObject);
		}

		[Fact]
		public void MultipleEnrichWithTest()
		{
			CompiledExportDelegateInfo info = new CompiledExportDelegateInfo
			                                  {
				                                  ActivationType = typeof(IBasicService),
				                                  Attributes = new Attribute[0]
			                                  };

			info.EnrichWithDelegate((scope, context, injectObject) => new EnrichContainer(injectObject));
			info.EnrichWithDelegate((scope, context, injectObject) => new EnrichContainer(injectObject));

			FuncCompiledExportDelegate compiledExport =
				new FuncCompiledExportDelegate(info, (x, y) => new BasicService(), new FauxInjectionScope());

			ExportActivationDelegate activationDelegate = compiledExport.CompileDelegate();

			Assert.NotNull(activationDelegate);
			EnrichContainer container =
				(EnrichContainer)activationDelegate(new FauxInjectionScope(), new FauxInjectionContext());

			Assert.NotNull(container);
			Assert.NotNull(container.EnrichedObject);

			EnrichContainer nestedContainer = (EnrichContainer)container.EnrichedObject;

			Assert.NotNull(nestedContainer.EnrichedObject);
			Assert.IsType(typeof(BasicService), nestedContainer.EnrichedObject);
		}

		[Fact]
		public void DisposalTest()
		{
			FauxInjectionScope injectionScope = new FauxInjectionScope();

			CompiledExportDelegateInfo info =
				new CompiledExportDelegateInfo
				{
					ActivationType = typeof(DisposableService),
					IsTransient = true,
					TrackDisposable = true,
					Attributes = new Attribute[0]
				};

			FuncCompiledExportDelegate compiledExport =
				new FuncCompiledExportDelegate(info, (x, y) => new DisposableService(), injectionScope);

			ExportActivationDelegate activationDelegate = compiledExport.CompileDelegate();

			Assert.NotNull(activationDelegate);

			FauxInjectionContext context = new FauxInjectionContext
			                               {
				                               DisposalScope = new DisposalScope()
			                               };

			IDisposableService disposableService = (IDisposableService)activationDelegate(injectionScope, context);

			bool eventFired = false;

			disposableService.Disposing += (sender, args) => eventFired = true;

			context.DisposalScope.Dispose();

			Assert.True(eventFired);
		}

		[Fact]
		public void MissingDisposalScopeTest()
		{
			FauxInjectionScope injectionScope = new FauxInjectionScope();

			CompiledExportDelegateInfo info =
				new CompiledExportDelegateInfo
				{
					ActivationType = typeof(DisposableService),
					IsTransient = true,
					TrackDisposable = true,
					Attributes = new Attribute[0]
				};

			FuncCompiledExportDelegate compiledExport =
				new FuncCompiledExportDelegate(info, (x, y) => new DisposableService(), injectionScope);

			ExportActivationDelegate activationDelegate = compiledExport.CompileDelegate();

			Assert.NotNull(activationDelegate);

			FauxInjectionContext context = new FauxInjectionContext();

			try
			{
				IDisposableService disposableService = (IDisposableService)activationDelegate(injectionScope, context);

				throw new Exception("Should have thrown a DisposalScopeMissingException");
			}
			catch (DisposalScopeMissingException)
			{
			}
		}
	}
}