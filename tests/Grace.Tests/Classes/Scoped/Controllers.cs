using System;
using System.Threading;

namespace Grace.Tests.Classes.Scoped
{
    public class TestController1 : IDisposable
    {
        private static int _counter;

        private static int _disposeCount;

        public TestController1(
            IRepositoryTransient1 transient1,
            IRepositoryTransient2 repositoryTransient2,
            IRepositoryTransient3 repositoryTransient3,
            IRepositoryTransient4 repositoryTransient4,
            IRepositoryTransient5 repositoryTransient5)
        {
            if (transient1 == null)
            {
                throw new ArgumentNullException(nameof(transient1));
            }

            if (repositoryTransient2 == null)
            {
                throw new ArgumentNullException(nameof(repositoryTransient2));
            }

            if (repositoryTransient3 == null)
            {
                throw new ArgumentNullException(nameof(repositoryTransient3));
            }

            if (repositoryTransient4 == null)
            {
                throw new ArgumentNullException(nameof(repositoryTransient4));
            }

            if (repositoryTransient5 == null)
            {
                throw new ArgumentNullException(nameof(repositoryTransient5));
            }

            Interlocked.Increment(ref _counter);
        }

        public static int Instances
        {
            get => _counter;
            set => _counter = value;
        }

        public static int DisposeCount
        {
            get => _disposeCount;
            set => _disposeCount = value;
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            Interlocked.Increment(ref _disposeCount);
        }
    }
}
