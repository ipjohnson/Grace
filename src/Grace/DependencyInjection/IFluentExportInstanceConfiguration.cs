using System;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection
{
    public interface IFluentExportInstanceConfiguration<T>
    {
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
        /// <typeparam name="TKey">key type</typeparam>
        /// <param name="key">key to export as</param>
        /// <returns></returns>
        IFluentExportInstanceConfiguration<T> AsKeyed<TExportType, TKey>(TKey key);


        /// <summary>
        /// Export using a specific lifestyle
        /// </summary>
        /// <param name="lifestyle">lifestlye to use</param>
        /// <returns>configuration object</returns>
        IFluentExportInstanceConfiguration<T> UsingLifestyle(ICompiledLifestyle lifestyle);

        /// <summary>
        /// Assign a lifestyle to this export
        /// </summary>
        ILifestylePicker<IFluentExportInstanceConfiguration<T>> Lifestyle { get; }
    }
}
