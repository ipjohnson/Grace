using System;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// configure constructor parameter
    /// </summary>
    /// <typeparam name="TParam"></typeparam>
    public interface IFluentWithCtorConfiguration<in TParam>
    {
        /// <summary>
        /// Name of the parameter being configured
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IFluentWithCtorConfiguration<TParam> Named(string name);

        /// <summary>
        /// Applies a filter to be used when resolving a parameter constructor
        /// It will be called each time the parameter is resolved
        /// </summary>
        /// <param name="filter">filter delegate to be used when resolving parameter</param>
        /// <returns>configuration object</returns>
        IFluentWithCtorConfiguration<TParam> Consider(ExportStrategyFilter filter);

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
    }

    /// <summary>
    /// configure constructor parameter
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TParam"></typeparam>
    public interface IFluentWithCtorConfiguration<T, in TParam> 
    {
        /// <summary>
        /// Name of the parameter being configured
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IFluentWithCtorConfiguration<T, TParam> Named(string name);

        /// <summary>
        /// Applies a filter to be used when resolving a parameter constructor
        /// It will be called each time the parameter is resolved
        /// </summary>
        /// <param name="filter">filter delegate to be used when resolving parameter</param>
        /// <returns>configuration object</returns>
        IFluentWithCtorConfiguration<T, TParam> Consider(ExportStrategyFilter filter);

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
    }
}
