using System.Linq;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.StrategyCollectionContainer
{
    public class StrategyCollectionContainerTests
    {
        [Fact]
        public void StrategyCollectionContainer_Clone_Test()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<BasicService>().As<IBasicService>());

            var cloneContainer = container.StrategyCollectionContainer.Clone();

            Assert.NotNull(cloneContainer);
            Assert.Equal(1, cloneContainer.GetAllStrategies().Count());
        }
    }
}
