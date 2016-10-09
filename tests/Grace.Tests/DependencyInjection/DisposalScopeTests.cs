using System;
using Grace.DependencyInjection.Impl;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection
{
    public class DisposalScopeTests
    {
        [Fact]
        public void DisposalScope_DisposeCalledTest()
        {
            DisposalScope disposalScope = new DisposalScope();
            var disposableService = new DisposableService();
            var eventFired = false;

            disposableService.Disposing += (sender, args) => eventFired = true;

            disposalScope.AddDisposable(disposableService);

            disposalScope.Dispose();

            Assert.True(eventFired);
        }

        [Fact]
        public void DisposalScope_ThrowExceptionDuringCleanUp()
        {
            DisposalScope disposalScope = new DisposalScope();
            var disposableService = new DisposableService();
            var eventFired = false;

            disposableService.Disposing += (sender, args) => eventFired = true;

            disposalScope.AddDisposable(disposableService, disposed => { throw new Exception(); });

            Assert.ThrowsAny<AggregateException>(() => disposalScope.Dispose());
        }

        [Fact]
        public void DisposalScope_ThrowExceptionDuringDispose()
        {
            DisposalScope disposalScope = new DisposalScope();
            var disposableService = new DisposableService();
            var eventFired = false;

            disposableService.Disposing += (sender, args) =>
            {
                eventFired = true;
                throw new Exception();
            };

            disposalScope.AddDisposable(disposableService, disposed => { throw new Exception(); });

            Assert.Throws<AggregateException>(() => disposalScope.Dispose());
        }

        [Fact]
        public void DoubleDisposeTest()
        {
            DisposalScope disposalScope = new DisposalScope();
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
            DisposalScope disposalScope = new DisposalScope();
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
