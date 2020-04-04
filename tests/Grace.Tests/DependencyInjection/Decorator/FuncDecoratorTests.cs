using Grace.DependencyInjection;
using Grace.Tests.Classes.Attributes;
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


        [Fact]
        public void FuncDecorator_ExportFactory_When()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.ExportFactory(() => new BasicService { Count = 5 }).As<IBasicService>();
                c.ExportDecorator<IBasicService>(service =>
                {
                    service.Count += 5;
                    return service;
                }).When.ClassHas<TestAttribute>();
                c.ExportDecorator<IBasicService>(service =>
                {
                    service.Count += 15;
                    return service;
                }).When.ClassHas<SomeTestAttribute>();
            });

            var instance = container.Locate<DependencyOne>();

            Assert.NotNull(instance);
            Assert.Equal(10, instance.BasicService.Count);

            var instance2 = container.Locate<DependencyTwo>();

            Assert.NotNull(instance2);
            Assert.Equal(20, instance2.BasicService.Count);
        }

        [Test]
        public class DependencyOne
        {
            public DependencyOne(IBasicService basicService)
            {
                BasicService = basicService;
            }

            public IBasicService BasicService { get; }
        }

        [SomeTest]
        public class DependencyTwo
        {
            public DependencyTwo(IBasicService basicService)
            {
                BasicService = basicService;
            }

            public IBasicService BasicService { get; }
        }
    }
}
