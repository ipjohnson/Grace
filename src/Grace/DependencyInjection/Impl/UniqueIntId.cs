using System.Threading;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// static class for generating unique int id within this process
    /// </summary>
    public class UniqueIntId
    {
        private static int _currentUniqueId;

        /// <summary>
        /// Create new unique int id
        /// </summary>
        /// <returns></returns>
        public static int New()
        {
            return Interlocked.Increment(ref _currentUniqueId);
        }
    }
}
