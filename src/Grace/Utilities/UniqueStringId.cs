using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Grace.Utilities
{
    /// <summary>
    /// Static class for generating unique id within the process space
    /// </summary>
    public static class UniqueStringId
    {
        private static readonly ThreadLocal<Random> _local = new ThreadLocal<Random>(() => new Random());
        private static int _counter;

        /// <summary>
        /// Generate string that is unique to this process
        /// </summary>
        /// <returns></returns>
        public static string Generate()
        {
            var intValue = _local.Value.Next();

            var builder = new StringBuilder();

            builder.Append((char)(intValue % 95 + 32));

            intValue = intValue >> 7;
            builder.Append((char)(intValue % 95 + 32));

            intValue = intValue >> 7;
            builder.Append((char)(intValue % 95 + 32));

            intValue = intValue >> 7;
            builder.Append((char)(intValue % 95 + 32));

            builder.Append(Interlocked.Increment(ref _counter));

            return builder.ToString();
        }
    }
}
