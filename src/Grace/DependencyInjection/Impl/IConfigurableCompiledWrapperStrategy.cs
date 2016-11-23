using System;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// Wrapper that can be configured
    /// </summary>
    public interface IConfigurableCompiledWrapperStrategy : ICompiledWrapperStrategy
    {
        /// <summary>
        /// Set the type that is being wrapped
        /// </summary>
        /// <param name="type"></param>
        void SetWrappedType(Type type);

        /// <summary>
        /// Set the position of the generic arg that is being wrapped
        /// </summary>
        /// <param name="argPosition"></param>
        void SetWrappedGenericArgPosition(int argPosition);
    }
}
