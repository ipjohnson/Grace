using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.UnitTests.Classes.Simple;
using Xunit;

namespace Grace.UnitTests.DependencyInjection
{
	public class IExportLocatorExtensionsTests
	{
		[Fact]
		public void WhatDoIHaveTest()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c => c.Export<ConstructorImportService>().AndSingleton());

			var child = container.CreateChildScope(c => c.Export<BasicService>().As<IBasicService>());

			string whatDoIHave = child.WhatDoIHave(true);

			Assert.NotNull(whatDoIHave);

			Assert.Contains("BasicService",whatDoIHave);
			Assert.Contains("ConstructorImportService",whatDoIHave);
			Assert.Contains("Singleton",whatDoIHave);
		}
	}
}
