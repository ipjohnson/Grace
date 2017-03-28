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
            
            return new string(new[]
            {
                (char)(intValue % 95 + 32),
                (char)((intValue >> 7) % 95 + 32),
                (char)((intValue >> 14) % 95 + 32),
                (char)((intValue >> 21) % 95 + 32)
            }) 
            + Interlocked.Increment(ref _counter);
        }
    }
}
