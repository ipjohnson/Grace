using System;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Lifestyle
{
    public class SingletonTests
    {
        [Fact]
        public void Singleton_Return_Same_Insance()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<BasicService>().As<IBasicService>().Lifestyle.Singleton());

            var instance1 = container.Locate<IBasicService>();
            var instance2 = container.Locate<IBasicService>();

            
        }

        [Fact]
        public void Singleton_Disposal()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<DisposableService>().As<IDisposableService>().Lifestyle.Singleton());

            var instance1 = container.Locate<IDisposableService>();
            var instance2 = container.Locate<IDisposableService>();

            Assert.Same(instance1, instance2);
        }


        [Fact]
        public void Singleton_Disposal_In_Lifetime()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<DisposableService>().As<IDisposableService>().Lifestyle.Singleton());

            var disposed = false;

            using (var scope = container.BeginLifetimeScope())
            {
                var service = scope.Locate<IDisposableService>();

                service.Disposing += (o, e) => disposed = true;
            }

            Assert.False(disposed);

            container.Dispose();

            Assert.True(disposed);
        }


        [Fact]
        public void Singleton_Import_Returns_Same_Instance()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>().Lifestyle.Singleton();
                c.Export(typeof(DependentService<>)).As(typeof(IDependentService<>));
            });

            var instance1 = container.Locate<IDependentService<IBasicService>>();
            var instance2 = container.Locate<IDependentService<IBasicService>>();

            Assert.NotNull(instance1);
            Assert.NotNull(instance2);

            Assert.NotSame(instance1, instance2);
            Assert.Same(instance1.Value, instance2.Value);
        }

        [Fact]
        public void Singleton_Import_ExportLocatorScope()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export(typeof(DependentService<>)).As(typeof(IDependentService<>)).Lifestyle.Singleton());

            using (var scope = container.BeginLifetimeScope())
            {
                var instance = container.Locate<IDependentService<IExportLocatorScope>>();

                Assert.NotNull(instance);
                Assert.Same(container, instance.Value);
            }
        }


    }
}
