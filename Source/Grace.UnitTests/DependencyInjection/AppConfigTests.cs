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
	public class AppConfigTests
	{
		[Fact]
		public void ConfigureWithXmlModuleTest()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.ConfigureWithXml();

			IBasicService basicService = container.Locate<IBasicService>();

			Assert.NotNull(basicService);
		}

		[Fact]
		public void ConfigureWithXmlModulePropertySetTest()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.ConfigureWithXml();

			int intProperty = (int)container.Locate("IntProperty");

			Assert.Equal(5,intProperty);
		}

		[Fact]
		public void ConfigureWithXmlExportTest()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.ConfigureWithXml();

			IConstructorImportService importService = container.Locate<IConstructorImportService>();

			Assert.NotNull(importService);
		}

		[Fact]
		public void ConfigureWithXmlSecondarySection()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.ConfigureWithXml("secondGrace");

			IConstructorImportService importService = container.Locate<IConstructorImportService>();

			Assert.NotNull(importService);
		}
	}
}
