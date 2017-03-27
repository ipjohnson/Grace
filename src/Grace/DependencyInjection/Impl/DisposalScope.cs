using System;
using System.Collections.Generic;
using System.Threading;
using Grace.Data.Immutable;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// class for tracking and disposing objects
    /// </summary>
    public class DisposalScope : IDisposalScope
    {
        private struct DisposableEntry
        {
            public IDisposable DisposableItem;

            public Action DisposalAction;
        }

        private ImmutableLinkedList<DisposableEntry> _disposable =
            ImmutableLinkedList<DisposableEntry>.Empty;

        /// <summary>
        /// Dispose of scope
        /// </summary>
        public void Dispose()
        {
            var disposable = Interlocked.Exchange(ref _disposable, ImmutableLinkedList<DisposableEntry>.Empty);

            while (disposable.Count != 0)
            {
                disposable.Value.DisposalAction?.Invoke();
                disposable.Value.DisposableItem.Dispose();

                disposable = disposable.Next;
            }
        }

        /// <summary>
        /// Add an object for disposal tracking
        /// </summary>
        /// <param name="disposable">disposable object to track</param>
        /// <param name="cleanupDelegate">logic that will be run directly before the object is disposed</param>
        public T AddDisposable<T>(T disposable, Action<T> cleanupDelegate = null) where T : IDisposable
        {
            if (cleanupDelegate == null)
            {
                ImmutableLinkedList.ThreadSafeAdd(ref _disposable,
                    new DisposableEntry { DisposableItem = disposable });

                return disposable;
            }

            ImmutableLinkedList.ThreadSafeAdd(ref _disposable,
                new DisposableEntry { DisposableItem = disposable, DisposalAction = () => cleanupDelegate(disposable) });

            return disposable;
        }
    }
}

