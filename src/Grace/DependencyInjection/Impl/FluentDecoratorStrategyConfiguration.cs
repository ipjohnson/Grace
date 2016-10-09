using System;

namespace Grace.DependencyInjection.Impl
{
    public class FluentDecoratorStrategyConfiguration : IFluentDecoratorStrategyConfiguration
    {
        private readonly ICompiledDecoratorStrategy _strategy;

        public FluentDecoratorStrategyConfiguration(ICompiledDecoratorStrategy strategy)
        {
            _strategy = strategy;
        }


        public IFluentDecoratorStrategyConfiguration ApplyAfterLifestyle()
        {
            _strategy.ApplyAfterLifestyle = true;

            return this;
        }

        public IFluentDecoratorStrategyConfiguration As(Type type)
        {
            _strategy.AddExportAs(type);

            return this;
        }
    }
}
