using System.Configuration;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Configuration;
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

			Assert.Equal(5, intProperty);
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

			Assert.Equal(5, container.Locate("IntProperty"));
		}

		[Fact]
		public void ConfigureWithXmlThirdSectionWithShortNames()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.ConfigureWithXml("thirdGrace");

			IConstructorImportService importService = container.Locate<IConstructorImportService>();

			Assert.NotNull(importService);

			Assert.Equal(5, container.Locate("IntProperty"));
		}

		[Fact]
		public void RegisterConfigurationSection()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c => c.ExportConfigurationSection<GraceConfigurationSection>("grace"));

			var section = container.Locate<GraceConfigurationSection>();

			Assert.NotNull(section);

			Assert.Equal(1, section.Exports.Count);

			Assert.Equal(1, section.Modules.Count);
		}
	}
}