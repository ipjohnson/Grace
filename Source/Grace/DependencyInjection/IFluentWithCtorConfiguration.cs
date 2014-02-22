using JetBrains.Annotations;

namespace Grace.DependencyInjection
{
	/// <summary>
	/// Interface to configure a constructor parameter
	/// </summary>
	/// <typeparam name="TParam">type of parameter being resolved</typeparam>
	public interface IFluentWithCtorConfiguration<TParam> : IFluentExportStrategyConfiguration
	{
		/// <summary>
		/// Is the parameter required when resolving the type
		/// </summary>
		/// <param name="isRequired">is the parameter required</param>
		/// <returns>configuration object</returns>
		IFluentWithCtorConfiguration<TParam> IsRequired(bool isRequired = true);

		/// <summary>
		/// Name of the parameter to resolve
		/// </summary>
		/// <param name="name"></param>
		/// <returns>configuration object</returns>
		IFluentWithCtorConfiguration<TParam> Named([NotNull] string name);

		/// <summary>
		/// Name to use when resolving parameter
		/// </summary>
		/// <param name="importName"></param>
		/// <returns>configuration object</returns>
		IFluentWithCtorConfiguration<TParam> ImportName([NotNull] string importName);

		/// <summary>
		/// Applies a filter to be used when resolving a parameter constructor
		/// It will be called each time the parameter is resolved
		/// </summary>
		/// <param name="filter">filter delegate to be used when resolving parameter</param>
		/// <returns>configuration object</returns>
		IFluentWithCtorConfiguration<TParam> Consider([NotNull] ExportStrategyFilter filter);

		/// <summary>
		/// Provides a value for a constructor parameter
		/// </summary>
		/// <param name="valueProvider">value provider for parameter</param>
		/// <returns>configuration object</returns>
		IFluentWithCtorConfiguration<TParam> UsingValueProvider([NotNull] IExportValueProvider valueProvider);
	}

	/// <summary>
	/// Interface to configure a constructor parameter
	/// </summary>
	/// <typeparam name="TParam">type of parameter being resolved</typeparam>
	/// <typeparam name="T">type being exported</typeparam>
	public interface IFluentWithCtorConfiguration<T, TParam> : IFluentExportStrategyConfiguration<T>
	{
		/// <summary>
		/// Is the parameter required to resolve T
		/// </summary>
		/// <param name="isRequired">is the parameter required</param>
		/// <returns>configuration object</returns>
		IFluentWithCtorConfiguration<T, TParam> IsRequired(bool isRequired = true);

		/// <summary>
		/// Name of the parameter being configured
		/// </summary>
		/// <param name="name">Name of parameter</param>
		/// <returns>configuration object</returns>
		IFluentWithCtorConfiguration<T, TParam> Named([NotNull] string name);

		/// <summary>
		/// Name to use when resolving parameter
		/// </summary>
		/// <param name="importName">name to use when resolving parameter</param>
		/// <returns>configuration object</returns>
		IFluentWithCtorConfiguration<T, TParam> ImportName([NotNull] string importName);

		/// <summary>
		/// Applies a filter to be used when resolving the parameter
		/// The filter will be used each time this parameter is resolved
		/// </summary>
		/// <param name="filter">filter delegate to use when resolving parameter</param>
		/// <returns>configuration object</returns>
		IFluentWithCtorConfiguration<T, TParam> Consider([NotNull] ExportStrategyFilter filter);

		/// <summary>
		/// Value provider to use when resolving constructor parameter
		/// </summary>
		/// <param name="valueProvider"></param>
		/// <returns></returns>
		IFluentWithCtorConfiguration<T, TParam> UsingValueProvider([NotNull] IExportValueProvider valueProvider);
	}
}