using System;
using System.Threading;

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
        private class DisposeEntry
        {
            /// <summary>
            /// Item to be disposed
            /// </summary>
            public IDisposable DisposeItem;

            /// <summary>
            /// Cleanup delegate that was passed in, this is a wrapper around the original delegate that was passed in
            /// </summary>
            public Action CleanupDelegate;

            /// <summary>
            /// Next entry to dispose
            /// </summary>
            public DisposeEntry Next;

            /// <summary>
            /// Empty entry
            /// </summary>
            public static readonly DisposeEntry Empty = new DisposeEntry();
        }
        
        /// <summary>
        /// Internal list of disposal entries
        /// </summary>
        private DisposeEntry _entry = DisposeEntry.Empty;

        /// <summary>
        /// Dispose of scope
        /// </summary>
        public virtual void Dispose()
        {
            var entry = Interlocked.Exchange(ref _entry, DisposeEntry.Empty);
            
            while (!ReferenceEquals(entry, DisposeEntry.Empty))
            {
                entry.CleanupDelegate?.Invoke();
                entry.DisposeItem?.Dispose();

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
            var entry = new DisposeEntry { DisposeItem = disposable, Next = current };

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
            DisposeEntry entry;

            var current = _entry;

            if (cleanupDelegate == null)
            {
                entry = new DisposeEntry { DisposeItem = disposable, Next = current };
            }
            else
            {
                entry = new DisposeEntry
                {
                    DisposeItem = disposable,
                    CleanupDelegate = () => cleanupDelegate(disposable),
                    Next = current
                };
            }
            
            if (ReferenceEquals(Interlocked.CompareExchange(ref _entry, entry, current), current))
            {
                return disposable;
            }

            SwapWaitAdd(entry);

            return disposable;
        }

        private void SwapWaitAdd(DisposeEntry entry)
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

