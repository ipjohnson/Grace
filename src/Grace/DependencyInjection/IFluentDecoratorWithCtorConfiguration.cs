using System;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// cconfigure decorator parameter
    /// </summary>
    /// <typeparam name="TParam"></typeparam>
    public interface IFluentDecoratorWithCtorConfiguration<TParam> : IFluentDecoratorStrategyConfiguration
    {
        /// <summary>
        /// Applies a filter to be used when resolving a parameter constructor
        /// It will be called each time the parameter is resolved
        /// </summary>
        /// <param name="filter">filter delegate to be used when resolving parameter</param>
        /// <returns>configuration object</returns>
        IFluentDecoratorWithCtorConfiguration<TParam> Consider(ActivationStrategyFilter filter);

        /// <summary>
        /// Assign a default value if no better option is found
        /// </summary>
        /// <param name="defaultValue">default value</param>
        /// <returns>configuration object</returns>
        IFluentDecoratorWithCtorConfiguration<TParam> DefaultValue(TParam defaultValue);

        /// <summary>
        /// Default value func
        /// </summary>
        /// <param name="defaultValueFunc"></param>
        /// <returns></returns>
        IFluentDecoratorWithCtorConfiguration<TParam> DefaultValue(Func<TParam> defaultValueFunc);

        /// <summary>
        /// Default value func
        /// </summary>
        /// <param name="defaultValueFunc"></param>
        /// <returns></returns>
        IFluentDecoratorWithCtorConfiguration<TParam> DefaultValue(Func<IExportLocatorScope, StaticInjectionContext, IInjectionContext, TParam> defaultValueFunc);

        /// <summary>
        /// Mark the parameter as dynamic
        /// </summary>
        /// <param name="isDynamic"></param>
        /// <returns>configuration object</returns>
        IFluentDecoratorWithCtorConfiguration<TParam> IsDynamic(bool isDynamic = true);

        /// <summary>
        /// Is the parameter required when resolving the type
        /// </summary>
        /// <param name="isRequired">is the parameter required</param>
        /// <returns>configuration object</returns>
        IFluentDecoratorWithCtorConfiguration<TParam> IsRequired(bool isRequired = true);

        /// <summary>
        /// Locate with a particular key
        /// </summary>
        /// <param name="locateKey">ocate key</param>
        /// <returns>configuration object</returns>
        IFluentDecoratorWithCtorConfiguration<TParam> LocateWithKey(object locateKey);

        /// <summary>
        /// Name of the parameter being configured
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IFluentDecoratorWithCtorConfiguration<TParam> Named(string name);

        /// <summary>
        /// Use a specific type for parameter
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        IFluentDecoratorWithCtorConfiguration<TParam> Use(Type type);
    }
}
