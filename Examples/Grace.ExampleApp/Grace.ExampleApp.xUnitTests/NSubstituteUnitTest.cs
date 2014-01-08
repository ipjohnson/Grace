using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.ExampleApp.DependencyInjection.ExampleClasses;
using Grace.NSubstitute;
using NSubstitute;
using Xunit;

namespace Grace.ExampleApp.UnitTests
{

	public class NSubstituteUnitTest
	{
		[Fact]
		public void SimpleExample()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Substitute();

			container.Configure(c => c.Export<ImportConstructor>());

			IImportConstructor constructor = container.Locate<IImportConstructor>();

			Assert.NotNull(constructor);
			Assert.NotNull(constructor.BasicService);
		}

		[Fact]
		public void SetReturnExample()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Substitute();

			container.Configure(c => c.Export<ImportConstructor>());

			container.Locate<IBasicService>().SomeMethod().Returns(5);

			ImportConstructor constructor = container.Locate<ImportConstructor>();

			Assert.NotNull(constructor);
			Assert.NotNull(constructor.BasicService);

			Assert.Equal(5, constructor.BasicService.SomeMethod());
		}

		[Fact]
		public void ConfigureExample()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Substitute();

			container.Configure(c =>
				                    {
					                    c.Export<ImportConstructor>();
					                    c.Substitute<IBasicService>().Arrange(x => x.SomeMethod().Returns(5));
				                    });

			ImportConstructor importConstructor = container.Locate<ImportConstructor>();

			Assert.NotNull(importConstructor);
			Assert.NotNull(importConstructor.BasicService);

			Assert.Equal(5, importConstructor.BasicService.SomeMethod());
		}
	}
}
