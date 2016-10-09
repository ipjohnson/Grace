using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grace.DependencyInjection
{
    public interface IFluentWrapperStrategyConfiguration
    {
        IFluentWrapperStrategyConfiguration As(Type type);

        IFluentWrapperStrategyConfiguration WrappedType(Type type);

        IFluentWrapperStrategyConfiguration WrappedGenericArg(int genericArgPosition);
    }
}
