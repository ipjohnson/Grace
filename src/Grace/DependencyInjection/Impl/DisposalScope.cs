using System;
using Grace.Data.Immutable;

namespace Grace.DependencyInjection.Impl
{
    public class DisposalScope : IDisposalScope
    {
        protected bool CatchDisposalException;

        private ImmutableLinkedList<Tuple<IDisposable, Action>> _disposable =
            ImmutableLinkedList<Tuple<IDisposable, Action>>.Empty;

        public void Dispose()
        {
            var disposables = ImmutableLinkedList.ThreadSafeEmpty(ref _disposable);

            if (disposables == ImmutableLinkedList<Tuple<IDisposable, Action>>.Empty)
            {
                return;
            }

            var exceptions = ImmutableLinkedList<Exception>.Empty;

            foreach (var disposable in disposables)
            {
                try
                {
                    disposable.Item2?.Invoke();

                    disposable.Item1.Dispose();

                }
                catch (Exception exp)
                {
                    if (!CatchDisposalException)
                    {
                        exceptions = exceptions.Add(exp);
                    }
                }
            }

            if (!ReferenceEquals(exceptions,ImmutableLinkedList<Exception>.Empty))
            {
                throw new AggregateException("Exceptions thrown while disposing scope", exceptions.Reverse());
            }
        }

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

