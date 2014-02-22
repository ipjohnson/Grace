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
		public IFluentWithCtorConfiguration<TParam> WithCtorParam<TParam>(Func<TParam> paramValue = null)
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

			return new FluentWithCtorConfiguration<TParam>(currenConstructorParamInfo, this);
		}

		public IFluentWithCtorConfiguration<TParam> WithCtorParam<TParam>(
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

			return new FluentWithCtorConfiguration<TParam>(currenConstructorParamInfo, this);
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
		public IFluentWithCtorConfiguration<T, TParam> WithCtorParam<TParam>(Func<TParam> paramValue = null)
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

			return new FluentWithCtorConfiguration<T, TParam>(currenConstructorParamInfo, this);
		}

		/// <summary>
		/// Add a specific value for a particuar parameter in the constructor
		/// </summary>
		/// <typeparam name="TParam">type of parameter</typeparam>
		/// <param name="paramValue">Func(IInjectionScope, IInjectionContext, T) value for the parameter</param>
		/// <returns>configuration object</returns>
		public IFluentWithCtorConfiguration<T, TParam> WithCtorParam<TParam>(
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

			return new FluentWithCtorConfiguration<T, TParam>(currenConstructorParamInfo, this);
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

	public class FluentWithCtorConfiguration<TParam> : FluentBaseExportConfiguration, IFluentWithCtorConfiguration<TParam>
	{
		private readonly ConstructorParamInfo currenConstructorParamInfo;

		public FluentWithCtorConfiguration(ConstructorParamInfo constructorParamInfo,
			IFluentExportStrategyConfiguration strategy)
			: base(strategy)
		{
			currenConstructorParamInfo = constructorParamInfo;
		}

		public IFluentWithCtorConfiguration<TParam> IsRequired(bool isRequired = true)
		{
			currenConstructorParamInfo.IsRequired = isRequired;

			return this;
		}

		public IFluentWithCtorConfiguration<TParam> Named(string name)
		{
			currenConstructorParamInfo.ParameterName = name;

			return this;
		}

		public IFluentWithCtorConfiguration<TParam> ImportName(string importName)
		{
			currenConstructorParamInfo.ImportName = importName;

			return this;
		}

		public IFluentWithCtorConfiguration<TParam> Consider(ExportStrategyFilter filter)
		{
			currenConstructorParamInfo.ExportStrategyFilter = filter;

			return this;
		}

		public IFluentWithCtorConfiguration<TParam> UsingValueProvider(IExportValueProvider valueProvider)
		{
			currenConstructorParamInfo.ValueProvider = valueProvider;

			return this;
		}
	}

	public class FluentWithCtorConfiguration<T, TParam> : FluentBaseExportConfiguration<T>,
		IFluentWithCtorConfiguration<T, TParam>
	{
		private readonly ConstructorParamInfo constructorParamInfo;

		public FluentWithCtorConfiguration(ConstructorParamInfo constructorParamInfo,
			IFluentExportStrategyConfiguration<T> strategy)
			: base(strategy)
		{
			this.constructorParamInfo = constructorParamInfo;
		}

		public IFluentWithCtorConfiguration<T, TParam> IsRequired(bool isRequired = true)
		{
			constructorParamInfo.IsRequired = isRequired;

			return this;
		}

		public IFluentWithCtorConfiguration<T, TParam> Named(string name)
		{
			constructorParamInfo.ParameterName = name;

			return this;
		}

		public IFluentWithCtorConfiguration<T, TParam> ImportName(string importName)
		{
			constructorParamInfo.ImportName = importName;

			return this;
		}

		public IFluentWithCtorConfiguration<T, TParam> Consider(ExportStrategyFilter filter)
		{
			constructorParamInfo.ExportStrategyFilter = filter;

			return this;
		}

		public IFluentWithCtorConfiguration<T, TParam> UsingValueProvider(IExportValueProvider valueProvider)
		{
			constructorParamInfo.ValueProvider = valueProvider;

			return this;
		}
	}
}