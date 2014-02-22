using System;

namespace Grace.DependencyInjection
{
	public interface IFluentImportMethodConfiguration : IFluentExportStrategyConfiguration
	{
		IFluentMethodParameterConfiguration<TParam> WithParam<TParam>(Func<TParam> paramValueFunc = null);
	}

	public interface IFluentMethodParameterConfiguration<TParam> : IFluentImportMethodConfiguration
	{
		IFluentMethodParameterConfiguration<TParam> IsRequired(bool isRequired = true);

		IFluentMethodParameterConfiguration<TParam> ImportName(string importName);

		IFluentMethodParameterConfiguration<TParam> UsingValueProvider(IExportValueProvider valueProvider);
	}

	public interface IFluentImportMethodConfiguration<T> : IFluentExportStrategyConfiguration<T>
	{
		IFluentMethodParameterConfiguration<T, TParam> WithMethodParam<TParam>(Func<TParam> paramValueFunc = null);
	}

	public interface IFluentMethodParameterConfiguration<T, in TParam> : IFluentImportMethodConfiguration<T>
	{
		IFluentMethodParameterConfiguration<T, TParam> Named(string parameterName);

		IFluentMethodParameterConfiguration<T, TParam> IsRequired(bool isRequired = true);

		IFluentMethodParameterConfiguration<T, TParam> ImportName(string importName);

		IFluentMethodParameterConfiguration<T, TParam> UsingValueProvider(IExportValueProvider valueProvider);
	}
}