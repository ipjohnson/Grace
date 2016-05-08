using System;
using Grace.DependencyInjection.Impl.CompiledExport;

namespace Grace.DependencyInjection.Impl
{
	public partial class FluentExportStrategyConfiguration
	{
		private ConstructorParamInfo currenConstructorParamInfo;

		/// <summary>
		/// Add a specific value for a particuar parameter in the constructor
		/// </summary>
		/// <typeparam name="TParam">type of parameter</typeparam>
		/// <param name="paramValue">Func(T) value for the parameter</param>
		/// <returns></returns>
		public IFluentWithCtorConfiguration WithCtorParam<TParam>(Func<TParam> paramValue = null)
		{
			ProcessCurrentConstructorParamInfo();

			currenConstructorParamInfo = new ConstructorParamInfo
			                             {
				                             ParameterType = typeof(TParam)
			                             };

			if (paramValue != null)
			{
				currenConstructorParamInfo.ValueProvider = new FuncValueProvider<TParam>(paramValue);
			}

			return new FluentWithCtorConfiguration(currenConstructorParamInfo, this);
		}

	    /// <summary>
	    /// Add a specific value for a particuar parameter in the constructor
	    /// </summary>
	    /// <param name="parameterType">type of parameter</param>
	    /// <param name="paramValue">Func(T) value for the parameter</param>
	    /// <returns>configuration object</returns>
	    public IFluentWithCtorConfiguration WithCtorParam(Type parameterType, Func<object> paramValue = null)
        {
            ProcessCurrentConstructorParamInfo();

            currenConstructorParamInfo = new ConstructorParamInfo
            {
                ParameterType = parameterType
            };

            if (paramValue != null)
            {
                currenConstructorParamInfo.ValueProvider = new FuncValueProvider<object>(paramValue);
            }

            return new FluentWithCtorConfiguration(currenConstructorParamInfo, this);
	    }

	    /// <summary>
	    /// Add a specific value for a particuar parameter in the constructor
	    /// </summary>
	    /// <typeparam name="TParam">type of parameter</typeparam>
	    /// <param name="paramValue">Func(IInjectionScope, IInjectionContext, T) value for the parameter</param>
	    /// <returns>configuration object</returns>
	    public IFluentWithCtorConfiguration WithCtorParam<TParam>(
			Func<IInjectionScope, IInjectionContext, TParam> paramValue = null)
		{
			ProcessCurrentConstructorParamInfo();

			currenConstructorParamInfo = new ConstructorParamInfo
			                             {
				                             ParameterType = typeof(TParam)
			                             };

			if (paramValue != null)
			{
				currenConstructorParamInfo.ValueProvider = new FuncValueProvider<TParam>(paramValue);
			}

			return new FluentWithCtorConfiguration(currenConstructorParamInfo, this);
		}

	    /// <summary>
	    /// Add a specific value for a particuar parameter in the constructor
	    /// </summary>
	    /// <param name="parameterType"></param>
	    /// <param name="paramValue">Func(IInjectionScope, IInjectionContext, T) value for the parameter</param>
	    /// <returns>configuration object</returns>
	    public IFluentWithCtorConfiguration WithCtorParam(Type parameterType, Func<IInjectionScope, IInjectionContext, object> paramValue)
        {
            ProcessCurrentConstructorParamInfo();

            currenConstructorParamInfo = new ConstructorParamInfo
            {
                ParameterType = parameterType
            };

            if (paramValue != null)
            {
                currenConstructorParamInfo.ValueProvider = new FuncValueProvider<object>(paramValue);
            }

            return new FluentWithCtorConfiguration(currenConstructorParamInfo, this);
	    }

	    /// <summary>
		/// Processes the current constructor parameter that was being configured
		/// </summary>
		protected void ProcessCurrentConstructorParamInfo()
		{
			if (currenConstructorParamInfo != null)
			{
				exportStrategy.WithCtorParam(currenConstructorParamInfo);

				currenConstructorParamInfo = null;
			}
		}
	}

	public partial class FluentExportStrategyConfiguration<T>
	{
		private ConstructorParamInfo currenConstructorParamInfo;

		/// <summary>
		/// Add a value to be used for constructor parameters
		/// </summary>
		/// <typeparam name="TParam">type of parameter</typeparam>
		/// <param name="paramValue">Func(TParam) for the parameter</param>
		/// <returns></returns>
		public IFluentWithCtorConfiguration<T> WithCtorParam<TParam>(Func<TParam> paramValue = null)
		{
			ProcessCurrentConstructorParamInfo();

			currenConstructorParamInfo = new ConstructorParamInfo
			                             {
				                             ParameterType = typeof(TParam)
			                             };

			if (paramValue != null)
			{
				currenConstructorParamInfo.ValueProvider = new FuncValueProvider<TParam>(paramValue);
			}

			return new FluentWithCtorConfiguration<T>(currenConstructorParamInfo, this);
		}

	    /// <summary>
	    /// Add a vlue to be used for constructor parameter
	    /// </summary>
	    /// <param name="parameterType">parameter type</param>
	    /// <param name="paramValue">parameter value</param>
	    /// <returns>configuration object</returns>
	    public IFluentWithCtorConfiguration<T> WithCtorParam(Type parameterType, Func<object> paramValue = null)
        {
            ProcessCurrentConstructorParamInfo();

            currenConstructorParamInfo = new ConstructorParamInfo
            {
                ParameterType = parameterType
            };

            if (paramValue != null)
            {
                currenConstructorParamInfo.ValueProvider = new FuncValueProvider<object>(paramValue);
            }

            return new FluentWithCtorConfiguration<T>(currenConstructorParamInfo, this);
	    }

	    /// <summary>
		/// Add a specific value for a particuar parameter in the constructor
		/// </summary>
		/// <typeparam name="TParam">type of parameter</typeparam>
		/// <param name="paramValue">Func(IInjectionScope, IInjectionContext, T) value for the parameter</param>
		/// <returns>configuration object</returns>
		public IFluentWithCtorConfiguration<T> WithCtorParam<TParam>(
			Func<IInjectionScope, IInjectionContext, TParam> paramValue = null)
		{
			ProcessCurrentConstructorParamInfo();

			currenConstructorParamInfo = new ConstructorParamInfo
			                             {
				                             ParameterType = typeof(TParam)
			                             };

			if (paramValue != null)
			{
				currenConstructorParamInfo.ValueProvider = new FuncValueProvider<TParam>(paramValue);
			}

			return new FluentWithCtorConfiguration<T>(currenConstructorParamInfo, this);
		}

	    /// <summary>
	    /// Add a vlue to be used for constructor parameter
	    /// </summary>
	    /// <param name="parameterType">parameter type</param>
	    /// <param name="paramValue">parameter value</param>
	    /// <returns>configuration object</returns>
	    public IFluentWithCtorConfiguration<T> WithCtorParam(Type parameterType, Func<IInjectionScope, IInjectionContext, object> paramValue)
        {
            ProcessCurrentConstructorParamInfo();

            currenConstructorParamInfo = new ConstructorParamInfo
            {
                ParameterType = parameterType
            };

            if (paramValue != null)
            {
                currenConstructorParamInfo.ValueProvider = new FuncValueProvider<object>(paramValue);
            }

            return new FluentWithCtorConfiguration<T>(currenConstructorParamInfo, this);
	    }

	    protected void ProcessCurrentConstructorParamInfo()
		{
			if (currenConstructorParamInfo != null)
			{
				exportStrategy.WithCtorParam(currenConstructorParamInfo);

				currenConstructorParamInfo = null;
			}
		}
	}

	public class FluentWithCtorConfiguration : FluentBaseExportConfiguration, IFluentWithCtorConfiguration
	{
		private readonly ConstructorParamInfo currenConstructorParamInfo;

		public FluentWithCtorConfiguration(ConstructorParamInfo constructorParamInfo,
			IFluentExportStrategyConfiguration strategy)
			: base(strategy)
		{
			currenConstructorParamInfo = constructorParamInfo;
		}

		public IFluentWithCtorConfiguration IsRequired(bool isRequired = true)
		{
			currenConstructorParamInfo.IsRequired = isRequired;

			return this;
		}

		public IFluentWithCtorConfiguration LocateWithKey(object locateKey)
		{
			currenConstructorParamInfo.LocateKeyProvider = new FuncLocateKeyProvider(t => locateKey);

			return this;
		}

	    public IFluentWithCtorConfiguration LocateWithKeyProvider(Func<IInjectionScope, IInjectionContext, Type, object> locateKeyFunc)
	    {
            currenConstructorParamInfo.LocateKeyProvider = new FuncLocateKeyProvider(locateKeyFunc);

	        return this;
	    }

	    public IFluentWithCtorConfiguration LocateWithKeyProvider(ILocateKeyValueProvider keyProvider)
	    {
	        currenConstructorParamInfo.LocateKeyProvider = keyProvider;

	        return this;
	    }

	    public IFluentWithCtorConfiguration Named(string name)
		{
			currenConstructorParamInfo.ParameterName = name;

			return this;
		}

		public IFluentWithCtorConfiguration ImportName(string importName)
		{
			currenConstructorParamInfo.ImportName = importName;

			return this;
		}

		public IFluentWithCtorConfiguration Consider(ExportStrategyFilter filter)
		{
			currenConstructorParamInfo.ExportStrategyFilter = filter;

			return this;
		}

		public IFluentWithCtorConfiguration UsingValueProvider(IExportValueProvider valueProvider)
		{
			currenConstructorParamInfo.ValueProvider = valueProvider;

			return this;
		}

        public IFluentWithCtorConfiguration DefaultValue(object defaultValue)
        {
            currenConstructorParamInfo.DefaultValue = defaultValue;

            return this;
        }
    }

	public class FluentWithCtorConfiguration<T> : FluentBaseExportConfiguration<T>,
		IFluentWithCtorConfiguration<T>
	{
		private readonly ConstructorParamInfo constructorParamInfo;

		public FluentWithCtorConfiguration(ConstructorParamInfo constructorParamInfo,
			IFluentExportStrategyConfiguration<T> strategy)
			: base(strategy)
		{
			this.constructorParamInfo = constructorParamInfo;
		}

		public IFluentWithCtorConfiguration<T> IsRequired(bool isRequired = true)
		{
			constructorParamInfo.IsRequired = isRequired;

			return this;
		}

		public IFluentWithCtorConfiguration<T> LocateWithKey(object locateKey)
		{
			constructorParamInfo.LocateKeyProvider = new FuncLocateKeyProvider(t => locateKey);

			return this;
		}

	    public IFluentWithCtorConfiguration<T> LocateWithKeyProvider(Func<IInjectionScope, IInjectionContext, Type, object> locateKeyFunc)
	    {
            constructorParamInfo.LocateKeyProvider = new FuncLocateKeyProvider(locateKeyFunc);

	        return this;
	    }

	    public IFluentWithCtorConfiguration<T> LocateWithKeyProvider(ILocateKeyValueProvider keyProvider)
	    {
	        constructorParamInfo.LocateKeyProvider = keyProvider;

	        return this;
	    }

	    public IFluentWithCtorConfiguration<T> Named(string name)
		{
			constructorParamInfo.ParameterName = name;

			return this;
		}

		public IFluentWithCtorConfiguration<T> ImportName(string importName)
		{
			constructorParamInfo.ImportName = importName;

			return this;
		}

		public IFluentWithCtorConfiguration<T> Consider(ExportStrategyFilter filter)
		{
			constructorParamInfo.ExportStrategyFilter = filter;

			return this;
		}

		public IFluentWithCtorConfiguration<T> UsingValueProvider(IExportValueProvider valueProvider)
		{
			constructorParamInfo.ValueProvider = valueProvider;

			return this;
		}

        public IFluentWithCtorConfiguration<T> DefaultValue(object defaultValue)
        {
            constructorParamInfo.DefaultValue = defaultValue;

            return this;
        }
    }
}