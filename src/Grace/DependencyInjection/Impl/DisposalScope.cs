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
        private class DisposableEntry
        {
            public IDisposable DisposableItem;

            public Action DisposalAction;

            public DisposableEntry Next;

            public static readonly DisposableEntry Empty = new DisposableEntry();
        }

        //private ImmutableLinkedList<DisposableEntry> _disposable =
        //    ImmutableLinkedList<DisposableEntry>.Empty;
        private DisposableEntry _entry = DisposableEntry.Empty;
        
        /// <summary>
        /// Dispose of scope
        /// </summary>
        public virtual void Dispose()
        {
            var entry = Interlocked.Exchange(ref _entry, null);

            if (entry != null)
            {
                while (entry != DisposableEntry.Empty)
                {
                    entry.DisposalAction?.Invoke();
                    entry.DisposableItem.Dispose();

                    entry = entry.Next;
                }
            }
        }

        /// <summary>
        /// Add an object for disposal tracking
        /// </summary>
        /// <param name="disposable">disposable object to track</param>
        /// <param name="cleanupDelegate">logic that will be run directly before the object is disposed</param>
        public T AddDisposable<T>(T disposable, Action<T> cleanupDelegate = null) where T : IDisposable
        { 
            DisposableEntry entry;

            if (cleanupDelegate == null)
            {
                entry = new DisposableEntry {DisposableItem = disposable, Next = _entry};
            }
            else
            {
                entry = new DisposableEntry
                {
                    DisposableItem = disposable,
                    DisposalAction = () => cleanupDelegate(disposable),
                    Next = _entry
                };
            }

            var current = _entry;

            if (Interlocked.CompareExchange(ref _entry, entry, current) == current)
            {
                return disposable;
            }

            SwapWaitAdd(entry);

            return disposable;
        }

        private void SwapWaitAdd(DisposableEntry entry)
        {
            var spinWait = new SpinWait();

            spinWait.SpinOnce();

            var current = _entry;

            while (Interlocked.CompareExchange(ref _entry, entry, current) != current)
            {
                spinWait.SpinOnce();

                current = _entry;

                entry.Next = _entry;
            }
        }
    }
}

