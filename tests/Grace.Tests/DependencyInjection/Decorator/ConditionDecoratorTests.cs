using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grace.DependencyInjection;
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
    }
}
