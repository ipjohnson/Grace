using System;
using System.Linq;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl.InstanceStrategies;
using SimpleFixture.NSubstitute;
using SimpleFixture.xUnit;
using Xunit;

namespace Grace.Tests.DependencyInjection.ExportInstance
{
    public class BaseInstanceTests
    {
        [Theory]
        [AutoData]
        [SubFixtureInitialize]
        public void BaseInstanceExportStrategy_AddSecondaryStrategy(BaseInstanceExportStrategy strategy, ICompiledExportStrategy addStrategy)
        {
            strategy.AddSecondaryStrategy(addStrategy);

            var array = strategy.SecondaryStrategies().ToArray();

            Assert.Equal(1, array.Length);
            Assert.Same(addStrategy, array[0]);
        }

        [Theory]
        [AutoData]
        [SubFixtureInitialize]
        public void BaseInstanceExportStrategy_AddSecondaryStrategy_Null(BaseInstanceExportStrategy strategy, ICompiledExportStrategy addStrategy)
        {
            Assert.Throws<ArgumentNullException>(() => strategy.AddSecondaryStrategy(null));
        }
    }
}
