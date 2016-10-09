using System.Collections.Generic;

namespace Grace.DependencyInjection.Impl
{
    public class SimpleDecoratorStrategyProvider : IDecoratorStrategyProvider
    {
        private readonly ICompiledDecoratorStrategy[] _decorators;

        public SimpleDecoratorStrategyProvider(params ICompiledDecoratorStrategy[] decorators)
        {
            _decorators = decorators;
        }

        public IEnumerable<ICompiledDecoratorStrategy> ProvideStrategies()
        {
            return _decorators;
        }
    }
}
