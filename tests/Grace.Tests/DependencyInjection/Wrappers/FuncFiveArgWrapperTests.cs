using System;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using SimpleFixture.NSubstitute;
using SimpleFixture.xUnit;
using Xunit;

namespace Grace.Tests.DependencyInjection.Wrappers
{
    public class FuncFiveArgWrapperTests
    {
        [Fact]
        public void FuncFiveArg_Locate_Instance()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export(typeof(FiveDependencyService<,,,,>)).As(typeof(IFiveDependencyService<,,,,>)));

            var func = container.Locate<Func<int, double, string, bool, byte, IFiveDependencyService<int, double, string, bool, byte>>>();

            var instance = func(5, 10, "hello", true, 3);

            Assert.NotNull(instance);
            Assert.Equal(5, instance.Dependency1);
            Assert.Equal(10, instance.Dependency2);
            Assert.Equal("hello", instance.Dependency3);
            Assert.Equal(true, instance.Dependency4);
            Assert.Equal(3, instance.Dependency5);
        }

        [Theory]
        [AutoData]
        [SubFixtureInitialize]
        public void FuncFiveArg_GetWrappedType(Grace.DependencyInjection.Impl.Wrappers.FuncFiveArgWrapperStrategy strategy)
        {
            Assert.Equal(typeof(IBasicService),
                         strategy.GetWrappedType(typeof(Func<int,int,int,int,int,IBasicService>)));
        }
        
        [Theory]
        [AutoData]
        [SubFixtureInitialize]
        public void FuncFiveArg_GetWrappedType_NonGeneric(Grace.DependencyInjection.Impl.Wrappers.FuncFiveArgWrapperStrategy strategy)
        {
            Assert.Equal(null, strategy.GetWrappedType(typeof(BasicService)));
        }
    }
}
