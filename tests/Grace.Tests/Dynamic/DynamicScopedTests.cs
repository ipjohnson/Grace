using System;
using Grace.DependencyInjection;
using Grace.Dynamic;
using Grace.Tests.Classes.Scoped;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.Dynamic
{
    public class DynamicScopedTests
    {
        public class DisposableDependent : IDisposable
        {
            public DisposableDependent(IBasicService basicService)
            {
                BasicService = basicService;
            }

            public IBasicService BasicService { get; }

            /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
            public void Dispose()
            {
                
            }
        }

        [Fact]
        public void Scoped_DisposalTest()
        {
            var container = new DependencyInjectionContainer(GraceDynamicMethod.Configuration(c =>
            {
                c.Trace = s => Assert.DoesNotContain("falling back", s);
            }));

            container.Configure(c => c.Export<BasicService>().As<IBasicService>().Lifestyle.SingletonPerScope());

            var value = container.Locate<DisposableDependent>();

            using (var scope = container.BeginLifetimeScope())
            {
                value = scope.Locate<DisposableDependent>();
            }
        }
        
        [Fact]
        public void DynamicMethod_Per_Scope()
        {
            var container = new DependencyInjectionContainer(GraceDynamicMethod.Configuration(c =>
            {
                c.Trace = s => Assert.DoesNotContain("falling back", s);
            }));

            container.Configure(c => c.Export<BasicService>().As<IBasicService>().Lifestyle.SingletonPerScope());

            var value = container.Locate<DisposableDependent>();

            using (var scope = container.BeginLifetimeScope())
            {
                value = scope.Locate<DisposableDependent>();
            }
        }

        [Fact]
        public void DynamicMethod_AspNet()
        {
            var container = new DependencyInjectionContainer(GraceDynamicMethod.Configuration(c =>
            {
                c.Trace = s => Assert.DoesNotContain("falling back", s);
            }));

            container.Configure(c =>
            {
                c.Export<TestController1>();
                c.Export<RepositoryTransient1>().As<IRepositoryTransient1>();
                c.Export<RepositoryTransient2>().As<IRepositoryTransient2>();
                c.Export<RepositoryTransient3>().As<IRepositoryTransient3>();
                c.Export<RepositoryTransient4>().As<IRepositoryTransient4>();
                c.Export<RepositoryTransient5>().As<IRepositoryTransient5>();
                c.Export<ScopedService1>().As<IScopedService1>().Lifestyle.SingletonPerScope();
                c.Export<ScopedService2>().As<IScopedService2>().Lifestyle.SingletonPerScope();
                c.Export<ScopedService3>().As<IScopedService3>().Lifestyle.SingletonPerScope();
                c.Export<ScopedService4>().As<IScopedService4>().Lifestyle.SingletonPerScope();
                c.Export<ScopedService5>().As<IScopedService5>().Lifestyle.SingletonPerScope();
            });

            var controller = container.Locate<TestController1>();

        }

    }
}
