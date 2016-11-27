using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grace.Dynamic.Console
{
    public interface IBasicService
    {
        int Count { get; }
    }

    public class BasicService : IBasicService
    {
        public int Count { get; }
    }
}
