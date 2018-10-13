using System;

namespace Grace.DependencyInjection
{

    /// <summary>
    /// configure constructor parameter
    /// </summary>
    public interface IFluentWithCtorConfiguration : IFluentExportStrategyConfiguration
    {
        /// <summary>
        /// Applies a filter to be used when resolving a parameter constructor
        /// It will be called each time the parameter is resolved
        /// </summary>
        /// <param name="filter">filter delegate to be used when resolving parameter</param>
        /// <returns>configuration object</returns>
        IFluentWithCtorConfiguration Consider(ActivationStrategyFilter filter);

        /// <summary>
        /// Assign a default value if no better option is found
        /// </summary>
        /// <param name="defaultValue">default value</param>
        /// <returns>configuration object</returns>
        IFluentWithCtorConfiguration DefaultValue(object defaultValue);

        /// <summary>
        /// Default value func
        /// </summary>
        /// <param name="defaultValueFunc"></param>
        /// <returns></returns>
        IFluentWithCtorConfiguration DefaultValue(Func<object> defaultValueFunc);

        /// <summary>
        /// Default value function
        /// </summary>
        /// <param name="defaultValueFunc"></param>
        /// <returns></returns>
        IFluentWithCtorConfiguration DefaultValue(Func<IExportLocatorScope, StaticInjectionContext, IInjectionContext, object> defaultValueFunc);

        /// <summary>
        /// Mark the parameter as dynamic
        /// </summary>
        /// <param name="isDynamic"></param>
        /// <returns>configuration object</returns>
        IFluentWithCtorConfiguration IsDynamic(bool isDynamic = true);

        /// <summary>
        /// Is the parameter required when resolving the type
        /// </summary>
        /// <param name="isRequired">is the parameter required</param>
        /// <returns>configuration object</returns>
        IFluentWithCtorConfiguration IsRequired(bool isRequired = true);

        /// <summary>
        /// Locate with a particular key
        /// </summary>
        /// <param name="locateKey">ocate key</param>
        /// <returns>configuration object</returns>
        IFluentWithCtorConfiguration LocateWithKey(object locateKey);

        /// <summary>
        /// Name of the parameter being configured
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IFluentWithCtorConfiguration Named(string name);

        /// <summary>
        /// Use a specific type for parameter
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        IFluentWithCtorConfiguration Use(Type type);
    }

    /// <summary>
    /// configure constructor parameter
    /// </summary>
    /// <typeparam name="TParam"></typeparam>
    public interface IFluentWithCtorConfiguration<in TParam> : IFluentExportStrategyConfiguration
    {
        /// <summary>
        /// Applies a filter to be used when resolving a parameter constructor
        /// It will be called each time the parameter is resolved
        /// </summary>
        /// <param name="filter">filter delegate to be used when resolving parameter</param>
        /// <returns>configuration object</returns>
        IFluentWithCtorConfiguration<TParam> Consider(ActivationStrategyFilter filter);

        /// <summary>
        /// Assign a default value if no better option is found
        /// </summary>
        /// <param name="defaultValue">default value</param>
        /// <returns>configuration object</returns>
        IFluentWithCtorConfiguration<TParam> DefaultValue(TParam defaultValue);

        /// <summary>
        /// Default value func
        /// </summary>
        /// <param name="defaultValueFunc"></param>
        /// <returns></returns>
        IFluentWithCtorConfiguration<TParam> DefaultValue(Func<TParam> defaultValueFunc);

        /// <summary>
        /// Default value func
        /// </summary>
        /// <param name="defaultValueFunc"></param>
        /// <returns></returns>
        IFluentWithCtorConfiguration<TParam> DefaultValue(Func<IExportLocatorScope, StaticInjectionContext, IInjectionContext, TParam> defaultValueFunc);

        /// <summary>
        /// Mark the parameter as dynamic
        /// </summary>
        /// <param name="isDynamic"></param>
        /// <returns>configuration object</returns>
        IFluentWithCtorConfiguration<TParam> IsDynamic(bool isDynamic = true);

        /// <summary>
        /// Is the parameter required when resolving the type
        /// </summary>
        /// <param name="isRequired">is the parameter required</param>
        /// <returns>configuration object</returns>
        IFluentWithCtorConfiguration<TParam> IsRequired(bool isRequired = true);

        /// <summary>
        /// Locate with a particular key
        /// </summary>
        /// <param name="locateKey">ocate key</param>
        /// <returns>configuration object</returns>
        IFluentWithCtorConfiguration<TParam> LocateWithKey(object locateKey);

        /// <summary>
        /// Name of the parameter being configured
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IFluentWithCtorConfiguration<TParam> Named(string name);
        
        /// <summary>
        /// Use a specific type for parameter
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        IFluentWithCtorConfiguration<TParam> Use(Type type);
    }

    /// <summary>
    /// configure constructor parameter
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TParam"></typeparam>
    public interface IFluentWithCtorConfiguration<T, in TParam> : IFluentExportStrategyConfiguration<T>
    {
        /// <summary>
        /// Applies a filter to be used when resolving a parameter constructor
        /// It will be called each time the parameter is resolved
        /// </summary>
        /// <param name="filter">filter delegate to be used when resolving parameter</param>
        /// <returns>configuration object</returns>
        IFluentWithCtorConfiguration<T, TParam> Consider(ActivationStrategyFilter filter);

        /// <summary>
        /// Assign a default value if no better option is found
        /// </summary>
        /// <param name="defaultValue">default value</param>
        /// <returns>configuration object</returns>
        IFluentWithCtorConfiguration<T, TParam> DefaultValue(TParam defaultValue);

        /// <summary>
        /// Default value func
        /// </summary>
        /// <param name="defaultValueFunc"></param>
        /// <returns></returns>
        IFluentWithCtorConfiguration<T, TParam> DefaultValue(Func<TParam> defaultValueFunc);

        /// <summary>
        /// Default value func
        /// </summary>
        /// <param name="defaultValueFunc"></param>
        /// <returns></returns>
        IFluentWithCtorConfiguration<T, TParam> DefaultValue(Func<IExportLocatorScope, StaticInjectionContext, IInjectionContext, TParam> defaultValueFunc);

        /// <summary>
        /// Mark the parameter as dynamic (will be located from child scopes)
        /// </summary>
        /// <param name="isDynamic"></param>
        /// <returns></returns>
        IFluentWithCtorConfiguration<T, TParam> IsDynamic(bool isDynamic = true);

        /// <summary>
        /// Is the parameter required when resolving the type
        /// </summary>
        /// <param name="isRequired">is the parameter required</param>
        /// <returns>configuration object</returns>
        IFluentWithCtorConfiguration<T, TParam> IsRequired(bool isRequired = true);

        /// <summary>
        /// Locate with a particular key
        /// </summary>
        /// <param name="locateKey">ocate key</param>
        /// <returns>configuration object</returns>
        IFluentWithCtorConfiguration<T, TParam> LocateWithKey(object locateKey);
        
        /// <summary>
        /// Name of the parameter being configured
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IFluentWithCtorConfiguration<T, TParam> Named(string name);

        /// <summary>
        /// Use specific type to satisfy parameter
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        IFluentWithCtorConfiguration<T, TParam> Use(Type type);
    }
}
