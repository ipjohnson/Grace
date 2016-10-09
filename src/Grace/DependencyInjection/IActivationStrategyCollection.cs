using System.Collections.Generic;
using Grace.Data.Immutable;

namespace Grace.DependencyInjection
{
    public interface IActivationStrategyCollection<T> where T : IActivationStrategy
    {
        void AddStrategy(T strategy, object key);

        T GetPrimary();

        ImmutableArray<T> GetStrategies();

        IEnumerable<KeyValuePair<object, T>> GetKeyedStrategies();

        T GetKeyedStrategy(object key);

        IActivationStrategyCollection<T> Clone();
    }
}
