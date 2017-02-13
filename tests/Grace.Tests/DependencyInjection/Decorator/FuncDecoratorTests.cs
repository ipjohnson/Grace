using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Decorator
{
    public class FuncDecoratorTests
    {
        [Fact]
        public void FuncDecorator_Initialize_Concrete()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>();
                c.ExportDecorator<BasicService>(service =>
                {
                    service.Count = 5;

                    return service;
                });
            });

            var instance = container.Locate<IBasicService>();

            Assert.NotNull(instance);
            Assert.Equal(5, instance.Count);
        }

        [Fact]
        public void FuncDecorator_Initialize_Interface()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>();
                c.ExportDecorator<IBasicService>(service =>
                {
                    service.Count = 5;

                    return service;
                });
            });

            var instance = container.Locate<IBasicService>();

            Assert.NotNull(instance);
            Assert.Equal(5, instance.Count);
        }

        [Fact]
        public void FuncDecorator_ApplyAfterLifestyle()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>().Lifestyle.Singleton();
                c.ExportDecorator<IBasicService>(service =>
                {
                    service.Count++;

                    return service;
                }, applyAfterLifestyle: true);
            });

            var instance = container.Locate<IBasicService>();

            Assert.NotNull(instance);
            Assert.Equal(1, instance.Count);

            var instance2 = container.Locate<IBasicService>();

            Assert.NotNull(instance2);
            Assert.Equal(2, instance2.Count);

            Assert.Same(instance, instance2);
        }

        [Fact]
        public void FuncDecorator_ApplyBeforeLifestyle()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>().Lifestyle.Singleton();
                c.ExportDecorator<IBasicService>(service =>
                {
                    service.Count++;

                    return service;
                }, applyAfterLifestyle: false);
            });

            var instance = container.Locate<IBasicService>();

            Assert.NotNull(instance);
            Assert.Equal(1, instance.Count);

            var instance2 = container.Locate<IBasicService>();

            Assert.NotNull(instance2);
            Assert.Equal(1, instance2.Count);

            Assert.Same(instance, instance2);
        }

        [Fact]
        public void FuncDecorator_ExportFactory()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.ExportFactory(() => new BasicService { Count = 5 }).As<IBasicService>();
                c.ExportDecorator<IBasicService>(service =>
                {
                    service.Count += 5;
                    return service;
                });
            });

            var instance = container.Locate<IBasicService>();

            Assert.NotNull(instance);
            Assert.Equal(10, instance.Count);
        }
    }
}
