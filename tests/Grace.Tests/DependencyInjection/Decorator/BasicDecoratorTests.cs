using System;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Decorator
{
    public class BasicDecoratorTests
    {
        [Fact]
        public void Decorate_BasicService_Returns_Correct_Instance()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>();
                c.ExportDecorator(typeof(BasicServiceDecorator)).As(typeof(IBasicService));
            });

            var instance = container.Locate<IBasicService>();

            Assert.NotNull(instance);
            Assert.IsType<BasicServiceDecorator>(instance);
        }

        [Fact]
        public void Decorator_BasicService_With_Second_Type()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>();
                c.ExportDecorator(typeof(BasicServiceDecorator)).As(typeof(IBasicService));
                c.ExportDecorator(typeof(SecondBasicServiceDecorator)).As(typeof(IBasicService));
            });

            var instance = container.Locate<IBasicService>();

            Assert.NotNull(instance);
            Assert.IsType<SecondBasicServiceDecorator>(instance);
        }

        public interface IDecoratorLifestyle
        {
            Guid InstanceGuid { get; }

            Guid DecoratorGuid { get; }
        }

        public class ImplementationDecoratorLifestyle : IDecoratorLifestyle
        {
            public Guid InstanceGuid { get; set; }

            public Guid DecoratorGuid { get; } = Guid.NewGuid();
        }

        public class DecoratorLifestyle : IDecoratorLifestyle
        {
            private readonly IDecoratorLifestyle _instance;

            public DecoratorLifestyle(IDecoratorLifestyle instance)
            {
                _instance = instance;
                DecoratorGuid = Guid.NewGuid();
            }

            public Guid InstanceGuid => _instance.InstanceGuid;

            public Guid DecoratorGuid { get; }
        }

        [Fact]
        public void Decorator_ApplyAfterLifestyle()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.ExportDecorator(typeof(DecoratorLifestyle)).As(typeof(IDecoratorLifestyle)).ApplyAfterLifestyle();
                c.Export<ImplementationDecoratorLifestyle>().As<IDecoratorLifestyle>().Lifestyle.Singleton();
            });

            var instance1 = container.Locate<IDecoratorLifestyle>();
            var instance2 = container.Locate<IDecoratorLifestyle>();

            Assert.Equal(instance1.InstanceGuid, instance2.InstanceGuid);
            Assert.NotEqual(instance1.DecoratorGuid, instance2.DecoratorGuid);
        }
    }
}
