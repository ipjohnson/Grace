using System.Collections.Generic;

namespace Grace.DependencyInjection.Impl
{
    public interface IDecoratorStrategyProvider
    {
        IEnumerable<ICompiledDecoratorStrategy> ProvideStrategies();
    }
}
