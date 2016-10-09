using System.Collections.Generic;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// Provide a list of export wrappers
    /// </summary>
    public interface IWrapperStrategyProvider
    {
        /// <summary>
        /// Provide list of wrappers
        /// </summary>
        /// <returns>list of wrappers</returns>
        IEnumerable<ICompiledWrapperStrategy> ProvideWrappers();
    }
}
