using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Named
{
    public class NamedScopedTests
    {
        [Fact]
        public void Scoped_Export_AsName()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<BasicService>().AsName("BasicService"));

            using (var scope = container.BeginLifetimeScope())
            {
                var instance = scope.LocateByName("BasicService");

                Assert.NotNull(instance);
                Assert.IsType<BasicService>(instance);
            }
        }

        [Fact]
        public void Scoped_Export_Disposable_AsName()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<DisposableService>().AsName("DisposableService"));

            var disposed = false;

            using (var scope = container.BeginLifetimeScope())
            {
                var instance = (IDisposableService)scope.LocateByName("DisposableService");

                Assert.NotNull(instance);
                Assert.IsType<DisposableService>(instance);

                instance.Disposing += (sender, args) => disposed = true;
            }

            Assert.True(disposed);
        }

        [Fact]
        public void Scoped_Export_TypeSet_ByName()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.ExportAssemblyContaining<NamedTests>().ByName());

            using (var scope = container.BeginLifetimeScope())
            {
                var instance = scope.LocateByName("BasicService");

                Assert.NotNull(instance);
                Assert.IsType<BasicService>(instance);
            }
        }

        [Fact]
        public void Scoped_TryLocateByName_Find()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<BasicService>().AsName("BasicService"));

            object instance;

            using (var scope = container.BeginLifetimeScope())
            {
                var returnValue = scope.TryLocateByName("BasicService", out instance);

                Assert.True(returnValue);
                Assert.NotNull(instance);
                Assert.IsType<BasicService>(instance);
            }
        }

        [Fact]
        public void Scoped_TryLocateByName_Cant_Find()
        {
            var container = new DependencyInjectionContainer();

            object instance;

            using (var scope = container.BeginLifetimeScope())
            {
                var returnValue = scope.TryLocateByName("BasicService", out instance);

                Assert.False(returnValue);
                Assert.Null(instance);
            }
        }

        [Fact]
        public void Scoped_LocateAllByName()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<MultipleService1>().AsName("MultipleService");
                c.Export<MultipleService2>().AsName("MultipleService");
                c.Export<MultipleService3>().AsName("MultipleService");
                c.Export<MultipleService4>().AsName("MultipleService");
                c.Export<MultipleService5>().AsName("MultipleService");
            });

            using (var scope = container.BeginLifetimeScope())
            {
                var services = scope.LocateAllByName("MultipleService");

                Assert.Equal(5, services.Count);
                Assert.IsType<MultipleService1>(services[0]);
                Assert.IsType<MultipleService2>(services[1]);
                Assert.IsType<MultipleService3>(services[2]);
                Assert.IsType<MultipleService4>(services[3]);
                Assert.IsType<MultipleService5>(services[4]);
            }
        }
    }
}
