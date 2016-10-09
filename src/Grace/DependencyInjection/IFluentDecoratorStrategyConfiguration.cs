using System;

namespace Grace.DependencyInjection
{
    public interface IFluentDecoratorStrategyConfiguration
    {
        /// <summary>
        /// Apply decorator after lifestyle, by default it's before
        /// </summary>
        /// <returns></returns>
        IFluentDecoratorStrategyConfiguration ApplyAfterLifestyle();

        /// <summary>
        /// Export as particular types
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        IFluentDecoratorStrategyConfiguration As(Type type);
    }
}
