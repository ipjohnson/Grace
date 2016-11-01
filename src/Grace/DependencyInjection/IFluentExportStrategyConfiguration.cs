using Grace.DependencyInjection.Lifestyle;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Grace.DependencyInjection
{
    public interface IFluentExportStrategyConfiguration
    {
        IFluentExportStrategyConfiguration As(Type type);

        IFluentExportStrategyConfiguration ExternallyOwned();

        IFluentExportStrategyConfiguration ImportMembers(Func<MemberInfo, bool> selector = null);
        
        ILifestylePicker<IFluentExportStrategyConfiguration> Lifestyle { get; }

        IFluentExportStrategyConfiguration UsingLifestyle(ICompiledLifestyle lifestyle);

        IWhenConditionConfiguration<IFluentExportStrategyConfiguration> When { get; }

        IFluentExportStrategyConfiguration WithMetadata(object key, object value);
    }

    /// <summary>
    /// Represents a class that configures an export
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IFluentExportStrategyConfiguration<T>
    {
        /// <summary>
        /// Apply an action to the export just after construction
        /// </summary>
        /// <param name="applyAction">action to apply to export upon construction</param>
        /// <returns>configuration object</returns>
        IFluentExportStrategyConfiguration<T> Apply(Action<T> applyAction);

        /// <summary>
        /// Export as a specific type
        /// </summary>
        /// <param name="type">type to export as</param>
        /// <returns></returns>
        IFluentExportStrategyConfiguration<T> As(Type type);

        /// <summary>
        /// Export as a particular type
        /// </summary>
        /// <typeparam name="TInterface">type to export as</typeparam>
        /// <returns>configuration object</returns>
        IFluentExportStrategyConfiguration<T> As<TInterface>();

        /// <summary>
        /// Export as a keyed type
        /// </summary>
        /// <typeparam name="TInterface">export type</typeparam>
        /// <param name="key">key to export under</param>
        /// <returns>configuration object</returns>
        IFluentExportStrategyConfiguration<T> AsKeyed<TInterface>(object key);

        /// <summary>
        /// Export the type by the interfaces it implements
        /// </summary>
        /// <returns></returns>
        IFluentExportStrategyConfiguration<T> ByInterfaces(Func<Type, bool> filter = null);
        
        /// <summary>
        /// You can provide a cleanup method to be called 
        /// </summary>
        /// <param name="disposalCleanupDelegate"></param>
        /// <returns></returns>
        IFluentExportStrategyConfiguration<T> DisposalCleanupDelegate(Action<T> disposalCleanupDelegate);

        /// <summary>
        /// Mark an export as externally owned means the container will not track and dispose the instance
        /// </summary>
        /// <returns></returns>
        IFluentExportStrategyConfiguration<T> ExternallyOwned();

        /// <summary>
        /// Mark specific members to be injected
        /// </summary>
        /// <param name="selector">select specific members, if null all public members will be injected</param>
        /// <returns>configuration object</returns>
        IFluentExportStrategyConfiguration<T> ImportMembers(Func<MemberInfo, bool> selector = null);

        /// <summary>
        /// Import a specific property
        /// </summary>
        /// <typeparam name="TProp"></typeparam>
        /// <param name="property">property expression</param>
        /// <returns></returns>
        IFluentImportPropertyConfiguration<T, TProp> ImportProperty<TProp>(Expression<Func<T, TProp>> property);

        /// <summary>
        /// Assign a lifestyle to this export
        /// </summary>
        ILifestylePicker<IFluentExportStrategyConfiguration<T>> Lifestyle { get; }

        /// <summary>
        /// Export using a specific lifestyle
        /// </summary>
        /// <param name="lifestyle">lifestlye to use</param>
        /// <returns>configuration object</returns>
        IFluentExportStrategyConfiguration<T> UsingLifestyle(ICompiledLifestyle lifestyle);

        /// <summary>
        /// Add a condition to when this export can be used
        /// </summary>
        IWhenConditionConfiguration<IFluentExportStrategyConfiguration<T>> When { get; }

        /// <summary>
        /// Add a specific value for a particuar parameter in the constructor
        /// </summary>
        /// <typeparam name="TParam">type of parameter</typeparam>
        /// <param name="paramValue">Func(T) value for the parameter</param>
        /// <returns>configuration object</returns>
        IFluentWithCtorConfiguration<T, TParam> WithCtorParam<TParam>(Func<TParam> paramValue = null);

        /// <summary>
        /// Add a specific value for a particuar parameter in the constructor
        /// </summary>
        /// <typeparam name="TParam">type of parameter</typeparam>
        /// <param name="paramValue">Func(IInjectionScope, IInjectionContext, T) value for the parameter</param>
        /// <returns>configuration object</returns>
        IFluentWithCtorConfiguration<T, TParam> WithCtorParam<TParam>(Func<IExportLocatorScope, StaticInjectionContext, IInjectionContext, TParam> paramValue);

        /// <summary>
        /// Adds metadata to an export
        /// </summary>
        /// <param name="key">metadata key</param>
        /// <param name="value">metadata value</param>
        /// <returns>configuration object</returns>
        IFluentExportStrategyConfiguration<T> WithMetadata(object key, object value);

    }
}
