using System;
using System.Threading;

namespace Grace.Tests.Classes.Scoped
{
    public interface IRepositoryTransient1
    {
        void DoSomething();
    }

    public interface IRepositoryTransient2
    {
        void DoSomething();
    }

    public interface IRepositoryTransient3
    {
        void DoSomething();
    }

    public interface IRepositoryTransient4
    {
        void DoSomething();
    }

    public interface IRepositoryTransient5
    {
        void DoSomething();
    }

    public class RepositoryTransient1 : IRepositoryTransient1
    {
        private static int _counter;

        public RepositoryTransient1(IScopedService1 scopedService1, IScopedService2 scopedService2, IScopedService3 scopedService3, IScopedService4 scopedService4, IScopedService5 scopedService5)
        {
            if (scopedService1 == null)
            {
                throw new ArgumentNullException(nameof(scopedService1));
            }

            if (scopedService2 == null)
            {
                throw new ArgumentNullException(nameof(scopedService2));
            }

            if (scopedService3 == null)
            {
                throw new ArgumentNullException(nameof(scopedService3));
            }

            if (scopedService4 == null)
            {
                throw new ArgumentNullException(nameof(scopedService4));
            }

            if (scopedService5 == null)
            {
                throw new ArgumentNullException(nameof(scopedService5));
            }

            Interlocked.Increment(ref _counter);
        }

        public static int Instances
        {
            get => _counter;
            set => _counter = value;
        }

        public void DoSomething()
        {
            // No content
        }
    }

    public class RepositoryTransient2 : IRepositoryTransient2
    {
        private static int _counter;

        public RepositoryTransient2( IScopedService1 scopedService1, IScopedService2 scopedService2, IScopedService3 scopedService3, IScopedService4 scopedService4, IScopedService5 scopedService5)
        {

            if (scopedService1 == null)
            {
                throw new ArgumentNullException(nameof(scopedService1));
            }

            if (scopedService2 == null)
            {
                throw new ArgumentNullException(nameof(scopedService2));
            }

            if (scopedService3 == null)
            {
                throw new ArgumentNullException(nameof(scopedService3));
            }

            if (scopedService4 == null)
            {
                throw new ArgumentNullException(nameof(scopedService4));
            }

            if (scopedService5 == null)
            {
                throw new ArgumentNullException(nameof(scopedService5));
            }

            Interlocked.Increment(ref _counter);
        }

        public static int Instances
        {
            get => _counter; 
            set => _counter = value;
        }

        public void DoSomething()
        {
            // No content
        }
    }

    public class RepositoryTransient3 : IRepositoryTransient3
    {
        private static int _counter;

        public RepositoryTransient3( IScopedService1 scopedService1, IScopedService2 scopedService2, IScopedService3 scopedService3, IScopedService4 scopedService4, IScopedService5 scopedService5)
        {
            if (scopedService1 == null)
            {
                throw new ArgumentNullException(nameof(scopedService1));
            }

            if (scopedService2 == null)
            {
                throw new ArgumentNullException(nameof(scopedService2));
            }

            if (scopedService3 == null)
            {
                throw new ArgumentNullException(nameof(scopedService3));
            }

            if (scopedService4 == null)
            {
                throw new ArgumentNullException(nameof(scopedService4));
            }

            if (scopedService5 == null)
            {
                throw new ArgumentNullException(nameof(scopedService5));
            }

            Interlocked.Increment(ref _counter);
        }

        public static int Instances
        {
            get => _counter; 
            set => _counter = value;
        }

        public void DoSomething()
        {
            // No content
        }
    }

    public class RepositoryTransient4 : IRepositoryTransient4
    {
        private static int _counter;

        public RepositoryTransient4( IScopedService1 scopedService1, IScopedService2 scopedService2, IScopedService3 scopedService3, IScopedService4 scopedService4, IScopedService5 scopedService5)
        {

            if (scopedService1 == null)
            {
                throw new ArgumentNullException(nameof(scopedService1));
            }

            if (scopedService2 == null)
            {
                throw new ArgumentNullException(nameof(scopedService2));
            }

            if (scopedService3 == null)
            {
                throw new ArgumentNullException(nameof(scopedService3));
            }

            if (scopedService4 == null)
            {
                throw new ArgumentNullException(nameof(scopedService4));
            }

            if (scopedService5 == null)
            {
                throw new ArgumentNullException(nameof(scopedService5));
            }

            Interlocked.Increment(ref _counter);
        }

        public static int Instances
        {
            get => _counter; 
            set => _counter = value;
        }

        public void DoSomething()
        {
            // No content
        }
    }

    public class RepositoryTransient5 : IRepositoryTransient5
    {
        private static int _counter;

        public RepositoryTransient5( IScopedService1 scopedService1, IScopedService2 scopedService2, IScopedService3 scopedService3, IScopedService4 scopedService4, IScopedService5 scopedService5)
        {
            if (scopedService1 == null)
            {
                throw new ArgumentNullException(nameof(scopedService1));
            }

            if (scopedService2 == null)
            {
                throw new ArgumentNullException(nameof(scopedService2));
            }

            if (scopedService3 == null)
            {
                throw new ArgumentNullException(nameof(scopedService3));
            }

            if (scopedService4 == null)
            {
                throw new ArgumentNullException(nameof(scopedService4));
            }

            if (scopedService5 == null)
            {
                throw new ArgumentNullException(nameof(scopedService5));
            }

            Interlocked.Increment(ref _counter);
        }

        public static int Instances
        {
            get => _counter; 
            set => _counter = value;
        }

        public void DoSomething()
        {
            // No content
        }
    }
}
