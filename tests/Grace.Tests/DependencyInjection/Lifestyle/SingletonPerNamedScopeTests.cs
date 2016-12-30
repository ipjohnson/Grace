using Grace.DependencyInjection;
using Grace.DependencyInjection.Exceptions;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Lifestyle
{
    public class SingletonPerNamedScopeTests
    {
        [Fact]
        public void SingletonPerNamedScopeBasicTest()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<BasicService>().As<IBasicService>().Lifestyle.SingletonPerNamedScope("Test"));

            var childScope = container.BeginLifetimeScope("Test");

            var basicService = childScope.Locate<IBasicService>();

            Assert.NotNull(basicService);

            Assert.Same(basicService, childScope.Locate<IBasicService>());
        }

        [Fact]
        public void SingletonPerNamedScopeNestedSameTest()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<BasicService>().As<IBasicService>().Lifestyle.SingletonPerNamedScope("Test"));

            var parentScope = container.BeginLifetimeScope("Test");

            var childScope1 = parentScope.BeginLifetimeScope(scopeName: "Child1");

            var childScope2 = parentScope.BeginLifetimeScope(scopeName: "Child2");

            var basicService = parentScope.Locate<IBasicService>();

            Assert.NotNull(basicService);

            Assert.Same(basicService, childScope1.Locate<IBasicService>());

            Assert.Same(basicService, childScope2.Locate<IBasicService>());
        }

        [Fact]
        public void SingletonPerScopeNamedMissingScopeExceptionTest()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<BasicService>().As<IBasicService>().Lifestyle.SingletonPerNamedScope("Test"));
            
            Assert.Throws<NamedScopeLocateException>(() => container.Locate<IBasicService>());
        }

        [Fact]
        public void SingletonPerScopeNamedDifferentNamedScopes()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<BasicService>().As<IBasicService>().Lifestyle.SingletonPerNamedScope("Test"));

            var child1 = container.BeginLifetimeScope(scopeName: "Test");

            var baseService1 = child1.Locate<IBasicService>();

            Assert.NotNull(baseService1);

            Assert.Same(baseService1, child1.Locate<IBasicService>());

            var child2 = container.BeginLifetimeScope(scopeName: "Test");

            var baseService2 = child2.Locate<IBasicService>();

            Assert.NotNull(baseService2);

            Assert.Same(baseService2, child2.Locate<IBasicService>());

            Assert.NotSame(baseService1, baseService2);
        }

        [Fact]
        public void SingletonPerNamedScopeDisposal()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<DisposableService>().As<IDisposableService>().Lifestyle.SingletonPerNamedScope("Test"));

            var disposed = false;

            using (var childScope = container.BeginLifetimeScope(scopeName: "Test"))
            {
                childScope.Locate<IDisposableService>().Disposing += (sender, args) => disposed = true;
            }

            Assert.True(disposed);
        }

        [Fact]
        public void SingletonPerNamedScopeNestedDisposal()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<DisposableService>().As<IDisposableService>().Lifestyle.SingletonPerNamedScope("Test"));

            var disposed = false;

            using (var childScope = container.BeginLifetimeScope(scopeName: "Test"))
            {
                using (var greatChildScope = childScope.BeginLifetimeScope(scopeName: "Test2"))
                {
                    greatChildScope.Locate<IDisposableService>().Disposing += (sender, args) => disposed = true;

                    Assert.Same(childScope.Locate<IDisposableService>(), greatChildScope.Locate<IDisposableService>());
                }

                Assert.False(disposed);
            }

            Assert.True(disposed);
        }
    }
}
