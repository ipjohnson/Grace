using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.UnitTests.Classes.Attributed;
using Grace.UnitTests.Classes.Simple;
using Xunit;

namespace Grace.UnitTests.DependencyInjection
{
	public class InjectionTests
	{
		[Fact]
		public void AttributedPropertyInjection()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c => c.Export<AttributeBasicService>().As<IAttributeBasicService>());

			AttributedImportPropertyService service = new AttributedImportPropertyService();

			container.Inject(service);

			Assert.NotNull(service.BasicService);
			Assert.IsType(typeof(AttributeBasicService),service.BasicService);
		}
	}
}
