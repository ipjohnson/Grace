using System;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Extensions;
using Grace.DependencyInjection.Impl.CompiledStrategies;
using Grace.Tests.Classes.Simple;
using SimpleFixture.NSubstitute;
using SimpleFixture.xUnit;
using Xunit;

namespace Grace.Tests.DependencyInjection.Factory
{
    [SubFixtureInitialize]
    public class ExportFuncTests
    {
        [Fact]
        public void ExportFunc_Dependency()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.ExportFunc<IBasicService>(scope => new BasicService());
                c.Export<DependentService<IBasicService>>().As<IDependentService<IBasicService>>();
            });

            var instance = container.Locate<IDependentService<IBasicService>>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.Value);
            Assert.IsType<BasicService>(instance.Value);
        }

        [Theory]
        [AutoData]
        public void SimpleFuncExportStrategy_GetDecoratorExpression_Throws(
            SimpleFuncExportStrategy<IBasicService> strategy,
            IInjectionScope scope, 
            IActivationExpressionRequest request)
        {
            Assert.Throws<NotSupportedException>(() => strategy.GetDecoratorActivationExpression(scope, request, null));
        }

        [Theory]
        [AutoData]
        public void SimpleFuncExportStrategy_AddSecondaryStrategy_Throws(
            SimpleFuncExportStrategy<IBasicService> strategy)
        {
            Assert.Throws<NotSupportedException>(() => strategy.AddSecondaryStrategy(null));
        }

        public delegate object ServiceFactory(Type serviceType);

        [Fact]
        public void SimpleFuncExportStrategy_GetStrategyActivationDelegate()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.ExportFunc<IBasicService>(scope => new BasicService());
                c.ExportFunc<ServiceFactory>(scope => type => scope.Locate(type));
            });

            var instance = container.Locate<IBasicService>();

            Assert.NotNull(instance);

            var factory = container.Locate<ServiceFactory>();

            var instance2 = factory(typeof(IBasicService));
        }

    }
}
