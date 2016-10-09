using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Lifestyle
{
    public class SingletonPerNamedScopeTests
    {
        [Fact]
        public void SingletonPerNamedScopeBasicTest()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<BasicService>().As<IBasicService>().Lifestyle.SingletonPerNamedScope("Test"));

            var childScope = container.BeginLifetimeScope("Test");

            IBasicService basicService = childScope.Locate<IBasicService>();

            Assert.NotNull(basicService);

            Assert.Same(basicService, childScope.Locate<IBasicService>());
        }

        [Fact]
        public void SingletonPerNamedScopeNestedSameTest()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<BasicService>().As<IBasicService>().Lifestyle.SingletonPerNamedScope("Test"));

            var parentScope = container.BeginLifetimeScope("Test");

            var childScope1 = parentScope.BeginLifetimeScope(scopeName: "Child1");

            var childScope2 = parentScope.BeginLifetimeScope(scopeName: "Child2");

            IBasicService basicService = parentScope.Locate<IBasicService>();

            Assert.NotNull(basicService);

            Assert.Same(basicService, childScope1.Locate<IBasicService>());

            Assert.Same(basicService, childScope2.Locate<IBasicService>());
        }

        //[Fact]
        //public void SingletonPerScopeNamedMissingScopeExceptionTest()
        //{
        //    DependencyInjectionContainer container = new DependencyInjectionContainer();

        //    container.Configure(c => c.Export<BasicService>().As<IBasicService>().Lifestyle.SingletonPerNamedScope("Test"));

        //    try
        //    {
        //        container.Locate<IBasicService>();
        //    }
        //    catch (GeneralLocateException exp)
        //    {
        //        if (!(exp.InnerException is InjectionScopeCouldNotBeFoundException))
        //        {
        //            throw new Exception("Wrong inner exception should have been injection scope could not be found");
        //        }
        //    }
        //}

        [Fact]
        public void SingletonPerScopeNamedDifferentNamedScopes()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<BasicService>().As<IBasicService>().Lifestyle.SingletonPerNamedScope("Test"));

            var child1 = container.BeginLifetimeScope(scopeName: "Test");

            IBasicService baseService1 = child1.Locate<IBasicService>();

            Assert.NotNull(baseService1);

            Assert.Same(baseService1, child1.Locate<IBasicService>());

            var child2 = container.BeginLifetimeScope(scopeName: "Test");

            IBasicService baseService2 = child2.Locate<IBasicService>();

            Assert.NotNull(baseService2);

            Assert.Same(baseService2, child2.Locate<IBasicService>());

            Assert.NotSame(baseService1, baseService2);
        }

        [Fact]
        public void SingletonPerNamedScopeDisposal()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<DisposableService>().As<IDisposableService>().Lifestyle.SingletonPerNamedScope("Test"));

            bool disposed = false;

            using (var childScope = container.BeginLifetimeScope(scopeName: "Test"))
            {
                childScope.Locate<IDisposableService>().Disposing += (sender, args) => disposed = true;
            }

            Assert.True(disposed);
        }

        [Fact]
        public void SingletonPerNamedScopeNestedDisposal()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<DisposableService>().As<IDisposableService>().Lifestyle.SingletonPerNamedScope("Test"));

            bool disposed = false;

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
