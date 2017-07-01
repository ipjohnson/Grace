using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Grace.Tests.Classes.Simple
{
    public class CustomBasicService : IBasicService
    {
        public int Count { get; set; }

        public int TestMethod()
        {
            return Count;
        }
    }
}
