using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Generics
{
    public class GenericConfigurationTests
    {
        [Fact]
        public void Generic_Constructor_Parameter_Not_Required()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export(typeof(DependentService<>)).As(typeof(IDependentService<>)).WithCtorParam<object>().Named("value").IsRequired(false));

            var instance = container.Locate<IDependentService<IBasicService>>();

            Assert.NotNull(instance);
            Assert.Null(instance.Value);
        }
    }
}
