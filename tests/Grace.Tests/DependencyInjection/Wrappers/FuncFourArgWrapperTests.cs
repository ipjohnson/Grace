using System;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl.Wrappers;
using Grace.Tests.Classes.Simple;
using SimpleFixture.NSubstitute;
using SimpleFixture.xUnit;
using Xunit;

namespace Grace.Tests.DependencyInjection.Wrappers
{
    public class FuncFourArgWrapperTests
    {
        [Fact]
        public void FuncFourArg_Locate_Instance()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export(typeof(FourDependencyService<,,,>)).As(typeof(IFourDependencyService<,,,>)));

            var func = container.Locate<Func<int, double, string, bool, IFourDependencyService<int, double, string, bool>>>();

            var instance = func(5, 10, "hello", true);

            Assert.NotNull(instance);
            Assert.Equal(5, instance.Dependency1);
            Assert.Equal(10, instance.Dependency2);
            Assert.Equal("hello", instance.Dependency3);
            Assert.True(instance.Dependency4);
        }
        
        [Theory]
        [AutoData]
        [SubFixtureInitialize]
        public void FuncFourArg_GetWrappedType(FuncFourArgWrapperStrategy strategy)
        {
            Assert.Equal(typeof(IBasicService),
                strategy.GetWrappedType(typeof(Func<int, int, int, int, IBasicService>)));
        }

        [Theory]
        [AutoData]
        [SubFixtureInitialize]
        public void FuncFourArg_GetWrappedType_NonGeneric(FuncFourArgWrapperStrategy strategy)
        {
            Assert.Null(strategy.GetWrappedType(typeof(BasicService)));
        }
    }
}
