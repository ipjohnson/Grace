using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grace.Tests.Classes.Simple
{
    public class MethodInjectionClass
    {
        public void InjectBasicService(IBasicService basicService)
        {
            BasicService = basicService;
        }

        public IBasicService BasicService { get; private set; }
    }
}
