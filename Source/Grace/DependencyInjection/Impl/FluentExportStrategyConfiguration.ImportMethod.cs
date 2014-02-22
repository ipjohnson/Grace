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

		protected void ProcessCurrentImportMethodInfo()
		{
			if (currentImportMethodInfo != null)
			{
				exportStrategy.ImportMethod(currentImportMethodInfo);

				currentImportMethodInfo = null;
			}
		}
	}

	public class FluentImportMethodConfiguration : FluentBaseExportConfiguration, IFluentImportMethodConfiguration
	{
		private ImportMethodInfo methodInfo;

		public FluentImportMethodConfiguration(ImportMethodInfo methodInfo, IFluentExportStrategyConfiguration strategy)
			: base(strategy)
		{
		}

		public IFluentMethodParameterConfiguration<TParam> WithParam<TParam>(Func<TParam> paramValueFunc = null)
		{
			throw new NotImplementedException();
		}
	}

	public class FluentMethodParameterConfiguration<TParam> : FluentBaseExportConfiguration,
		IFluentMethodParameterConfiguration<TParam>
	{
		public FluentMethodParameterConfiguration(IFluentExportStrategyConfiguration strategy)
			: base(strategy)
		{
		}

		public IFluentMethodParameterConfiguration<TParam1> WithParam<TParam1>(Func<TParam1> paramValueFunc = null)
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

		protected void ProcessCurrentImportMethodInfo()
		{
			if (currentImportMethodInfo != null)
			{
				exportStrategy.ImportMethod(currentImportMethodInfo);

				currentImportMethodInfo = null;
			}
		}
	}

	public class FluentImportMethodConfiguration<T> : FluentBaseExportConfiguration<T>, IFluentImportMethodConfiguration<T>
	{
		private readonly ImportMethodInfo methodInfo;

		public FluentImportMethodConfiguration(ImportMethodInfo methodInfo, IFluentExportStrategyConfiguration<T> strategy)
			: base(strategy)
		{
			this.methodInfo = methodInfo;
		}

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

	public class FluentMethodParameterConfiguration<T, TProp> : FluentBaseExportConfiguration<T>,
		IFluentMethodParameterConfiguration<T, TProp>
	{
		private readonly MethodParamInfo methodInfo;
		private readonly FluentImportMethodConfiguration<T> importMethodConfiguration;

		public FluentMethodParameterConfiguration(MethodParamInfo methodInfo,
			FluentImportMethodConfiguration<T> importMethodConfiguration)
			: base(importMethodConfiguration)
		{
			this.methodInfo = methodInfo;
			this.importMethodConfiguration = importMethodConfiguration;
		}

		public IFluentMethodParameterConfiguration<T, TParam> WithMethodParam<TParam>(Func<TParam> paramValueFunc = null)
		{
			return importMethodConfiguration.WithMethodParam(paramValueFunc);
		}

		public IFluentMethodParameterConfiguration<T, TProp> Named(string parameterName)
		{
			methodInfo.ParameterName = parameterName;

			return this;
		}

		public IFluentMethodParameterConfiguration<T, TProp> IsRequired(bool isRequired = true)
		{
			methodInfo.IsRequired = isRequired;

			return this;
		}

		public IFluentMethodParameterConfiguration<T, TProp> ImportName(string importName)
		{
			methodInfo.ImportName = importName;

			return this;
		}

		public IFluentMethodParameterConfiguration<T, TProp> UsingValueProvider(IExportValueProvider valueProvider)
		{
			methodInfo.ValueProvider = valueProvider;

			return this;
		}
	}

	#endregion
}