using System;
using System.Linq.Expressions;
using System.Reflection;
using Grace.DependencyInjection.Impl.CompiledExport;

namespace Grace.DependencyInjection.Impl
{

	#region Non Generic

	public partial class FluentExportStrategyConfiguration
	{
		private ImportMethodInfo currentImportMethodInfo;

		/// <summary>
		/// Mark a method for importing
		/// </summary>
		/// <param name="methodName">method name to import</param>
		/// <returns>configuration object</returns>
		public IFluentImportMethodConfiguration ImportMethod(string methodName)
		{
			ProcessCurrentImportMethodInfo();

			foreach (MethodInfo runtimeMethod in exportType.GetRuntimeMethods())
			{
				if (runtimeMethod.Name == methodName)
				{
					currentImportMethodInfo = new ImportMethodInfo
					                          {
						                          MethodToImport = runtimeMethod
					                          };

					return new FluentImportMethodConfiguration(currentImportMethodInfo, this);
				}
			}

			throw new ArgumentException(string.Format("could not find methodName {0} on type {1}",
				methodName,
				exportType.FullName));
		}

		/// <summary>
		/// Process the current import method
		/// </summary>
		protected void ProcessCurrentImportMethodInfo()
		{
			if (currentImportMethodInfo != null)
			{
				exportStrategy.ImportMethod(currentImportMethodInfo);

				currentImportMethodInfo = null;
			}
		}
	}

	/// <summary>
	/// Configuration object for importing a method
	/// </summary>
	public class FluentImportMethodConfiguration : FluentBaseExportConfiguration, IFluentImportMethodConfiguration
	{
		private ImportMethodInfo methodInfo;

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="methodInfo">method to import</param>
		/// <param name="strategy">strategy to configure</param>
		public FluentImportMethodConfiguration(ImportMethodInfo methodInfo, IFluentExportStrategyConfiguration strategy)
			: base(strategy)
		{
		}

		public IFluentImportMethodConfiguration AfterConstruction()
		{
			throw new NotImplementedException();
		}

		public IFluentMethodParameterConfiguration<TParam> WithParam<TParam>(Func<TParam> paramValueFunc = null)
		{
			throw new NotImplementedException();
		}
	}

	/// <summary>
	/// Parameter configuration object
	/// </summary>
	/// <typeparam name="TParam"></typeparam>
	public class FluentMethodParameterConfiguration<TParam> : FluentBaseExportConfiguration,
		IFluentMethodParameterConfiguration<TParam>
	{
		public FluentMethodParameterConfiguration(IFluentExportStrategyConfiguration strategy)
			: base(strategy)
		{
		}

		public IFluentImportMethodConfiguration AfterConstruction()
		{
			throw new NotImplementedException();
		}

		public IFluentMethodParameterConfiguration<TParam1> WithParam<TParam1>(Func<TParam1> paramValueFunc = null)
		{
			throw new NotImplementedException();
		}

		public IFluentMethodParameterConfiguration<TParam> ImportParameterAfterConstruction()
		{
			throw new NotImplementedException();
		}

		public IFluentMethodParameterConfiguration<TParam> IsRequired(bool isRequired = true)
		{
			throw new NotImplementedException();
		}

		public IFluentMethodParameterConfiguration<TParam> ImportName(string importName)
		{
			throw new NotImplementedException();
		}

		public IFluentMethodParameterConfiguration<TParam> UsingValueProvider(IExportValueProvider valueProvider)
		{
			throw new NotImplementedException();
		}
	}

	#endregion

	#region Generic

	public partial class FluentExportStrategyConfiguration<T>
	{
		private ImportMethodInfo currentImportMethodInfo;

		/// <summary>
		/// Mark a method for importing
		/// </summary>
		/// <param name="method">method to import</param>
		/// <returns>configuration object</returns>
		public IFluentImportMethodConfiguration<T> ImportMethod(Expression<Action<T>> method)
		{
			ProcessCurrentImportMethodInfo();

			MethodCallExpression callExpression = method.Body as MethodCallExpression;

			if (callExpression == null)
			{
				throw new ArgumentException("must be method", "method");
			}

			currentImportMethodInfo = new ImportMethodInfo
			                          {
				                          MethodToImport = callExpression.Method
			                          };

			return new FluentImportMethodConfiguration<T>(currentImportMethodInfo, this);
		}

		/// <summary>
		/// Process the current method
		/// </summary>
		protected void ProcessCurrentImportMethodInfo()
		{
			if (currentImportMethodInfo != null)
			{
				exportStrategy.ImportMethod(currentImportMethodInfo);

				currentImportMethodInfo = null;
			}
		}
	}

	/// <summary>
	/// Object for configuring a method for importing
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class FluentImportMethodConfiguration<T> : FluentBaseExportConfiguration<T>, IFluentImportMethodConfiguration<T>
	{
		private readonly ImportMethodInfo methodInfo;

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="methodInfo">method to import</param>
		/// <param name="strategy">trategy to configure</param>
		public FluentImportMethodConfiguration(ImportMethodInfo methodInfo, IFluentExportStrategyConfiguration<T> strategy)
			: base(strategy)
		{
			this.methodInfo = methodInfo;
		}

		/// <summary>
		/// Configure a particular parameter in a method
		/// </summary>
		/// <typeparam name="TParam"></typeparam>
		/// <param name="paramValueFunc"></param>
		/// <returns></returns>
		public IFluentMethodParameterConfiguration<T, TParam> WithMethodParam<TParam>(Func<TParam> paramValueFunc = null)
		{
			MethodParamInfo info = new MethodParamInfo
			                       {
				                       ParameterType = typeof(TParam),
				                       IsRequired = true
			                       };

			if (paramValueFunc != null)
			{
				info.ValueProvider = new FuncValueProvider<TParam>(paramValueFunc);
			}

			methodInfo.AddMethodParamInfo(info);

			return new FluentMethodParameterConfiguration<T, TParam>(info, this);
		}
	}

	/// <summary>
	/// Parameter configuration object
	/// </summary>
	/// <typeparam name="T">type being exported</typeparam>
	/// <typeparam name="TProp">parameter type being configured</typeparam>
	public class FluentMethodParameterConfiguration<T, TProp> : FluentBaseExportConfiguration<T>,
		IFluentMethodParameterConfiguration<T, TProp>
	{
		private readonly MethodParamInfo methodInfo;
		private readonly FluentImportMethodConfiguration<T> importMethodConfiguration;

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="methodInfo"></param>
		/// <param name="importMethodConfiguration"></param>
		public FluentMethodParameterConfiguration(MethodParamInfo methodInfo,
			FluentImportMethodConfiguration<T> importMethodConfiguration)
			: base(importMethodConfiguration)
		{
			this.methodInfo = methodInfo;
			this.importMethodConfiguration = importMethodConfiguration;
		}

		/// <summary>
		/// Specify a particular parameter 
		/// </summary>
		/// <typeparam name="TParam">parameter type</typeparam>
		/// <param name="paramValueFunc">value providing func</param>
		/// <returns>configuration object</returns>
		public IFluentMethodParameterConfiguration<T, TParam> WithMethodParam<TParam>(Func<TParam> paramValueFunc = null)
		{
			return importMethodConfiguration.WithMethodParam(paramValueFunc);
		}

		/// <summary>
		/// specify parameter name
		/// </summary>
		/// <param name="parameterName">parameter name</param>
		/// <returns>configuration object</returns>
		public IFluentMethodParameterConfiguration<T, TProp> Named(string parameterName)
		{
			methodInfo.ParameterName = parameterName;

			return this;
		}

		/// <summary>
		/// Is the parameter required
		/// </summary>
		/// <param name="isRequired">is required</param>
		/// <returns>configuration object</returns>
		public IFluentMethodParameterConfiguration<T, TProp> IsRequired(bool isRequired = true)
		{
			methodInfo.IsRequired = isRequired;

			return this;
		}

		/// <summary>
		/// Name to use when importing parameter
		/// </summary>
		/// <param name="importName">Import name</param>
		/// <returns>configuration object</returns>
		public IFluentMethodParameterConfiguration<T, TProp> ImportName(string importName)
		{
			methodInfo.ImportName = importName;

			return this;
		}

		/// <summary>
		/// Using a specified value provider
		/// </summary>
		/// <param name="valueProvider">value provider</param>
		/// <returns>configuration object</returns>
		public IFluentMethodParameterConfiguration<T, TProp> UsingValueProvider(IExportValueProvider valueProvider)
		{
			methodInfo.ValueProvider = valueProvider;

			return this;
		}
	}

	#endregion
}