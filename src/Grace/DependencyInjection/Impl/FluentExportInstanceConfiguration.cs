using System;
using Grace.DependencyInjection.Conditions;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// Configuration object for an export instance
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FluentExportInstanceConfiguration<T> : IFluentExportInstanceConfiguration<T>
    {
        private readonly IConfigurableActivationStrategy _exportConfiguration;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="exportConfiguration"></param>
        public FluentExportInstanceConfiguration(IConfigurableActivationStrategy exportConfiguration)
        {
            _exportConfiguration = exportConfiguration;
        }

        /// <summary>
        /// Export as specific type
        /// </summary>
        /// <param name="type">type to export as</param>
        /// <returns></returns>
        public IFluentExportInstanceConfiguration<T> As(Type type)
        {
            _exportConfiguration.AddExportAs(type);

            return this;
        }

        /// <summary>
        /// Export as keyed interface
        /// </summary>
        /// <param name="type">export type</param>
        /// <param name="key">export key</param>
        /// <returns></returns>
        public IFluentExportInstanceConfiguration<T> AsKeyed(Type type, object key)
        {
            _exportConfiguration.AddExportAsKeyed(type, key);

            return this;
        }

        /// <summary>
        /// Export as a specific type
        /// </summary>
        /// <typeparam name="TInterface">type to export as</typeparam>
        /// <returns>configuration object</returns>
        public IFluentExportInstanceConfiguration<T> As<TInterface>()
        {
            _exportConfiguration.AddExportAs(typeof(TInterface));

            return this;
        }

        /// <summary>
        /// Export as a specific keyed type
        /// </summary>
        /// <typeparam name="TExportType">type to export as</typeparam>
        /// <param name="key">key to export as</param>
        /// <returns></returns>
        public IFluentExportInstanceConfiguration<T> AsKeyed<TExportType>(object key)
        {
            _exportConfiguration.AddExportAsKeyed(typeof(TExportType), key);

            return this;
        }

        /// <summary>
        /// Mark an export as externally owned means the container will not track and dispose the instance
        /// </summary>
        /// <returns></returns>
        public IFluentExportInstanceConfiguration<T> ExternallyOwned()
        {
            _exportConfiguration.ExternallyOwned = true;

            return this;
        }

        /// <summary>
        /// Export using a specific lifestyle
        /// </summary>
        /// <param name="lifestyle">lifestlye to use</param>
        /// <returns>configuration object</returns>
        public IFluentExportInstanceConfiguration<T> UsingLifestyle(ICompiledLifestyle lifestyle)
        {
            _exportConfiguration.Lifestyle = lifestyle;

            return this;
        }

        /// <summary>
        /// Use export under specific conditions
        /// </summary>
        public IWhenConditionConfiguration<IFluentExportInstanceConfiguration<T>> When
            => new WhenConditionConfiguration<IFluentExportInstanceConfiguration<T>>(condition => _exportConfiguration.AddCondition(condition), this);

        /// <summary>
        /// Assign a lifestyle to this export
        /// </summary>
        public ILifestylePicker<IFluentExportInstanceConfiguration<T>> Lifestyle => 
            new LifestylePicker<IFluentExportInstanceConfiguration<T>>(this, lifestyle => _exportConfiguration.Lifestyle = lifestyle);
    }
}
