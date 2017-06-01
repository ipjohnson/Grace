using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Decorator
{
    public class GenericDecoratorTests
    {
        public class DecoratorDependentService<T> : IDependentService<T>
        {
            private readonly IDependentService<T> _service;

            public DecoratorDependentService(IDependentService<T> service)
            {
                _service = service;
            }

            public T Value => _service.Value;
        }

        [Fact]
        public void Generic_Decorator_Export_And_Locate()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>();
                c.Export(typeof(DependentService<>)).As(typeof(IDependentService<>));
                c.ExportDecorator(typeof(DecoratorDependentService<>)).As(typeof(IDependentService<>));
            });

            var instance = container.Locate<IDependentService<IBasicService>>();

            Assert.NotNull(instance);
            Assert.IsType<DecoratorDependentService<IBasicService>>(instance);
        }
    }
}
