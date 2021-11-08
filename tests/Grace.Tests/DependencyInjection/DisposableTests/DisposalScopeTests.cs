using System.Threading.Tasks;
using Grace.DependencyInjection.Impl;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.DisposableTests
{
    public class DisposalScopeTests
    {

#if NET5_0

        [Fact]
        public async Task DisposalScope_DisposeAsyncMixedResourcesTest()
        {
            var disposalScope = new DisposalScope();
            var asyncDisposableService = new AsyncDisposableService();
            var disposableService = new DisposableService();
            var disposeEventFired = false;
            var asyncDisposeEventFired = false;

            disposableService.Disposing += (sender, args) => disposeEventFired = true;
            asyncDisposableService.Disposing += (sender, args) => asyncDisposeEventFired = true;

            disposalScope.AddAsyncDisposable(asyncDisposableService);
            disposalScope.AddDisposable(disposableService);

            await disposalScope.DisposeAsync();

            Assert.True(disposeEventFired && asyncDisposeEventFired);
        }

        [Fact]
        public void DisposalScope_DisposeMixedResourcesTest()
        {
            var disposalScope = new DisposalScope();
            var asyncDisposableService = new AsyncDisposableService();
            var disposableService = new DisposableService();
            var disposeEventFired = false;
            var asyncDisposeEventFired = false;

            disposableService.Disposing += (sender, args) => disposeEventFired = true;
            asyncDisposableService.Disposing += (sender, args) => asyncDisposeEventFired = true;

            disposalScope.AddAsyncDisposable(asyncDisposableService);
            disposalScope.AddDisposable(disposableService);

            disposalScope.Dispose();

            Assert.True(disposeEventFired && !asyncDisposeEventFired);
        }

        [Fact]
        public async Task DisposalScope_DisposeAsyncCalledTest()
        {
            var disposalScope = new DisposalScope();
            var disposableService = new AsyncDisposableService();
            var eventFired = false;

            disposableService.Disposing += (sender, args) => eventFired = true;

            disposalScope.AddAsyncDisposable(disposableService);

            await disposalScope.DisposeAsync();

            Assert.True(eventFired);
        }

        [Fact]
        public async Task DoubleAsyncDisposeTest()
        {
            var disposalScope = new DisposalScope();
            var disposableService = new AsyncDisposableService();
            var eventFired = false;

            disposableService.Disposing += (sender, args) => eventFired = true;

            disposalScope.AddAsyncDisposable(disposableService);

            await disposalScope.DisposeAsync();

            Assert.True(eventFired);

            eventFired = false;

            await disposalScope.DisposeAsync();

            Assert.False(eventFired);
        }

        [Fact]
        public async Task CleanUpDelegateAsyncTest()
        {
            var disposalScope = new DisposalScope();
            var disposableService = new AsyncDisposableService();
            var eventFired = false;
            var cleanUpCalled = false;

            disposableService.Disposing += (sender, args) => eventFired = true;

            disposalScope.AddAsyncDisposable(disposableService,
                disposed =>
                {
                    Assert.True(ReferenceEquals(disposableService, disposed));

                    cleanUpCalled = true;
                });

            await disposalScope.DisposeAsync();

            Assert.True(eventFired);
            Assert.True(cleanUpCalled);
        }
#endif

        [Fact]
        public void DisposalScope_DisposeCalledTest()
        {
            var disposalScope = new DisposalScope();
            var disposableService = new DisposableService();
            var eventFired = false;

            disposableService.Disposing += (sender, args) => eventFired = true;

            disposalScope.AddDisposable(disposableService);

            disposalScope.Dispose();

            Assert.True(eventFired);
        }

        //[Fact]
        //public void DisposalScope_ThrowExceptionDuringCleanUp()
        //{
        //    var disposalScope = new DisposalScope();
        //    var disposableService = new DisposableService();
        //    var eventFired = false;

        //    disposableService.Disposing += (sender, args) => eventFired = true;

        //    disposalScope.AddDisposable(disposableService, disposed => { throw new Exception(); });

        //    Assert.ThrowsAny<AggregateException>(() => disposalScope.Dispose());
        //}

        //[Fact]
        //public void DisposalScope_ThrowExceptionDuringDispose()
        //{
        //    var disposalScope = new DisposalScope();
        //    var disposableService = new DisposableService();
        //    var eventFired = false;

        //    disposableService.Disposing += (sender, args) =>
        //    {
        //        eventFired = true;
        //        throw new Exception();
        //    };

        //    disposalScope.AddDisposable(disposableService, disposed => { throw new Exception(); });

        //    Assert.Throws<AggregateException>(() => disposalScope.Dispose());
        //}

        [Fact]
        public void DoubleDisposeTest()
        {
            var disposalScope = new DisposalScope();
            var disposableService = new DisposableService();
            var eventFired = false;

            disposableService.Disposing += (sender, args) => eventFired = true;

            disposalScope.AddDisposable(disposableService);

            disposalScope.Dispose();

            Assert.True(eventFired);

            eventFired = false;

            disposalScope.Dispose();

            Assert.False(eventFired);
        }

        [Fact]
        public void CleanUpDelegateTest()
        {
            var disposalScope = new DisposalScope();
            var disposableService = new DisposableService();
            var eventFired = false;
            var cleanUpCalled = false;

            disposableService.Disposing += (sender, args) => eventFired = true;

            disposalScope.AddDisposable(disposableService,
                disposed =>
                {
                    Assert.True(ReferenceEquals(disposableService, disposed));

                    cleanUpCalled = true;
                });

            disposalScope.Dispose();

            Assert.True(eventFired);
            Assert.True(cleanUpCalled);
        }
    }
}
