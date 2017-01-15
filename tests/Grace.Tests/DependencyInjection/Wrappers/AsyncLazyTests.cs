using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Xunit;

namespace Grace.Tests.DependencyInjection.Wrappers
{
    public class AsyncLazy<T>
    {
        private readonly Lazy<T> _lazy;
        private TaskCompletionSource<T> _completionSource;

        public AsyncLazy(Lazy<T> lazy)
        {
            _lazy = lazy;
        }

        public Task<T> Value
        {
            get
            {
                if (_completionSource == null && 
                    Interlocked.CompareExchange(ref _completionSource, new TaskCompletionSource<T>(), null) == null)
                {
                    Task.Run(() =>
                    {
                        try
                        {
                            _completionSource.SetResult(_lazy.Value);
                        }
                        catch (Exception exp)
                        {
                            _completionSource.SetException(exp);
                        }
                    });
                }

                return _completionSource.Task;
            }
        }

        public bool IsFinished => _completionSource != null &&
                                   (_completionSource.Task.IsCompleted || _completionSource.Task.IsCompleted);

        public bool IsCreated => _completionSource != null && _completionSource.Task.IsCompleted;

        public bool IsStarted => _completionSource != null;
    }

    public class AsyncLazyTests
    {
        [Fact]
        public void AsyncLazy_Resolve()
        {
            var task = ResolveAsync();

            task.Wait(10000);
        }

        public class DelayedResolveClass
        {
            public DelayedResolveClass()
            {
                Thread.Sleep(1000);
            }
        }

        private async Task ResolveAsync()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<DelayedResolveClass>();
                c.ExportWrapper(typeof(AsyncLazy<>));
            });

            var lazy = container.Locate<AsyncLazy<DelayedResolveClass>>();

            var startTime = DateTime.Now;

            var value = await lazy.Value;

            var endTime = DateTime.Now;

            Assert.NotNull(value);
            Assert.True((endTime - startTime).TotalSeconds > 1);
        }
    }
}
