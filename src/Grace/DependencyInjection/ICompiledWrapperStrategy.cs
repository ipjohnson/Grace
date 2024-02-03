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

        /// <summary>
        /// Get a keyed activation strategy for this delegate
        /// </summary>
        /// <param name="scope">injection scope</param>
        /// <param name="compiler"></param>
        /// <param name="activationType">activation type</param>
        /// <param name="key">activation key</param>
        /// <returns>activation delegate</returns>
        ActivationStrategyDelegate GetKeyedActivationStrategyDelegate(IInjectionScope scope, IActivationStrategyCompiler compiler, Type activationType, object key);
    }
}
