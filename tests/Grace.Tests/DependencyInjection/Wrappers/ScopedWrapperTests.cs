using System;
using System.Collections.Generic;
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

        [Fact]
        public void Scoped_Complex_Wrapper()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<CountValueMultipleService1>().As<ICountValueMultipleService>().WithMetadata("tag", "1");
                c.Export<CountValueMultipleService2>().As<ICountValueMultipleService>().WithMetadata("tag", "2");
                c.Export<CountValueMultipleService3>().As<ICountValueMultipleService>().WithMetadata("tag", "3");
            });

            var instanceList = container.Locate<IEnumerable<Meta<Scoped<Func<int, ICountValueMultipleService>>>>>();

            var i = 1;

            foreach (var meta in instanceList)
            {
                Assert.Equal(meta.Metadata["tag"], i.ToString());

                var func = meta.Value.Instance;

                var instance = func(i);

                Assert.Equal(instance.Count, i);

                i++;

                meta.Value.Dispose();
            }
        }

       

    }
}
