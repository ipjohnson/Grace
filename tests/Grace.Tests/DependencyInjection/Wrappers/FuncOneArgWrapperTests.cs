using System;
using System.Collections.Generic;
using System.Linq;
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
        public interface ITest
        {
            int Value { get; }    
        }

        public class Test1 : ITest
        {
            public Test1(int value)
            {
                Value = value;
            }

            public int Value { get; }
        }
        
        public class Test2 : ITest
        {
            public Test2(int value)
            {
                Value = value;
            }

            public int Value { get; }
        }

        [Fact]
        public void FuncArg_Test()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.ExportFactory<int, ITest>(i => new Test1(i));
                c.ExportFactory<int, ITest>(i => new Test2(i));
            });

            var valueFunc = container.Locate<IEnumerable<Func<int, ITest>>>().ToArray();

            var value1 = valueFunc[0](5);
            var value2 = valueFunc[1](10);
        }

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
