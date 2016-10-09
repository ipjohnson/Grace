using System;
using System.Collections.Generic;

namespace Grace.DependencyInjection
{
    public interface IActivationStrategyCollectionContainer<T> where T : IActivationStrategy
    {
        void AddStrategy(T strategy);

        IEnumerable<T> GetAllStrategies();

        IActivationStrategyCollection<T> GetActivationStrategyCollection(Type type);

        IActivationStrategyCollectionContainer<T> Clone();

        void AddInspector(IActivationStrategyInspector inspector);
    }
}
