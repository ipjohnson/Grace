using Grace.DependencyInjection;
using Grace.DependencyInjection.Conditions;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Decorator
{
    public class BasicDecoratorWithConditionalDependenciesTests
    {
        [Fact]
        public void Decorate_BasicService_When_Injected_Into_BasicServiceDecorator()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>()
                    .As<IBasicService>()
                    .When.InjectedInto<BasicServiceDecorator>();

                c.ExportDecorator(typeof(BasicServiceDecorator)).As(typeof(IBasicService));
            });

            var instance = container.Locate<IBasicService>();

            Assert.NotNull(instance);
            Assert.IsType<BasicServiceDecorator>(instance);
        }

        [Fact]
        public void Decorate_BasicService_When_Meets_Activation_Type_Is_BasicService_Condition()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>()
                    .As<IBasicService>()
                    .When.MeetsCondition(new WhenActivationTypeIs<BasicService>());

                c.ExportDecorator(typeof(BasicServiceDecorator)).As(typeof(IBasicService));
            });

            var instance = container.Locate<IBasicService>();

            Assert.NotNull(instance);
            Assert.IsType<BasicServiceDecorator>(instance);
        }
    }

    public class WhenActivationTypeIs<T> : ICompiledCondition where T : class
    {
        /// <summary>
        /// Test if strategy meets condition at configuration time
        /// </summary>
        /// <param name="strategy">strategy to test</param>
        /// <param name="staticInjectionContext">static injection context</param>
        /// <returns>meets condition</returns>
        public bool MeetsCondition(IActivationStrategy strategy, StaticInjectionContext staticInjectionContext)
        {
            return typeof(T).IsAssignableFrom(strategy.ActivationType);
        }
    }
}
