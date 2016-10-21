using System;

namespace Grace.DependencyInjection.Impl
{
    public interface IConfigurableCompiledWrapperStrategy : ICompiledWrapperStrategy
    {
        void SetWrappedType(Type type);

        void SetWrappedGenericArgPosition(int argPosition);
    }
}
