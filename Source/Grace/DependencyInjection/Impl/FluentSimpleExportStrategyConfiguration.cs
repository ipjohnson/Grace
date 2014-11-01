using System;
using System.Collections.Generic;
using Grace.DependencyInjection.Conditions;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl
{
	/// <summary>
	/// Configures a simple export strategy
	/// </summary>
	public class FluentSimpleExportStrategyConfiguration : IFluentSimpleExportStrategyConfiguration,
		IExportStrategyProvider
	{
		private readonly ConfigurableExportStrategy exportStrategy;

		/// <summary>
		/// default constructor
		/// </summary>
		/// <param name="exportStrategy">export strategy to configure</param>
		public FluentSimpleExportStrategyConfiguration(ConfigurableExportStrategy exportStrategy)
		{
			this.exportStrategy = exportStrategy;
		}

		/// <summary>
		/// Defines the priority to export at
		/// </summary>
		/// <param name="priority">priority for export</param>
		/// <returns>configuration object</returns>
		public IFluentSimpleExportStrategyConfiguration WithPriority(int priority)
		{
			exportStrategy.SetPriority(priority);

			return this;
		}

		/// <summary>
		/// Export under a particular key
		/// </summary>
		/// <param name="key">key to associate with export</param>
		/// <returns>configuration object</returns>
		public IFluentSimpleExportStrategyConfiguration WithKey(object key)
		{
			exportStrategy.SetKey(key);

			return this;
		}

		/// <summary>
		/// Export as a particular type
		/// </summary>
		/// <param name="exportType">type to export as</param>
		/// <returns>configuration object</returns>
		public IFluentSimpleExportStrategyConfiguration As(Type exportType)
		{
			exportStrategy.AddExportType(exportType);

			return this;
		}

		/// <summary>
		/// Export as a particular type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public IFluentSimpleExportStrategyConfiguration As<T>()
		{
			exportStrategy.AddExportType(typeof(T));

			return this;
		}

	    /// <summary>
	    /// Export this type as particular type under the specified key
	    /// </summary>
	    /// <typeparam name="T">export type</typeparam>
	    /// <typeparam name="TKey">type of key</typeparam>
	    /// <param name="key">key to export under</param>
	    /// <returns>configuration object</returns>
	    public IFluentSimpleExportStrategyConfiguration AsKeyed<T, TKey>(TKey key)
        {
            exportStrategy.AddKeyedExportType(typeof(T), key);

	        return this;
	    }

	    /// <summary>
	    /// Export this type as particular type under the specified key
	    /// </summary>
	    /// <param name="exportType">type to export under</param>
	    /// <param name="key">export key</param>
	    /// <returns>configuration object</returns>
	    public IFluentSimpleExportStrategyConfiguration AsKeyed(Type exportType, object key)
	    {
	        exportStrategy.AddKeyedExportType(exportType, key);

	        return this;
	    }

	    /// <summary>
		/// Defines which environment this export should be exported in
		/// </summary>
		/// <param name="environment"></param>
		/// <returns>configuration object</returns>
		public IFluentSimpleExportStrategyConfiguration InEnvironment(ExportEnvironment environment)
		{
			exportStrategy.SetEnvironment(environment);

			return this;
		}

		/// <summary>
		/// Export this type as a particular name
		/// </summary>
		/// <param name="name"></param>
		/// <returns>configuration object</returns>
		public IFluentSimpleExportStrategyConfiguration AsName(string name)
		{
			exportStrategy.AddExportName(name);

			return this;
		}

		/// <summary>
		/// Export will be treated as a singleton for the lifetime of the container
		/// </summary>
		/// <returns>configuration object</returns>
		public IFluentSimpleExportStrategyConfiguration AndSingleton()
		{
			exportStrategy.SetLifestyleContainer(new SingletonLifestyle());

			return this;
		}

		/// <summary>
		/// Export will be treated as a singleton for the lifetime of the scope
		/// </summary>
		/// <returns>configuration object</returns>
		public IFluentSimpleExportStrategyConfiguration AndSingletonPerScope()
		{
			exportStrategy.SetLifestyleContainer(new SingletonPerScopeLifestyle());

			return this;
		}

		/// <summary>
		/// Exports will be trated as a singleton using a weak reference
		/// </summary>
		/// <returns>configuration object</returns>
		public IFluentSimpleExportStrategyConfiguration AndWeakSingleton()
		{
			exportStrategy.SetLifestyleContainer(new WeakSingletonLifestyle());

			return this;
		}

		/// <summary>
		/// Mark the export as externally owned, doing so will absolve the container of having to call Dispose when done
		/// </summary>
		/// <returns>configuration object</returns>
		public IFluentSimpleExportStrategyConfiguration ExternallyOwned()
		{
			exportStrategy.SetExternallyOwned();

			return this;
		}

		/// <summary>
		/// Specify a custom Lifestyle container for export.
		/// </summary>
		/// <param name="lifestyle">Lifestyle container for the export</param>
		/// <returns>configuration object</returns>
		public IFluentSimpleExportStrategyConfiguration UsingLifestyle(ILifestyle lifestyle)
		{
			exportStrategy.SetLifestyleContainer(lifestyle);

			return this;
		}

		/// <summary>
		/// Adds a condition to the export
		/// </summary>
		/// <param name="conditionDelegate">export condition delegate</param>
		/// <returns>configuration object</returns>
		public IFluentSimpleExportStrategyConfiguration When(ExportConditionDelegate conditionDelegate)
		{
			exportStrategy.AddCondition(new WhenCondition(conditionDelegate));

			return this;
		}

		/// <summary>
		/// Adds a condition to the export
		/// </summary>
		/// <param name="conditionDelegate">export condition delegate</param>
		/// <returns>configuration object</returns>
		public IFluentSimpleExportStrategyConfiguration Unless(ExportConditionDelegate conditionDelegate)
		{
			exportStrategy.AddCondition(new UnlessCondition(conditionDelegate));

			return this;
		}

		/// <summary>
		/// Adds a condition to the export
		/// </summary>
		/// <param name="condition">condition for export</param>
		/// <returns>configuration object</returns>
		public IFluentSimpleExportStrategyConfiguration AndCondition(IExportCondition condition)
		{
			exportStrategy.AddCondition(condition);

			return this;
		}

		/// <summary>
		/// Adds metadata to an export
		/// </summary>
		/// <param name="metadataName">metadata name</param>
		/// <param name="metadataValue">metadata value</param>
		/// <returns>configuration object</returns>
		public IFluentSimpleExportStrategyConfiguration WithMetadata(string metadataName, object metadataValue)
		{
			exportStrategy.AddMetadata(metadataName, metadataValue);

			return this;
		}

		/// <summary>
		/// Allows you to add custom activation logic to process before the object is returned.
		/// </summary>
		/// <param name="enrichWithDelegate"></param>
		/// <returns>configuration object</returns>
		public IFluentSimpleExportStrategyConfiguration EnrichWith(EnrichWithDelegate enrichWithDelegate)
		{
			exportStrategy.EnrichWithDelegate(enrichWithDelegate);

			return this;
		}

		/// <summary>
		/// Provide a list of strategies
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IExportStrategy> ProvideStrategies()
		{
			yield return exportStrategy;
		}
	}
}