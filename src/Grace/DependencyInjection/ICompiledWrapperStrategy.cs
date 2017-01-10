using System;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// Compiled wrapper 
    /// </summary>
    public interface ICompiledWrapperStrategy : IConfigurableActivationStrategy, IWrapperOrExportActivationStrategy
    {
        /// <summary>
        /// Get type that wrapper wraps
        /// </summary>
        /// <param name="type">wrapper type</param>
        /// <returns>type that has been wrapped</returns>
        Type GetWrappedType(Type type);
    }
}
