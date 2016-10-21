using System;

namespace Grace.DependencyInjection
{
    public interface IFluentWrapperStrategyConfiguration
    {
        IFluentWrapperStrategyConfiguration As(Type type);

        IFluentWrapperStrategyConfiguration WrappedType(Type type);

        IFluentWrapperStrategyConfiguration WrappedGenericArg(int genericArgPosition);
    }
}
