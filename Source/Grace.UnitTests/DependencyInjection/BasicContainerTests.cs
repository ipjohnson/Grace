using System;
using System.Collections.Generic;
using System.Linq;
using Grace.DependencyInjection;
using Grace.UnitTests.Classes.Simple;
using Xunit;

namespace Grace.UnitTests.DependencyInjection
{
	public class BasicContainerTests
	{
		[Fact]
		public void BlackOutListTest()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.BlackListExportType(typeof(BasicService));

			container.Configure(
				ioc => ioc.Export<BasicService>().As<IBasicService>());

			IBasicService basicService = container.Locate<IBasicService>();

			Assert.Null(basicService);
		}

		[Fact]
		public void BlackOutListByNameTest()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.BlackListExport(typeof(BasicService).FullName);

			container.Configure(
				ioc => ioc.Export<BasicService>().As<IBasicService>());

			IBasicService basicService = container.Locate<IBasicService>();

			Assert.Null(basicService);
		}

		[Fact]
		public void SimpleRegistrationTest()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c => c.Export<BasicService>().As<IBasicService>().AndSingleton());

			IBasicService basicService = container.RootScope.Locate<IBasicService>();

			Assert.NotNull(basicService);
		}

		[Fact]
		public void TransientRegistrationTest()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(
				c =>
				{
					c.Export<BasicService>().As<IBasicService>().AndSingleton();
					c.Export<Transient>().As<ITransient>().ImportProperty(x => x.BasicService);
				});

			ITransient transient = container.RootScope.Locate<ITransient>();

			Assert.NotNull(transient);
			Assert.NotNull(transient.BasicService);
		}

		[Fact]
		public void FilterRunTime()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer(ExportEnvironment.RunTime);

			container.Configure(c =>
			                    {
				                    c.Export<SimpleObjectA>().As<ISimpleObject>().InEnvironment(ExportEnvironment.RunTimeOnly);
				                    c.Export<SimpleObjectB>().As<ISimpleObject>().InEnvironment(ExportEnvironment.UnitTestOnly);
				                    c.Export<SimpleObjectC>().As<ISimpleObject>().InEnvironment(ExportEnvironment.DesignTimeOnly);
			                    });

			ISimpleObject simpleObject = container.Locate<ISimpleObject>();

			Assert.NotNull(simpleObject);
			Assert.IsType(typeof(SimpleObjectA), simpleObject);

			Assert.Equal(1, container.RootScope.GetStrategies(typeof(ISimpleObject)).Count());
		}

		[Fact]
		public void FilterUnitTest()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer(ExportEnvironment.UnitTest);

			container.Configure(c =>
			                    {
				                    c.Export<SimpleObjectA>().As<ISimpleObject>().InEnvironment(ExportEnvironment.RunTimeOnly);
				                    c.Export<SimpleObjectB>().As<ISimpleObject>().InEnvironment(ExportEnvironment.UnitTestOnly);
				                    c.Export<SimpleObjectC>().As<ISimpleObject>().InEnvironment(ExportEnvironment.DesignTimeOnly);
			                    });

			ISimpleObject simpleObject = container.Locate<ISimpleObject>();

			Assert.NotNull(simpleObject);
			Assert.IsType(typeof(SimpleObjectB), simpleObject);

			Assert.Equal(1, container.RootScope.GetStrategies(typeof(ISimpleObject)).Count());
		}

		[Fact]
		public void FilterDesignTime()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer(ExportEnvironment.DesignTime);

			container.Configure(c =>
			                    {
				                    c.Export<SimpleObjectA>().As<ISimpleObject>().InEnvironment(ExportEnvironment.RunTimeOnly);
				                    c.Export<SimpleObjectB>().As<ISimpleObject>().InEnvironment(ExportEnvironment.UnitTestOnly);
				                    c.Export<SimpleObjectC>().As<ISimpleObject>().InEnvironment(ExportEnvironment.DesignTimeOnly);
			                    });

			ISimpleObject simpleObject = container.Locate<ISimpleObject>();

			Assert.NotNull(simpleObject);
			Assert.IsType(typeof(SimpleObjectC), simpleObject);

			Assert.Equal(1, container.RootScope.GetStrategies(typeof(ISimpleObject)).Count());
		}

		[Fact]
		public void MissingExportTest()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c => c.Export<ImportConstructorService>().As<IImportConstructorService>());

			container.ResolveUnknownExports += (sender, args) => { args.ExportedValue = new BasicService(); };

			IImportConstructorService constructorService = container.Locate<IImportConstructorService>();

			Assert.NotNull(constructorService);
		}

		[Fact]
		public void MissingExportFromChildScopeTest()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c => c.Export<ImportConstructorService>().As<IImportConstructorService>());

			container.ResolveUnknownExports += (sender, args) => { args.ExportedValue = new BasicService(); };

			IInjectionScope childScope = container.CreateChildScope();

			IImportConstructorService constructorService = childScope.Locate<IImportConstructorService>();

			Assert.NotNull(constructorService);
		}

		[Fact]
		public void AutoCreateTest()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c => c.Export<BasicService>().As<IBasicService>());

			ImportConstructorService constructorService = container.Locate<ImportConstructorService>();

			Assert.NotNull(constructorService);
		}

		[Fact]
		public void ExceptionThrownTest()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer { ThrowExceptions = true };

			container.Configure(c => c.Export<ImportConstructorService>().As<IImportConstructorService>());

			IImportConstructorService constructorService = null;
			bool exceptionThrown = false;

			try
			{
				constructorService = container.Locate<IImportConstructorService>();
			}
			catch (Exception)
			{
				exceptionThrown = true;
			}

			Assert.Null(constructorService);
			Assert.True(exceptionThrown);
		}

		[Fact]
		public void ExceptionNotThrownTest()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer { ThrowExceptions = false };

			container.Configure(c => c.Export<ImportConstructorService>().As<IImportConstructorService>());

			IImportConstructorService constructorService = null;
			bool exceptionThrown = false;

			try
			{
				constructorService = container.Locate<IImportConstructorService>();
			}
			catch (Exception exp)
			{
				exceptionThrown = true;
			}

			Assert.Null(constructorService);
			Assert.False(exceptionThrown);
		}

		[Fact]
		public void InjectContainerTest()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c => c.Export<DependencyInjectionContainerImportService>());

			DependencyInjectionContainerImportService service = container.Locate<DependencyInjectionContainerImportService>();

			Assert.NotNull(service);
			Assert.NotNull(service.Container);
			Assert.Same(container, service.Container);
		}

		[Fact]
		public void InjectionContextResolveTest()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c => c.Export<BasicService>().As<IBasicService>());

			InjectionContext context = new InjectionContext(container.RootScope)
			                           {
				                           { "stringParam", "Hello" },
				                           { "intParam", (x, y) => 7 }
			                           };

			WithCtorParamClass paramClass = container.Locate<WithCtorParamClass>(context);

			Assert.NotNull(paramClass);
			Assert.Equal("Hello", paramClass.StringParam);
			Assert.Equal(7, paramClass.IntParam);
		}

		[Fact]
		public void ImportDateTime()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c => c.Export<ImportDateTimeByName>());

			InjectionContext context = new InjectionContext(container.RootScope)
			                           {
				                           { "DateTime", (x, y) => DateTime.Now }
			                           };

			ImportDateTimeByName importName = container.Locate<ImportDateTimeByName>(context);

			Assert.NotNull(importName);
			Assert.Equal(DateTime.Today, importName.DateTime.Date);
		}

		[Fact]
		public void ImportByNameDateTime()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c =>
			                    {
				                    c.Export<ImportDateTimeByName>();
				                    c.ExportInstance((x, y) => DateTime.Now).AsName("DateTime");
			                    });

			ImportDateTimeByName importName = container.Locate<ImportDateTimeByName>();

			Assert.NotNull(importName);
			Assert.Equal(DateTime.Today, importName.DateTime.Date);
		}

		[Fact]
		public void ImportFuncByNameDateTime()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c =>
			                    {
				                    c.Export<ImportDateTimeByName>();
				                    c.ExportFunc((scope, context) => DateTime.Now).AsName("DateTime");
			                    });

			ImportDateTimeByName importName = container.Locate<ImportDateTimeByName>();

			Assert.NotNull(importName);
			Assert.Equal(DateTime.Today, importName.DateTime.Date);
		}

		[Fact]
		public void QueryableExample()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c => (from type in Types.FromThisAssembly() 
											  where type.Namespace.Contains("Simple")
											  select type)
											 .ExportTo(c)
											 .ByInterfaces());

			IImportConstructorService constructorService = container.Locate<IImportConstructorService>();
		}

		[Fact]
		public void ImportPropertyExample()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c => c.Export(Types.FromThisAssembly())
												.ByInterfaces()
												.ImportProperty<IBasicService>());

			IImportPropertyService importService = container.Locate<IImportPropertyService>();

			Assert.NotNull(importService);
			Assert.NotNull(importService.BasicService);
		}
	}
}