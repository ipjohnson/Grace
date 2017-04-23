using System;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl.Wrappers;
using Grace.Tests.Classes.Simple;
using SimpleFixture.NSubstitute;
using SimpleFixture.xUnit;
using Xunit;

namespace Grace.Tests.DependencyInjection.Wrappers
{
    public class FuncOneArgWrapperTests
    {
        [Fact]
        public void FuncOneArg_Create_Instance()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>();
                c.Export(typeof(TwoDependencyService<,>)).As(typeof(ITwoDependencyService<,>));
                c.Export(typeof(DependsOnOneArgFunc<,>)).As(typeof(IDependsOnOneArgFunc<,>));
            });

            var instance = container.Locate<IDependsOnOneArgFunc<IBasicService, int>>();

            var twoService = instance.CreateWithT2(5);

            Assert.NotNull(twoService);
            Assert.NotNull(twoService.Dependency1);
            Assert.IsType<BasicService>(twoService.Dependency1);
            Assert.Equal(5, twoService.Dependency2);
        }

        [Theory]
        [AutoData]
        [SubFixtureInitialize]
        public void FuncOneArg_GetWrappedType(FuncOneArgWrapperStrategy strategy)
        {
            Assert.Equal(typeof(IBasicService),
                strategy.GetWrappedType(typeof(Func<int, IBasicService>)));
        }

        [Theory]
        [AutoData]
        [SubFixtureInitialize]
        public void FuncOneArg_GetWrappedType_NonGeneric(FuncOneArgWrapperStrategy strategy)
        {
            Assert.Equal(null, strategy.GetWrappedType(typeof(BasicService)));
        }
    }
}
