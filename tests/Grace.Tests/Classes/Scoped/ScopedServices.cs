using System.Threading;

namespace Grace.Tests.Classes.Scoped
{
    public interface IScopedService1
    {
        void DoSomething();
    }

    public class ScopedService1 : IScopedService1
    {
        private static int _counter;

        public ScopedService1()
        {
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

    public interface IScopedService2
    {
        void DoSomething();
    }

    public class ScopedService2 : IScopedService2
    {
        private static int _counter;

        public ScopedService2()
        {
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

    public interface IScopedService3
    {
        void DoSomething();
    }

    public class ScopedService3 : IScopedService3
    {
        private static int _counter;

        public ScopedService3()
        {
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

    public interface IScopedService4
    {
        void DoSomething();
    }

    public class ScopedService4 : IScopedService4
    {
        private static int _counter;

        public ScopedService4()
        {
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

    public interface IScopedService5
    {
        void DoSomething();
    }

    public class ScopedService5 : IScopedService5
    {
        private static int _counter;

        public ScopedService5()
        {
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
