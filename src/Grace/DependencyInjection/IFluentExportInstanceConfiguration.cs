using System;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// Configuration interface for export instance
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IFluentExportInstanceConfiguration<T>
    {
        /// <summary>
        /// This will turn off graces built in check for null return values on exports.
        /// By default an exception will be thrown if null is returned.
        /// </summary>
        /// <param name="allowNullReturn"></param>
        IFluentExportInstanceConfiguration<T> AllowNullReturn(bool allowNullReturn = true);

        /// <summary>
        /// Export as specific type
        /// </summary>
        /// <param name="type">type to export as</param>
        IFluentExportInstanceConfiguration<T> As(Type type);

        /// <summary>
        /// Export as keyed interface
        /// </summary>
        /// <param name="type">export type</param>
        /// <param name="key">export key</param>
        IFluentExportInstanceConfiguration<T> AsKeyed(Type type, object key);

        /// <summary>
        /// Export as a specific type
        /// </summary>
        /// <typeparam name="TInterface">type to export as</typeparam>
        /// <returns>configuration object</returns>
        IFluentExportInstanceConfiguration<T> As<TInterface>();

        /// <summary>
        /// Export as a specific keyed type
        /// </summary>
        /// <typeparam name="TExportType">type to export as</typeparam>
        /// <param name="key">key to export as</param>
        IFluentExportInstanceConfiguration<T> AsKeyed<TExportType>(object key);

        /// <summary>
        /// Export as specific name
        /// </summary>
        /// <param name="name">export name</param>
        IFluentExportInstanceConfiguration<T> AsName(string name);
        
        /// <summary>
        /// Mark an export as externally owned means the container will not track and dispose the instance
        /// </summary>
        IFluentExportInstanceConfiguration<T> ExternallyOwned();

        /// <summary>
        /// Assign a lifestyle to this export
        /// </summary>
        ILifestylePicker<IFluentExportInstanceConfiguration<T>> Lifestyle { get; }

        /// <summary>
        /// Only export if delegate returns true
        /// </summary>
        /// <param name="filter"></param>
        IFluentExportInstanceConfiguration<T> OnlyIf(Func<IExportRegistrationBlock, bool> filter);

        /// <summary>
        /// Use export under specific conditions
        /// </summary>
        IWhenConditionConfiguration<IFluentExportInstanceConfiguration<T>> When { get; }

        /// <summary>
        /// Set priority for export
        /// </summary>
        /// <param name="priority"></param>
        IFluentExportInstanceConfiguration<T> WithPriority(int priority);
    }
}
