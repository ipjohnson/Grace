using System;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Exceptions;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Wrappers
{
    public class ScopedWrapperTests
    {
        [Fact]
        public void Scoped_Disposed()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<DisposableService>().As<IDisposableService>());

            var func = container.Locate<Func<Scoped<IDisposableService>>>();

            var disposed = false;

            using (var scoped = func())
            {
                Assert.NotNull(scoped);
                Assert.NotNull(scoped.Instance);

                scoped.Instance.Disposing += (sender, args) => disposed = true;

                Assert.False(disposed);
            }

            Assert.True(disposed);
        }

        [Fact]
        public void Scoped_Dependent_MissingType()
        {
            var container = new DependencyInjectionContainer();

            var funcService = container.Locate<DependentService<Func<Scoped<DisposableService>>>>();

            var disposed = false;

            using (var scoped = funcService.Value())
            {
                Assert.NotNull(scoped);
                Assert.NotNull(scoped.Instance);

                scoped.Instance.Disposing += (sender, args) => disposed = true;

                Assert.False(disposed);
            }

            Assert.True(disposed);
        }

        [Fact]
        public void Scoped_Named()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<BasicService>().As<IBasicService>().Lifestyle.SingletonPerNamedScope("TestScope"));

            var func = container.Locate<Func<string, Scoped<IBasicService>>>();

            using (var scoped = func("TestScope"))
            {
                Assert.NotNull(scoped);
                Assert.NotNull(scoped.Instance);
            }

            Assert.Throws<NamedScopeLocateException>(() =>
            {
                var scoped = func("SomeScope");

                var instance = scoped.Instance;
            });
        }
    }
}
