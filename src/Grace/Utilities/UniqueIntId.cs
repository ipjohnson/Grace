using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Grace.Utilities
{
    public static class UniqueIntId
    {
        private static int _uniqueId;

        public static int GetId()
        {
            return Interlocked.Increment(ref _uniqueId);
        }
    }
}
