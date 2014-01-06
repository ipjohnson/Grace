using System;
using Grace.DependencyInjection;
using Grace.Moq;
using Grace.UnitTests.Classes.Simple;
using Moq;
using Xunit;

namespace Grace.UnitTests.Moq
{
	public class MoqTests
	{
		[Fact]
		public void AutoMoqContainerTest()
		{
			MoqContainer container = new MoqContainer();

			ImportConstructorService service = container.Locate<ImportConstructorService>();

			Assert.NotNull(service);
			Assert.NotNull(service.BasicService);
		}

		[Fact]
		public void AutoMoqSetupTest()
		{
			MoqContainer container = new MoqContainer();

			ImportConstructorService service = container.Locate<ImportConstructorService>();

			container.Locate<Mock<IBasicService>>().Setup(x => x.TestMethod()).Returns(5);

			Assert.NotNull(service);
			Assert.Equal(5, service.TestMethod());
		}

		[Fact]
		public void MoqContainerSetupTest()
		{
			MoqContainer container = new MoqContainer();

			container.Configure(c => c.Export<ImportConstructorService>().As<IImportConstructorService>());

			container.Locate<Mock<IBasicService>>().Setup(x => x.TestMethod()).Returns(5);

			IImportConstructorService service = container.Locate<IImportConstructorService>();

			Assert.NotNull(service);
			Assert.Equal(5, service.TestMethod());

			container.Locate<Mock<IBasicService>>().Verify(x => x.TestMethod(), Times.Once);
		}

		[Fact]
		public void MoqExtensionTest()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Moq();

			container.Configure(c => c.Export<ImportConstructorService>().As<IImportConstructorService>());

			container.Locate<Mock<IBasicService>>().Setup(x => x.TestMethod()).Returns(5);

			IImportConstructorService service = container.Locate<IImportConstructorService>();

			Assert.NotNull(service);
			Assert.Equal(5, service.TestMethod());

			container.Locate<Mock<IBasicService>>().Verify(x => x.TestMethod(), Times.Once);
		}

		[Fact]
		public void RegisterTransientMock()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c =>
			                    {
				                    c.Export<ImportConstructorService>().As<IImportConstructorService>();
				                    c.Moq<IBasicService>().Arrange(m => m.Setup(x => x.TestMethod()).Returns(5));
			                    });

			IImportConstructorService constructorService = container.Locate<IImportConstructorService>();

			Assert.NotNull(constructorService);
			Assert.NotNull(constructorService.BasicService);
			Assert.Equal(5, constructorService.BasicService.TestMethod());

			Assert.Equal(5, container.Locate<IBasicService>().TestMethod());

			Assert.False(ReferenceEquals(constructorService.BasicService, container.Locate<IBasicService>()));
		}

		[Fact]
		public void RegisterSingletonMock()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c =>
			                    {
				                    c.Export<ImportConstructorService>().As<IImportConstructorService>();
				                    c.Moq<IBasicService>().Arrange(m => m.Setup(x => x.TestMethod()).Returns(5)).AndSingleton();
			                    });

			IImportConstructorService constructorService = container.Locate<IImportConstructorService>();

			Assert.NotNull(constructorService);
			Assert.Equal(5, constructorService.TestMethod());

			Assert.True(ReferenceEquals(constructorService.BasicService, container.Locate<IBasicService>()));
		}

		[Fact]
		public void VerifyMock()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Moq();

			container.Configure(c =>
			                    {
				                    c.Export<ImportConstructorService>().As<IImportConstructorService>();
				                    c.Moq<IBasicService>().
					                    Arrange(m => m.Setup(x => x.TestMethod()).Returns(5)).
					                    Assert(m => m.Verify(b => b.TestMethod(), Times.Once)).
					                    AndSingleton();
			                    });

			IImportConstructorService constructorService = container.Locate<IImportConstructorService>();

			Assert.NotNull(constructorService);
			Assert.Equal(5, constructorService.TestMethod());

			container.Assert();
		}

		[Fact]
		public void VerifyFailMock()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Moq();

			container.Configure(c =>
			                    {
				                    c.Export<ImportConstructorService>().As<IImportConstructorService>();
				                    c.Moq<IBasicService>().
					                    Arrange(m => m.Setup(x => x.TestMethod()).Returns(5)).
					                    Assert(m => m.Verify(b => b.TestMethod(), Times.Once)).
					                    AndSingleton();
			                    });

			IImportConstructorService constructorService = container.Locate<IImportConstructorService>();

			Assert.NotNull(constructorService);

			constructorService.TestMethod();
			constructorService.TestMethod();

			try
			{
				container.Assert();

				throw new Exception("Should have thrown a mock exception and failed");
			}
			catch (MockException exp)
			{
			}
		}
	}
}