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
        IEnumerable<ICompiledDecoratorStrategy> ProvideStrategies();
    }
}
