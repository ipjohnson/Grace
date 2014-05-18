using System;

namespace Grace.DependencyInjection
{
	/// <summary>
	/// Configuration interface to configure an importing a method
	/// </summary>
	public interface IFluentImportMethodConfiguration : IFluentExportStrategyConfiguration
	{
		/// <summary>
		/// Import the methods parameters after consrtuction instead of the default before
		/// </summary>
		/// <returns>configuration object</returns>
		IFluentImportMethodConfiguration AfterConstruction();

		/// <summary>
		/// specify how to import a particular parameter
		/// </summary>
		/// <typeparam name="TParam">parameter type</typeparam>
		/// <param name="paramValueFunc">value func</param>
		/// <returns>configuration object</returns>
		IFluentMethodParameterConfiguration<TParam> WithParam<TParam>(Func<TParam> paramValueFunc = null);
	}

	/// <summary>
	/// method parameter configuration object
	/// </summary>
	/// <typeparam name="TParam"></typeparam>
	public interface IFluentMethodParameterConfiguration<TParam> : IFluentImportMethodConfiguration
	{
		/// <summary>
		/// Import the parameter after construction
		/// </summary>
		/// <returns></returns>
		IFluentMethodParameterConfiguration<TParam> ImportParameterAfterConstruction();

		/// <summary>
		/// Is the parameter required
		/// </summary>
		/// <param name="isRequired">is required, true by default</param>
		/// <returns>configuration object</returns>
		IFluentMethodParameterConfiguration<TParam> IsRequired(bool isRequired = true);

		/// <summary>
		/// Name to use when importing
		/// </summary>
		/// <param name="importName">import name</param>
		/// <returns>configuration object</returns>
		IFluentMethodParameterConfiguration<TParam> ImportName(string importName);

		/// <summary>
		/// Specify a value provider for this parameter
		/// </summary>
		/// <param name="valueProvider">value provider</param>
		/// <returns>configuration object</returns>
		IFluentMethodParameterConfiguration<TParam> UsingValueProvider(IExportValueProvider valueProvider);
	}

	/// <summary>
	/// Configuration object for importing a method
	/// </summary>
	/// <typeparam name="T">type being exported</typeparam>
	public interface IFluentImportMethodConfiguration<T> : IFluentExportStrategyConfiguration<T>
	{
		/// <summary>
		/// specify how to import a particular method parameter
		/// </summary>
		/// <typeparam name="TParam">parameter type</typeparam>
		/// <param name="paramValueFunc">value provider func</param>
		/// <returns>configuration object</returns>
		IFluentMethodParameterConfiguration<T, TParam> WithMethodParam<TParam>(Func<TParam> paramValueFunc = null);
	}

	/// <summary>
	/// method parameter configuration interface
	/// </summary>
	/// <typeparam name="T">type being exported</typeparam>
	/// <typeparam name="TParam">parameter type being exported</typeparam>
	public interface IFluentMethodParameterConfiguration<T, in TParam> : IFluentImportMethodConfiguration<T>
	{
		/// <summary>
		/// parameter name
		/// </summary>
		/// <param name="parameterName">parameter name</param>
		/// <returns>configuration object</returns>
		IFluentMethodParameterConfiguration<T, TParam> Named(string parameterName);

		/// <summary>
		/// Is the parameter required
		/// </summary>
		/// <param name="isRequired">is required</param>
		/// <returns>configuration object</returns>
		IFluentMethodParameterConfiguration<T, TParam> IsRequired(bool isRequired = true);

		/// <summary>
		/// Name to use when importing parameter
		/// </summary>
		/// <param name="importName">import name</param>
		/// <returns>configuration object</returns>
		IFluentMethodParameterConfiguration<T, TParam> ImportName(string importName);

		/// <summary>
		/// Value provider for method parameter
		/// </summary>
		/// <param name="valueProvider">value provider</param>
		/// <returns>configuration object</returns>
		IFluentMethodParameterConfiguration<T, TParam> UsingValueProvider(IExportValueProvider valueProvider);
	}
}