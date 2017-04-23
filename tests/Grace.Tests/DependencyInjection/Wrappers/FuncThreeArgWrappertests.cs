using System;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl.Wrappers;
using Grace.Tests.Classes.Simple;
using SimpleFixture.NSubstitute;
using SimpleFixture.xUnit;
using Xunit;

namespace Grace.Tests.DependencyInjection.Wrappers
{
    public class FuncThreeArgWrapperTests
    {
        [Fact]
        public void FuncThreeArg_Locate_Instance()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export(typeof(ThreeDependencyService<,,>)).As(typeof(IThreeDependencyService<,,>)));

            var func = container.Locate<Func<int, double, string, IThreeDependencyService<int, double, string>>>();

            var instance = func(5, 10, "hello");

            Assert.NotNull(instance);
            Assert.Equal(5, instance.Dependency1);
            Assert.Equal(10, instance.Dependency2);
            Assert.Equal("hello", instance.Dependency3);
        }

        [Theory]
        [AutoData]
        [SubFixtureInitialize]
        public void FuncThreeArg_GetWrappedType(FuncThreeArgWrapperStrategy strategy)
        {
            Assert.Equal(typeof(IBasicService),
                strategy.GetWrappedType(typeof(Func<int, int, int, IBasicService>)));
        }

        [Theory]
        [AutoData]
        [SubFixtureInitialize]
        public void FuncThreeArg_GetWrappedType_NonGeneric(FuncThreeArgWrapperStrategy strategy)
        {
            Assert.Equal(null, strategy.GetWrappedType(typeof(BasicService)));
        }
    }
}
