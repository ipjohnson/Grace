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
        /// <summary>
        /// For memory allocation and execution performance I've written a one off linked list to track items for disposal
        /// </summary>
        private class DisposableEntry
        {
            public IDisposable DisposableItem;

            public Action DisposalAction;

            public DisposableEntry Next;

            public static readonly DisposableEntry Empty = new DisposableEntry();
        }
        
        private DisposableEntry _entry = DisposableEntry.Empty;

        /// <summary>
        /// Dispose of scope
        /// </summary>
        public virtual void Dispose()
        {
            var entry = Interlocked.Exchange(ref _entry, DisposableEntry.Empty);
            
            while (!ReferenceEquals(entry, DisposableEntry.Empty))
            {
                entry.DisposalAction?.Invoke();
                entry.DisposableItem?.Dispose();

                entry = entry.Next;
            }
        }

        /// <summary>
        /// Add an object for disposal tracking
        /// </summary>
        /// <param name="disposable">object to track for disposal</param>
        public T AddDisposable<T>(T disposable) where T : IDisposable
        {
            var current = _entry;
            var entry = new DisposableEntry { DisposableItem = disposable, Next = current };

            if (ReferenceEquals(Interlocked.CompareExchange(ref _entry, entry, current), current))
            {
                return disposable;
            }

            SwapWaitAdd(entry);

            return disposable;
        }

        /// <summary>
        /// Add an object for disposal tracking
        /// </summary>
        /// <param name="disposable">disposable object to track</param>
        /// <param name="cleanupDelegate">logic that will be run directly before the object is disposed</param>
        public T AddDisposable<T>(T disposable, Action<T> cleanupDelegate) where T : IDisposable
        {
            DisposableEntry entry;

            if (cleanupDelegate == null)
            {
                entry = new DisposableEntry { DisposableItem = disposable, Next = _entry };
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

            if (ReferenceEquals(Interlocked.CompareExchange(ref _entry, entry, current), current))
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

            var current = entry.Next = _entry;

            while (!ReferenceEquals(Interlocked.CompareExchange(ref _entry, entry, current), current))
            {
                spinWait.SpinOnce();

                current = entry.Next = _entry;
            }
        }
    }
}

