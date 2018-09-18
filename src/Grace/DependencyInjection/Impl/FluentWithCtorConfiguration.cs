using System;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// 
    /// </summary>
    public class FluentWithCtorConfiguration : ProxyFluentExportStrategyConfiguration,
        IFluentWithCtorConfiguration
    {
        private ConstructorParameterInfo _constructorParameterInfo;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="strategy"></param>
        /// <param name="constructorParameterInfo"></param>
        public FluentWithCtorConfiguration(IFluentExportStrategyConfiguration strategy, ConstructorParameterInfo constructorParameterInfo) : base(strategy)
        {
            _constructorParameterInfo = constructorParameterInfo;
        }

        /// <summary>
        /// Applies a filter to be used when resolving a parameter constructor
        /// It will be called each time the parameter is resolved
        /// </summary>
        /// <param name="filter">filter delegate to be used when resolving parameter</param>
        /// <returns>configuration object</returns>
        public IFluentWithCtorConfiguration Consider(ActivationStrategyFilter filter)
        {
            _constructorParameterInfo.ExportStrategyFilter = filter;

            return this;
        }

        /// <summary>
        /// Assign a default value if no better option is found
        /// </summary>
        /// <param name="defaultValue">default value</param>
        /// <returns>configuration object</returns>
        public IFluentWithCtorConfiguration DefaultValue(object defaultValue)
        {
            _constructorParameterInfo.DefaultValue = defaultValue;

            return this;
        }

        /// <summary>
        /// Default value func
        /// </summary>
        /// <param name="defaultValueFunc"></param>
        /// <returns></returns>
        public IFluentWithCtorConfiguration DefaultValue(Func<object> defaultValueFunc)
        {
            _constructorParameterInfo.DefaultValue = defaultValueFunc;

            return this;
        }

        /// <summary>
        /// Default value function
        /// </summary>
        /// <param name="defaultValueFunc"></param>
        /// <returns></returns>
        public IFluentWithCtorConfiguration DefaultValue(Func<IExportLocatorScope, StaticInjectionContext, IInjectionContext, object> defaultValueFunc)
        {
            _constructorParameterInfo.DefaultValue = defaultValueFunc;

            return this;
        }

        /// <summary>
        /// Mark the parameter as dynamic
        /// </summary>
        /// <param name="isDynamic"></param>
        /// <returns>configuration object</returns>
        public IFluentWithCtorConfiguration IsDynamic(bool isDynamic = true)
        {
            _constructorParameterInfo.IsDynamic = isDynamic;

            return this;
        }

        /// <summary>
        /// Is the parameter required when resolving the type
        /// </summary>
        /// <param name="isRequired">is the parameter required</param>
        /// <returns>configuration object</returns>
        public IFluentWithCtorConfiguration IsRequired(bool isRequired = true)
        {
            _constructorParameterInfo.IsRequired = isRequired;

            return this;
        }

        /// <summary>
        /// Locate with a particular key
        /// </summary>
        /// <param name="locateKey">ocate key</param>
        /// <returns>configuration object</returns>
        public IFluentWithCtorConfiguration LocateWithKey(object locateKey)
        {
            _constructorParameterInfo.LocateWithKey = locateKey;

            return this;
        }

        /// <summary>
        /// Name of the parameter being configured
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IFluentWithCtorConfiguration Named(string name)
        {
            _constructorParameterInfo.ParameterName = name;

            return this;
        }

        /// <summary>
        /// Use a specific type for parameter
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IFluentWithCtorConfiguration Use(Type type)
        {
            _constructorParameterInfo.UseType = type;

            return this;
        }
    }


    /// <summary>
    /// Constructor parameter configuration
    /// </summary>
    /// <typeparam name="TParam"></typeparam>
    public class FluentWithCtorConfiguration<TParam> : ProxyFluentExportStrategyConfiguration,
        IFluentWithCtorConfiguration<TParam>
    {
        private readonly ConstructorParameterInfo _constructorParameterInfo;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="strategy"></param>
        /// <param name="constructorParameterInfo"></param>
        public FluentWithCtorConfiguration(IFluentExportStrategyConfiguration strategy, ConstructorParameterInfo constructorParameterInfo) : base(strategy)
        {
            _constructorParameterInfo = constructorParameterInfo;
        }

        /// <summary>
        /// Name of the parameter being configured
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IFluentWithCtorConfiguration<TParam> Named(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            _constructorParameterInfo.ParameterName = name;

            return this;
        }

        /// <summary>
        /// Use a specific type for parameter
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IFluentWithCtorConfiguration<TParam> Use(Type type)
        {
            _constructorParameterInfo.UseType = type;

            return this;
        }

        /// <summary>
        /// Applies a filter to be used when resolving a parameter constructor
        /// It will be called each time the parameter is resolved
        /// </summary>
        /// <param name="filter">filter delegate to be used when resolving parameter</param>
        /// <returns>configuration object</returns>
        public IFluentWithCtorConfiguration<TParam> Consider(ActivationStrategyFilter filter)
        {
            if (filter == null) throw new ArgumentNullException(nameof(filter));

            _constructorParameterInfo.ExportStrategyFilter = filter;

            return this;
        }

        /// <summary>
        /// Assign a default value if no better option is found
        /// </summary>
        /// <param name="defaultValue">default value</param>
        /// <returns>configuration object</returns>
        public IFluentWithCtorConfiguration<TParam> DefaultValue(TParam defaultValue)
        {
            _constructorParameterInfo.DefaultValue = defaultValue;

            return this;
        }

        /// <summary>
        /// Default value func
        /// </summary>
        /// <param name="defaultValueFunc"></param>
        /// <returns></returns>
        public IFluentWithCtorConfiguration<TParam> DefaultValue(Func<TParam> defaultValueFunc)
        {
            if (defaultValueFunc == null) throw new ArgumentNullException(nameof(defaultValueFunc));

            _constructorParameterInfo.DefaultValue = defaultValueFunc;

            return this;
        }

        /// <summary>
        /// Default value func
        /// </summary>
        /// <param name="defaultValueFunc"></param>
        /// <returns></returns>
        public IFluentWithCtorConfiguration<TParam> DefaultValue(Func<IExportLocatorScope, StaticInjectionContext, IInjectionContext, TParam> defaultValueFunc)
        {
            if (defaultValueFunc == null) throw new ArgumentNullException(nameof(defaultValueFunc));

            _constructorParameterInfo.DefaultValue = defaultValueFunc;

            return this;
        }

        /// <summary>
        /// Is the parameter required when resolving the type
        /// </summary>
        /// <param name="isRequired">is the parameter required</param>
        /// <returns>configuration object</returns>
        public IFluentWithCtorConfiguration<TParam> IsRequired(bool isRequired = true)
        {
            _constructorParameterInfo.IsRequired = isRequired;

            return this;
        }

        /// <summary>
        /// Locate with a particular key
        /// </summary>
        /// <param name="locateKey">ocate key</param>
        /// <returns>configuration object</returns>
        public IFluentWithCtorConfiguration<TParam> LocateWithKey(object locateKey)
        {
            if (locateKey == null) throw new ArgumentNullException(nameof(locateKey));

            _constructorParameterInfo.LocateWithKey = locateKey;

            return this;
        }

        /// <summary>
        /// Mark the parameter as dynamic
        /// </summary>
        /// <param name="isDynamic"></param>
        /// <returns>configuration object</returns>
        public IFluentWithCtorConfiguration<TParam> IsDynamic(bool isDynamic = true)
        {
            _constructorParameterInfo.IsDynamic = isDynamic;

            return this;
        }
    }

    /// <summary>
    /// Configuration object for constructor parameter
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TParam"></typeparam>
    public class FluentWithCtorConfiguration<T, TParam> : ProxyFluentExportStrategyConfiguration<T>, IFluentWithCtorConfiguration<T, TParam>
    {
        private readonly ConstructorParameterInfo _constructorParameterInfo;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="strategy"></param>
        /// <param name="constructorParameterInfo"></param>
        public FluentWithCtorConfiguration(IFluentExportStrategyConfiguration<T> strategy, ConstructorParameterInfo constructorParameterInfo) : base(strategy)
        {
            _constructorParameterInfo = constructorParameterInfo;
        }

        /// <summary>
        /// Applies a filter to be used when resolving a parameter constructor
        /// It will be called each time the parameter is resolved
        /// </summary>
        /// <param name="filter">filter delegate to be used when resolving parameter</param>
        /// <returns>configuration object</returns>
        public IFluentWithCtorConfiguration<T, TParam> Consider(ActivationStrategyFilter filter)
        {
            _constructorParameterInfo.ExportStrategyFilter = filter;

            return this;
        }

        /// <summary>
        /// Assign a default value if no better option is found
        /// </summary>
        /// <param name="defaultValue">default value</param>
        /// <returns>configuration object</returns>
        public IFluentWithCtorConfiguration<T, TParam> DefaultValue(TParam defaultValue)
        {
            _constructorParameterInfo.DefaultValue = defaultValue;

            return this;
        }

        /// <summary>
        /// Default value func
        /// </summary>
        /// <param name="defaultValueFunc"></param>
        /// <returns></returns>
        public IFluentWithCtorConfiguration<T, TParam> DefaultValue(Func<TParam> defaultValueFunc)
        {
            if (defaultValueFunc == null) throw new ArgumentNullException(nameof(defaultValueFunc));

            _constructorParameterInfo.DefaultValue = defaultValueFunc;

            return this;
        }

        /// <summary>
        /// Default value func
        /// </summary>
        /// <param name="defaultValueFunc"></param>
        /// <returns></returns>
        public IFluentWithCtorConfiguration<T, TParam> DefaultValue(Func<IExportLocatorScope, StaticInjectionContext, IInjectionContext, TParam> defaultValueFunc)
        {
            if (defaultValueFunc == null) throw new ArgumentNullException(nameof(defaultValueFunc));

            _constructorParameterInfo.DefaultValue = defaultValueFunc;

            return this;
        }

        /// <summary>
        /// Mark the parameter as dynamic (will be located from child scopes)
        /// </summary>
        /// <param name="isDynamic"></param>
        /// <returns></returns>
        public IFluentWithCtorConfiguration<T, TParam> IsDynamic(bool isDynamic = true)
        {
            _constructorParameterInfo.IsDynamic = isDynamic;

            return this;
        }

        /// <summary>
        /// Is the parameter required when resolving the type
        /// </summary>
        /// <param name="isRequired">is the parameter required</param>
        /// <returns>configuration object</returns>
        public IFluentWithCtorConfiguration<T, TParam> IsRequired(bool isRequired = true)
        {
            _constructorParameterInfo.IsRequired = isRequired;

            return this;
        }

        /// <summary>
        /// Locate with a particular key
        /// </summary>
        /// <param name="locateKey">ocate key</param>
        /// <returns>configuration object</returns>
        public IFluentWithCtorConfiguration<T, TParam> LocateWithKey(object locateKey)
        {
            _constructorParameterInfo.LocateWithKey = locateKey;

            return this;
        }

        /// <summary>
        /// Name of the parameter being configured
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IFluentWithCtorConfiguration<T, TParam> Named(string name)
        {
            _constructorParameterInfo.ParameterName = name;

            return this;
        }

        /// <summary>
        /// Use specific type to satisfy parameter
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IFluentWithCtorConfiguration<T, TParam> Use(Type type)
        {
            _constructorParameterInfo.UseType = type;

            return this;
        }
    }
}

