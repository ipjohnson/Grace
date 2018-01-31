using System;
using System.Collections.Generic;
using System.Reflection;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Conditions;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Decorator
{
    public class ConditionDecoratorTests
    {
        [Fact]
        public void DecoratorCondition_InjectedType()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>();
                c.ExportDecorator(typeof(BasicServiceDecorator))
                    .As(typeof(IBasicService))
                    .When.InjectedInto<DependentService<IBasicService>>();
            });

            var instance = container.Locate<IBasicService>();

            Assert.NotNull(instance);
            Assert.IsType<BasicService>(instance);

            var dependentService = container.Locate<DependentService<IBasicService>>();

            Assert.NotNull(dependentService);
            Assert.IsType<BasicServiceDecorator>(dependentService.Value);
        }

        [Fact]
        public void DecoratorCondition_Decoratoring()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<CommandA>().As<ICommand<int>>();
                c.Export<OtherCommand>().As<ICommand<int>>();
                c.ExportDecorator(typeof(ValidatingCommand<>)).As(typeof(ICommand<>)).When
                    .MeetsCondition(new WhenDecoratingCondition(typeof(BaseCommand<>)));
                c.ExportDecorator(typeof(LoggingComand<>)).As(typeof(ICommand<>)).When
                    .MeetsCondition(new WhenDecoratingCondition(typeof(BaseCommand<>)));
            });

            var instances = container.Locate<List<ICommand<int>>>();

            Assert.Equal(2, instances.Count);
            Assert.IsType<LoggingComand<int>>(instances[0]);
            Assert.IsType<OtherCommand>(instances[1]);
        }
    }

    public class WhenDecoratingCondition : ICompiledCondition
    {
        private Type _decoratedType;

        public WhenDecoratingCondition(Type decoratedType)
        {
            _decoratedType = decoratedType;
        }

        /// <summary>
        /// Test if strategy meets condition at configuration time
        /// </summary>
        /// <param name="strategy">strategy to test</param>
        /// <param name="staticInjectionContext">static injection context</param>
        /// <returns>meets condition</returns>
        public bool MeetsCondition(IActivationStrategy strategy, StaticInjectionContext staticInjectionContext)
        {
            if (_decoratedType.GetTypeInfo().IsGenericTypeDefinition)
            {
                var current = strategy.ActivationType;

                while (current != null && current != typeof(object))
                {
                    if (current.IsConstructedGenericType && current.GetGenericTypeDefinition() == _decoratedType)
                    {
                        return true;
                    }

                    current = current.GetTypeInfo().BaseType;
                }

                return false;
            }

            return strategy.ActivationType.IsAssignableFrom(_decoratedType);
        }
    }
}
