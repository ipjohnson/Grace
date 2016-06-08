using Grace.DependencyInjection;
using Grace.UnitTests.Classes.Attributed;
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
			Assert.IsType(typeof(AttributeBasicService), service.BasicService);
		}

        [Fact]
        public void TryLocateString()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            object value;
            container.TryLocate(typeof(string), out value);

            Assert.Null(value);
        }
	}
}