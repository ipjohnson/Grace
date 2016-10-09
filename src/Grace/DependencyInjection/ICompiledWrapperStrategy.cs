using System;

namespace Grace.DependencyInjection
{
    public interface ICompiledWrapperStrategy : IConfigurableActivationStrategy, IWrapperOrExportActivationStrategy
    {
        Type GetWrappedType(Type type);
    }
}
