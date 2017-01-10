using System.Collections.Generic;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// Simple class to return decorators
    /// </summary>
    public class SimpleDecoratorStrategyProvider : IDecoratorStrategyProvider
    {
        private readonly ICompiledDecoratorStrategy[] _decorators;

        /// <summary>
        /// default constructor
        /// </summary>
        /// <param name="decorators"></param>
        public SimpleDecoratorStrategyProvider(params ICompiledDecoratorStrategy[] decorators)
        {
            _decorators = decorators;
        }

        /// <summary>
        /// Provide a list of decorator strategies
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ICompiledDecoratorStrategy> ProvideStrategies()
        {
            return _decorators;
        }
    }
}
