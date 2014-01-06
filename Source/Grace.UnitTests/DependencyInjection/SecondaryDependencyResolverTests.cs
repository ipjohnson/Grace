using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl;
using Grace.UnitTests.Classes.Simple;
using Xunit;

namespace Grace.UnitTests.DependencyInjection
{
	public class SecondaryDependencyResolverTests
	{
		[Fact]
		public void BasicResolverTest()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			SimpleSecondaryExportLocator resolver = new SimpleSecondaryExportLocator();

			container.AddSecondaryLocator(resolver);
			container.Configure(c => c.Export<ImportConstructorService>().As<IImportConstructorService>());

			resolver.AddResolveValue((IBasicService)new BasicService());

			IImportConstructorService constructorService = container.Locate<IImportConstructorService>();

			Assert.NotNull(constructorService);
		}

		[Fact]
		public void ResolveFromChildScope()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			SimpleSecondaryExportLocator resolver = new SimpleSecondaryExportLocator();

			container.AddSecondaryLocator(resolver);
			container.Configure(c => c.Export<ImportConstructorService>().As<IImportConstructorService>());

			resolver.AddResolveValue((IBasicService)new BasicService());

			IInjectionScope childScope = container.CreateChildScope();

			IImportConstructorService constructorService = childScope.Locate<IImportConstructorService>();

			Assert.NotNull(constructorService);
		}

		[Fact]
		public void ResolveValueTypeTest()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			SimpleSecondaryExportLocator resolver = new SimpleSecondaryExportLocator();

			container.AddSecondaryLocator(resolver);
			container.Configure(c => c.Export<WithCtorParamClass>());

			resolver.AddResolveValue(() => (IBasicService)new BasicService());
			resolver.AddResolveValue("stringParam", "Hello");
			resolver.AddResolveValue("intParam", 10);

			WithCtorParamClass paramClass = container.Locate<WithCtorParamClass>();

			Assert.NotNull(paramClass);
			Assert.Equal("Hello", paramClass.StringParam);
			Assert.Equal(10, paramClass.IntParam);
		}

		[Fact]
		public void ResolveValueTypeFromChildTest()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			SimpleSecondaryExportLocator resolver = new SimpleSecondaryExportLocator();

			container.AddSecondaryLocator(resolver);
			container.Configure(c => c.Export<WithCtorParamClass>());

			resolver.AddResolveValue(() => (IBasicService)new BasicService());
			resolver.AddResolveValue("stringParam", "Hello");
			resolver.AddResolveValue("intParam", 10);

			WithCtorParamClass paramClass = container.Locate<WithCtorParamClass>();

			Assert.NotNull(paramClass);
			Assert.Equal("Hello", paramClass.StringParam);
			Assert.Equal(10, paramClass.IntParam);
		}
	}
}