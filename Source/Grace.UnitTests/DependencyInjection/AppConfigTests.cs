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
		public void ConfigureWithXmlTest()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.ConfigureWithXml();

			IBasicService basicService = container.Locate<IBasicService>();

			Assert.NotNull(basicService);
		}
	}
}
