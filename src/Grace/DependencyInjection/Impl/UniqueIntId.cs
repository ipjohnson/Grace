using System.Threading;

namespace Grace.DependencyInjection.Impl
{
    public class UniqueIntId
    {
        private static int _currentUniqueId;

        public static int New()
        {
            return Interlocked.Increment(ref _currentUniqueId);
        }
    }
}
