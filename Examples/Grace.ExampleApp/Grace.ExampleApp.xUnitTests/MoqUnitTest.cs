using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.ExampleApp.DependencyInjection.ExampleClasses;
using Grace.Moq;
using Moq;
using Xunit;

namespace Grace.ExampleApp.UnitTests
{

	public class MoqUnitTest
	{
		/// <summary>
		/// MoqContainer is a wrapper around DependencyInjectionContainer
		/// </summary>
		[Fact]
		public void MoqContainer()
		{
			MoqContainer container = new MoqContainer();

			container.Configure(c => c.Export<ImportConstructor>());

			ImportConstructor importConstructor = container.Locate<ImportConstructor>();

			Assert.NotNull(importConstructor);
			Assert.NotNull(importConstructor.BasicService);
		}

		/// <summary>
		/// Moq extension works on a Container or any child injection scope
		/// </summary>
		[Fact]
		public void MoqExtension()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Moq();

			container.Configure(c => c.Export<ImportConstructor>());

			ImportConstructor importConstructor = container.Locate<ImportConstructor>();

			Assert.NotNull(importConstructor);
			Assert.NotNull(importConstructor.BasicService);
		}

		/// <summary>
		/// This example shows how you can setup a moq
		/// </summary>
		[Fact]
		public void SetupExample()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Moq();

			container.Configure(c => c.Export<ImportConstructor>());

			container.Locate<Mock<IBasicService>>().Setup(x => x.SomeMethod()).Returns(5);

			ImportConstructor importConstructor = container.Locate<ImportConstructor>();

			Assert.NotNull(importConstructor);
			Assert.NotNull(importConstructor.BasicService);

			Assert.Equal(5, importConstructor.BasicService.SomeMethod());
		}

		/// <summary>
		/// This example shows you how to configure a Moq ahead of time
		/// </summary>
		[Fact]
		public void ConfigureExample()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Moq();

			container.Configure(c =>
				                    {
					                    c.Export<ImportConstructor>();
											  c.Moq<IBasicService>().Setup(mock => mock.Setup(y => y.SomeMethod()).Returns(5));
				                    });

			ImportConstructor importConstructor = container.Locate<ImportConstructor>();

			Assert.NotNull(importConstructor);
			
			Assert.Equal(5, importConstructor.SomeMethod());
		}

		/// <summary>
		/// This example shows you how to verify all mock objects that have been created by the container.
		/// </summary>
		[Fact]
		public void Verify()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Moq();

			container.Configure(c =>
			{
				c.Export<ImportConstructor>();
				c.Moq<IBasicService>().
				  Setup(mock => mock.Setup(y => y.SomeMethod()).Returns(5)).
				  Verify(mock => mock.Verify(b => b.SomeMethod(), Times.Once));
			});

			ImportConstructor importConstructor = container.Locate<ImportConstructor>();

			Assert.NotNull(importConstructor);

			Assert.Equal(5, importConstructor.SomeMethod());

			container.Verify();
		}
	}
}
