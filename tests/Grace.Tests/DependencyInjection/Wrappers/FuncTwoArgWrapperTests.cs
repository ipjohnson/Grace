using System;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl.Wrappers;
using Grace.Tests.Classes.Simple;
using SimpleFixture.NSubstitute;
using SimpleFixture.xUnit;
using Xunit;

namespace Grace.Tests.DependencyInjection.Wrappers
{
    public class FuncTwoArgWrapperTests
    {
        [Fact]
        public void FuncTwoArg_Locate_Instance()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export(typeof(TwoDependencyService<,>)).As(typeof(ITwoDependencyService<,>)));

            var func = container.Locate<Func<int, double, ITwoDependencyService<int, double>>>();

            var instance = func(5, 10);

            Assert.NotNull(instance);
            Assert.Equal(5, instance.Dependency1);
            Assert.Equal(10, instance.Dependency2);
        }
        
        [Theory]
        [AutoData]
        [SubFixtureInitialize]
        public void FuncTwoArg_GetWrappedType(FuncTwoArgWrapperStrategy strategy)
        {
            Assert.Equal(typeof(IBasicService),
                strategy.GetWrappedType(typeof(Func<int, int, IBasicService>)));
        }

        [Theory]
        [AutoData]
        [SubFixtureInitialize]
        public void FuncTwoArg_GetWrappedType_NonGeneric(FuncTwoArgWrapperStrategy strategy)
        {
            Assert.Equal(null, strategy.GetWrappedType(typeof(BasicService)));
        }
    }
}
