using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl;
using Xunit;

namespace Grace.Tests.DependencyInjection.DisposableTests
{
    public class DisposableThreadingTest
    {
        public class DisposableItem : IDisposable
        {
            public static int DisposeCount;
            public static int CreateCount;

            public DisposableItem()
            {
                Interlocked.Increment(ref CreateCount);
            }

            public void Dispose()
            {
                DisposeCount++;
            }
        }
        
        [Fact]
        public void DisposableScope_Threading_Test()
        {
            DisposableItem.CreateCount = 0;
            DisposableItem.DisposeCount = 0;

            var scope = new DisposalScope();

            var syncEvent = new ManualResetEvent(false);

            var task1 = Task.Run(() => AddToScope(scope, syncEvent));
            var task2 = Task.Run(() => AddToScope(scope, syncEvent));

            syncEvent.Set();

            Task.WaitAll(task1, task2);

            scope.Dispose();
            
            Assert.Equal(DisposableItem.CreateCount, DisposableItem.DisposeCount);
        }

        [Fact]
        public void DisposableScope_WithCleanup_Threading_Test()
        {
            DisposableItem.CreateCount = 0;
            DisposableItem.DisposeCount = 0;

            var scope = new DisposalScope();

            var syncEvent = new ManualResetEvent(false);

            var task1 = Task.Run(() => AddToScopeWithCleanup(scope, syncEvent, item => { }));
            var task2 = Task.Run(() => AddToScopeWithCleanup(scope, syncEvent, item => { }));

            syncEvent.Set();

            Task.WaitAll(task1, task2);

            scope.Dispose();

            Assert.Equal(DisposableItem.CreateCount, DisposableItem.DisposeCount);
        }
        
        [Fact]
        public void DisposableScope_WithNullCleanup_Threading_Test()
        {
            DisposableItem.CreateCount = 0;
            DisposableItem.DisposeCount = 0;

            var scope = new DisposalScope();

            var syncEvent = new ManualResetEvent(false);

            var task1 = Task.Run(() => AddToScopeWithCleanup(scope, syncEvent, null));
            var task2 = Task.Run(() => AddToScopeWithCleanup(scope, syncEvent, null));

            syncEvent.Set();

            Task.WaitAll(task1, task2);

            scope.Dispose();

            Assert.Equal(DisposableItem.CreateCount, DisposableItem.DisposeCount);
        }

        private void AddToScope(IDisposalScope scope, ManualResetEvent syncEvent)
        {
            syncEvent.WaitOne();

            for (int i = 0; i < 10000; i++)
            {
                scope.AddDisposable(new DisposableItem());
            }
        }


        private void AddToScopeWithCleanup(IDisposalScope scope, ManualResetEvent syncEvent, Action<DisposableItem> action)
        {
            syncEvent.WaitOne();

            for (int i = 0; i < 10000; i++)
            {
                scope.AddDisposable(new DisposableItem(), action);
            }
        }
    }
}
