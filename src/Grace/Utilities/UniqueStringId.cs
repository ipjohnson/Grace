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
        private static int _counter;

        /// <summary>
        /// Generate string that is unique to this process
        /// </summary>
        /// <returns></returns>
        public static string Generate()
        {
            return "*-g" + Interlocked.Increment(ref _counter);
        }
    }
}
