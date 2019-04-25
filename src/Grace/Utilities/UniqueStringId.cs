using System.Threading;

namespace Grace.Utilities
{
    /// <summary>
    /// Static class for generating unique id within the process space
    /// </summary>
    public static class UniqueStringId
    {
        private static int _counter;

        /// <summary>
        /// Prefix for all string id's
        /// </summary>
        public const string Prefix = "^-";

        /// <summary>
        /// Generate string that is unique to this process
        /// </summary>
        /// <returns></returns>
        public static string Generate()
        {
            return Prefix + Interlocked.Increment(ref _counter);
        }
    }
}
