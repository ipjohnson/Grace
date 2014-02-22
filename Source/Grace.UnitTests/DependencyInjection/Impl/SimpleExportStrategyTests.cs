using Grace.DependencyInjection;
using Grace.UnitTests.Classes.Simple;
using Xunit;

namespace Grace.UnitTests.DependencyInjection.Impl
{
	public class SimpleExportStrategyTests
	{
		[Fact]
		public void BasicExportTest()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c =>
			                    {
				                    c.SimpleExport<BasicService>().As<IBasicService>();
				                    c.SimpleExport<ConstructorImportService>().As<IConstructorImportService>();
			                    });

			IConstructorImportService transient = container.Locate<IConstructorImportService>();

			Assert.NotNull(transient);
			Assert.NotNull(transient.BasicService);
		}

		[Fact]
		public void GenericExportTest()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c =>
			                    {
				                    c.SimpleExport(typeof(GenericService<>)).As(typeof(IGenericService<>));
				                    c.SimpleExport(typeof(GenericTransient<>)).As(typeof(IGenericTransient<>));
			                    });

			IGenericTransient<int> genericTransient = container.Locate<IGenericTransient<int>>();

			Assert.NotNull(genericTransient);
		}
	}
}