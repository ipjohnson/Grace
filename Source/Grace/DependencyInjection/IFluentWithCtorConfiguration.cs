using System;
using JetBrains.Annotations;

namespace Grace.DependencyInjection
{
	/// <summary>
	/// Interface to configure a constructor parameter
	/// </summary>
	public interface IFluentWithCtorConfiguration : IFluentExportStrategyConfiguration
	{
		/// <summary>
		/// Applies a filter to be used when resolving a parameter constructor
		/// It will be called each time the parameter is resolved
		/// </summary>
		/// <param name="filter">filter delegate to be used when resolving parameter</param>
		/// <returns>configuration object</returns>
		IFluentWithCtorConfiguration Consider([NotNull] ExportStrategyFilter filter);

		/// <summary>
		/// Name to use when resolving parameter
		/// </summary>
		/// <param name="importName"></param>
		/// <returns>configuration object</returns>
		IFluentWithCtorConfiguration ImportName([NotNull] string importName);

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
        /// Locate with a particular key using a provided func
        /// </summary>
        /// <param name="locateKeyFunc">locate key func</param>
        /// <returns>configuration object</returns>
	    IFluentWithCtorConfiguration LocateWithKeyProvider(
	        Func<IInjectionScope, IInjectionContext, Type, object> locateKeyFunc);

        /// <summary>
        /// Locate with a particular key provider
        /// </summary>
        /// <param name="keyProvider">key provder</param>
        /// <returns>configuration object</returns>
	    IFluentWithCtorConfiguration LocateWithKeyProvider(ILocateKeyValueProvider keyProvider);

		/// <summary>
		/// Name of the parameter to resolve
		/// </summary>
		/// <param name="name"></param>
		/// <returns>configuration object</returns>
		IFluentWithCtorConfiguration Named([NotNull] string name);

		/// <summary>
		/// Provides a value for a constructor parameter
		/// </summary>
		/// <param name="valueProvider">value provider for parameter</param>
		/// <returns>configuration object</returns>
		IFluentWithCtorConfiguration UsingValueProvider([NotNull] IExportValueProvider valueProvider);
	}

	/// <summary>
	/// Interface to configure a constructor parameter
	/// </summary>
	/// <typeparam name="T">type being exported</typeparam>
	public interface IFluentWithCtorConfiguration<T> : IFluentExportStrategyConfiguration<T>
	{
		/// <summary>
		/// Applies a filter to be used when resolving the parameter
		/// The filter will be used each time this parameter is resolved
		/// </summary>
		/// <param name="filter">filter delegate to use when resolving parameter</param>
		/// <returns>configuration object</returns>
		IFluentWithCtorConfiguration<T> Consider([NotNull] ExportStrategyFilter filter);

		/// <summary>
		/// Name to use when resolving parameter
		/// </summary>
		/// <param name="importName">name to use when resolving parameter</param>
		/// <returns>configuration object</returns>
		IFluentWithCtorConfiguration<T> ImportName([NotNull] string importName);

		/// <summary>
		/// Is the parameter required to resolve T
		/// </summary>
		/// <param name="isRequired">is the parameter required</param>
		/// <returns>configuration object</returns>
		IFluentWithCtorConfiguration<T> IsRequired(bool isRequired = true);

		/// <summary>
		/// Locate with a particular key
		/// </summary>
		/// <param name="locateKey">locate key</param>
		/// <returns>configuration object</returns>
		IFluentWithCtorConfiguration<T> LocateWithKey(object locateKey);

        /// <summary>
        /// Locate with a particular key using a provided func
        /// </summary>
        /// <param name="locateKeyFunc">locate key func</param>
        /// <returns>configuration object</returns>
        IFluentWithCtorConfiguration<T> LocateWithKeyProvider(
            Func<IInjectionScope, IInjectionContext, Type, object> locateKeyFunc);

	    /// <summary>
	    /// Locate with a particular key provider
	    /// </summary>
	    /// <param name="keyProvider">key provder</param>
	    /// <returns>configuration object</returns>
	    IFluentWithCtorConfiguration<T> LocateWithKeyProvider(ILocateKeyValueProvider keyProvider);

		/// <summary>
		/// Name of the parameter being configured
		/// </summary>
		/// <param name="name">Name of parameter</param>
		/// <returns>configuration object</returns>
		IFluentWithCtorConfiguration<T> Named([NotNull] string name);

		/// <summary>
		/// Value provider to use when resolving constructor parameter
		/// </summary>
		/// <param name="valueProvider"></param>
		/// <returns></returns>
		IFluentWithCtorConfiguration<T> UsingValueProvider([NotNull] IExportValueProvider valueProvider);
	}
}