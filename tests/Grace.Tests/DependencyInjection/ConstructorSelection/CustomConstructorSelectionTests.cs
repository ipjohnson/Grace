using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.ConstructorSelection
{
    public class CustomConstructorSelectionTests
    {
        [Fact]
        public void CustomConstructor_TimedInstantiation()
        {
            var container = new DependencyInjectionContainer(
                c => c.Behaviors.ConstructorSelection = new TimedConstructorSelectionMethod());

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>();
            });

            var instance = container.Locate<DependentService<IBasicService>>();

            Assert.NotNull(instance);
        }
    }
}
