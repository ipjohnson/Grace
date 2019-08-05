using System;
using System.Collections.Generic;
using System.Text;

namespace Grace.Tests.Classes.Simple
{
    public interface ICountValueMultipleService
    {
        int Count { get; }
    }


    public class CountValueMultipleService1 : ICountValueMultipleService
    {
        public CountValueMultipleService1(int count)
        {
            Count = count;
        }

        public int Count { get; }
    }

    public class CountValueMultipleService2 : ICountValueMultipleService
    {
        public CountValueMultipleService2(int count)
        {
            Count = count;
        }

        public int Count { get; }
    }

    public class CountValueMultipleService3 : ICountValueMultipleService
    {
        public CountValueMultipleService3(int count)
        {
            Count = count;
        }

        public int Count { get; }
    }
}
