using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grace.DependencyInjection.Conditions;

namespace Grace.DependencyInjection
{
    public interface IWhenConditionConfiguration<T>
    {
        /// <summary>
        /// Use strategy when it meets condition
        /// </summary>
        /// <param name="condition">condition</param>
        /// <returns></returns>
        T MeetsCondition(ICompiledCondition condition);

        /// <summary>
        /// Use strategy when it meets a condition
        /// </summary>
        /// <param name="condition">condition</param>
        /// <returns></returns>
        T MeetsCondition(Func<IActivationStrategy, StaticInjectionContext, bool> condition);

        /// <summary>
        /// Add a condition to use this export only when the clas being injected into has a specific attribute
        /// </summary>
        /// <typeparam name="TAttribute">attribute</typeparam>
        /// <returns></returns>
        T ClassHas<TAttribute>();

        /// <summary>
        /// Use export when injecting into a specific type
        /// </summary>
        /// <typeparam name="TInjectedType">injected type</typeparam>
        /// <returns>configuration</returns>
        T WhenInjectedInto<TInjectedType>();
    }
}
