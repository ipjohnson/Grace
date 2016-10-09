using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grace.DependencyInjection.Impl
{
    public class DefaultValueInformation : IDefaultValueInformation
    {
        public object DefaultValue { get; set; }
    }
}
