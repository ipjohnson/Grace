using System.Collections.Generic;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// Class that provides decorators
    /// </summary>
    public interface IDecoratorStrategyProvider
    {
        /// <summary>
        /// Provide a list of decorator strategies
        /// </summary>
        /// <returns></returns>
        IEnumerable<ICompiledDecoratorStrategy> ProvideStrategies();
    }
}
