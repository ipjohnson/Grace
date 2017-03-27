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
        private ImmutableLinkedList<Tuple<IDisposable, Action>> _disposable =
            ImmutableLinkedList<Tuple<IDisposable, Action>>.Empty;

        /// <summary>
        /// Dispose of scope
        /// </summary>
        public virtual void Dispose()
        {
            var disposable = Interlocked.Exchange(ref _disposable, null);

            if (disposable != null)
            {
                while (disposable.Count != 0)
                {
                    disposable.Value.Item2?.Invoke();
                    disposable.Value.Item1.Dispose();

                    disposable = disposable.Next;
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
            Action cleanupAction = null;

            if (cleanupDelegate != null)
            {
                cleanupAction = () => cleanupDelegate(disposable);
            }

            ImmutableLinkedList.ThreadSafeAdd(ref _disposable,
                new Tuple<IDisposable, Action>(disposable, cleanupAction));

            return disposable;
        }
    }
}

