using System;
using Grace.DependencyInjection.Conditions;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// Configuration object for an export instance
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FluentExportInstanceConfiguration<T> : IFluentExportInstanceConfiguration<T>, IActivationStrategyProvider
    {
        private readonly IInstanceActivationStrategy _exportConfiguration;
        private readonly IExportRegistrationBlock _registrationBlock;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="exportConfiguration"></param>
        /// <param name="registrationBlock"></param>
        public FluentExportInstanceConfiguration(IInstanceActivationStrategy exportConfiguration, IExportRegistrationBlock registrationBlock)
        {
            _exportConfiguration = exportConfiguration;
            _registrationBlock = registrationBlock;
        }

        /// <summary>
        /// This will turn off graces built in check for null return values on exports.
        /// By default an exception will be thrown if null is returned.
        /// </summary>
        /// <param name="allowNullReturn"></param>
        /// <returns></returns>
        public IFluentExportInstanceConfiguration<T> AllowNullReturn(bool allowNullReturn = true)
        {
            _exportConfiguration.AllowNullReturn = allowNullReturn;

            return this;
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
        /// Export as specific name
        /// </summary>
        /// <param name="name">export name</param>
        /// <returns></returns>
        public IFluentExportInstanceConfiguration<T> AsName(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentException("Value cannot be null or empty.", nameof(name));

            _exportConfiguration.AddExportAsName(name);

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
        /// Only export if delegate returns true
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IFluentExportInstanceConfiguration<T> OnlyIf(Func<IExportRegistrationBlock, bool> filter)
        {
            if (!filter(_registrationBlock))
            {
                _registrationBlock.ClearExports(export => export == _exportConfiguration);
            }

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
        /// Set priority for export
        /// </summary>
        /// <param name="priority"></param>
        /// <returns></returns>
        public IFluentExportInstanceConfiguration<T> WithPriority(int priority)
        {
            _exportConfiguration.Priority = priority;

            return this;
        }

        /// <summary>
        /// Assign a lifestyle to this export
        /// </summary>
        public ILifestylePicker<IFluentExportInstanceConfiguration<T>> Lifestyle => 
            new LifestylePicker<IFluentExportInstanceConfiguration<T>>(this, lifestyle => _exportConfiguration.Lifestyle = lifestyle);

        /// <summary>
        /// Get stragey from configuration
        /// </summary>
        /// <returns></returns>
        public IActivationStrategy GetStrategy()
        {
            return _exportConfiguration;
        }
    }
}
