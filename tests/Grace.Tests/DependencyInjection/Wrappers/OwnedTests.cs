using System;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Wrappers
{

    public class OwnedTests
    {
        [Fact]
        public void OwnedLocate_ScopesChild()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<DisposableService>().As<IDisposableService>();
            });

            var disposed = false;

            using (var owned = container.Locate<Owned<IDisposableService>>())
            {
                Assert.NotNull(owned);
                Assert.NotNull(owned.Value);

                owned.Value.Disposing += (sender, args) => disposed = true;
            }

            Assert.True(disposed);
        }

        [Fact]
        public void OwnedFuncLocate_ScopesDisposalCorrectly()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<DisposableService>().As<IDisposableService>();
            });

            var disposalCount = 0;

            using (var ownedFunc = container.Locate<Owned<Func<IDisposableService>>>())
            {
                var instanceOne = ownedFunc.Value();
                instanceOne.Disposing += (sender, args) => disposalCount++;

                var instanceTwo = ownedFunc.Value();
                instanceTwo.Disposing += (sender, args) => disposalCount++;
            }

            Assert.Equal(2, disposalCount);
        }

        [Fact]
        public void FuncOwnedLocate_ScopesDisposalCorrectly()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<DisposableService>().As<IDisposableService>();
            });

            var ownedFunc = container.Locate<Func<Owned<IDisposableService>>>();

            var disposed = false;

            using (var owned = ownedFunc())
            {
                Assert.NotNull(owned);
                Assert.NotNull(owned.Value);

                owned.Value.Disposing += (sender, args) => disposed = true;
            }

            Assert.True(disposed);

            disposed = false;

            using (var owned = ownedFunc())
            {
                Assert.NotNull(owned);
                Assert.NotNull(owned.Value);

                owned.Value.Disposing += (sender, args) => disposed = true;
            }

            Assert.True(disposed);
        }
    }
}
