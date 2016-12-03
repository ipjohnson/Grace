using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grace.Tests.Classes.Simple
{
    public class ImportIntMethodClass
    {
        public void SetValue(int value)
        {
            Value = value;
        }

        public int Value { get; private set; }
    }
}
