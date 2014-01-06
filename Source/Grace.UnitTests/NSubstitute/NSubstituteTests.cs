using Grace.DependencyInjection;
using Grace.NSubstitute;
using Grace.UnitTests.Classes.Simple;
using NSubstitute;
using Xunit;

namespace Grace.UnitTests.NSubstitute
{
	public class NSubstituteTests
	{
		[Fact]
		public void BasicSubstituteTest()
		{
			SubstituteContainer container = new SubstituteContainer();

			var located = container.Locate<ImportConstructorService>();

			Assert.NotNull(located);
			Assert.NotNull(located.BasicService);
		}

		[Fact]
		public void ResolveTest()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Substitute();

			container.Configure(c => c.Export<ImportConstructorService>().As<IImportConstructorService>());

			container.Locate<IBasicService>().TestMethod().Returns(5);
			container.Locate<IBasicService>().Count.Returns(5);

			IImportConstructorService importConstructorService =
				container.Locate<IImportConstructorService>();

			Assert.NotNull(importConstructorService);
			Assert.NotNull(importConstructorService.BasicService);

			Assert.Equal(5, importConstructorService.BasicService.Count);
			Assert.Equal(5, importConstructorService.BasicService.TestMethod());
		}

		[Fact]
		public void SubstituteSingletonTest()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c =>
			                    {
				                    c.Export<ImportConstructorService>().As<IImportConstructorService>();
				                    c.Substitute<IBasicService>().AndSingleton();
			                    });

			container.Locate<IBasicService>().TestMethod().Returns(5);
			container.Locate<IBasicService>().Count.Returns(5);

			IImportConstructorService importConstructorService =
				container.Locate<IImportConstructorService>();

			Assert.NotNull(importConstructorService);
			Assert.NotNull(importConstructorService.BasicService);

			Assert.Equal(5, importConstructorService.BasicService.Count);
			Assert.Equal(5, importConstructorService.BasicService.TestMethod());
		}

		[Fact]
		public void SubstituteTransientTest()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c =>
			                    {
				                    c.Export<ImportConstructorService>().As<IImportConstructorService>();
				                    c.Substitute<IBasicService>().Arrange(service => service.Count.Returns(5));
			                    });

			IImportConstructorService importConstructorService =
				container.Locate<IImportConstructorService>();

			Assert.NotNull(importConstructorService);
			Assert.NotNull(importConstructorService.BasicService);

			Assert.Equal(5, importConstructorService.BasicService.Count);

			Assert.False(ReferenceEquals(importConstructorService.BasicService, container.Locate<IBasicService>()));
		}

		[Fact]
		public void SubstituteMultiple()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Substitute();

			container.Configure(c => c.Export<ImportIEnumerableService>().As<IImportIEnumerableService>());

			IImportIEnumerableService iEnumerableService = container.Locate<IImportIEnumerableService>();

			Assert.NotNull(iEnumerableService);
			Assert.Equal(1, iEnumerableService.Count);
		}
	}
}