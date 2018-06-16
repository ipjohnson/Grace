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
        /// <returns></returns>
        IFluentExportInstanceConfiguration<T> AllowNullReturn(bool allowNullReturn = true);

        /// <summary>
        /// Export as specific type
        /// </summary>
        /// <param name="type">type to export as</param>
        /// <returns></returns>
        IFluentExportInstanceConfiguration<T> As(Type type);

        /// <summary>
        /// Export as keyed interface
        /// </summary>
        /// <param name="type">export type</param>
        /// <param name="key">export key</param>
        /// <returns></returns>
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
        /// <returns></returns>
        IFluentExportInstanceConfiguration<T> AsKeyed<TExportType>(object key);

        /// <summary>
        /// Export as specific name
        /// </summary>
        /// <param name="name">export name</param>
        /// <returns></returns>
        IFluentExportInstanceConfiguration<T> AsName(string name);
        
        /// <summary>
        /// Mark an export as externally owned means the container will not track and dispose the instance
        /// </summary>
        /// <returns></returns>
        IFluentExportInstanceConfiguration<T> ExternallyOwned();

        /// <summary>
        /// Assign a lifestyle to this export
        /// </summary>
        ILifestylePicker<IFluentExportInstanceConfiguration<T>> Lifestyle { get; }

        /// <summary>
        /// Only export if delegate returns true
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        IFluentExportInstanceConfiguration<T> OnlyIf(Func<IExportRegistrationBlock, bool> filter);

        /// <summary>
        /// Export using a specific lifestyle
        /// </summary>
        /// <param name="lifestyle">lifestlye to use</param>
        /// <returns>configuration object</returns>
        [Obsolete("Use Lifestyle.Custom instead")]
        IFluentExportInstanceConfiguration<T> UsingLifestyle(ICompiledLifestyle lifestyle);

        /// <summary>
        /// Use export under specific conditions
        /// </summary>
        IWhenConditionConfiguration<IFluentExportInstanceConfiguration<T>> When { get; }

        /// <summary>
        /// Set priority for export
        /// </summary>
        /// <param name="priority"></param>
        /// <returns></returns>
        IFluentExportInstanceConfiguration<T> WithPriority(int priority);
    }
}
