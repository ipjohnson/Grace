using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Grace.DependencyInjection.Impl.Expressions;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// Interface for configuring a non generic type
    /// </summary>
    public interface IFluentExportStrategyConfiguration
    {
        /// <summary>
        /// Export as a specific type
        /// </summary>
        /// <param name="type">type to export as</param>
        /// <returns>configuraiton object</returns>
        IFluentExportStrategyConfiguration As(Type type);

        /// <summary>
        /// Export as keyed interface
        /// </summary>
        /// <param name="type">type to export as</param>
        /// <param name="key">key to export under</param>
        /// <returns></returns>
        IFluentExportStrategyConfiguration AsKeyed(Type type, object key);

        /// <summary>
        /// Export as Name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IFluentExportStrategyConfiguration AsName(string name);

        /// <summary>
        /// Export by interfaces
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        IFluentExportStrategyConfiguration ByInterfaces(Func<Type, bool> filter = null);

        /// <summary>
        /// Mark the export as externally owned so the container does not track for disposal
        /// </summary>
        /// <returns>configuraiton object</returns>
        IFluentExportStrategyConfiguration ExternallyOwned();

        /// <summary>
        /// Use specific constructor for use
        /// </summary>
        /// <param name="constructorInfo">constructor to use</param>
        /// <returns></returns>
        IFluentExportStrategyConfiguration ImportConstructor(ConstructorInfo constructorInfo);

        /// <summary>
        /// Specify the constructor selection algorithm
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        IFluentExportStrategyConfiguration ImportConstructorSelection(IConstructorExpressionCreator method);

        /// <summary>
        /// Import public members Fields, Properties, and Methods (not done by default) 
        /// </summary>
        /// <param name="selector">selector method, can be null</param>
        /// <param name="includeMethods">import all public methods that have parameters, false by default</param>
        /// <returns>configuraiton object</returns>
        IFluentExportStrategyConfiguration ImportMembers(Func<MemberInfo, bool> selector = null, bool includeMethods = false);

        /// <summary>
        /// Import property by name
        /// </summary>
        /// <param name="propertyName">property name</param>
        /// <returns>configuration object</returns>
        IFluentImportPropertyConfiguration ImportProperty(string propertyName);

        /// <summary>
        /// Apply a lifestlye to export strategy
        /// </summary>
        ILifestylePicker<IFluentExportStrategyConfiguration> Lifestyle { get; }

        /// <summary>
        /// Export only if function returns true
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        IFluentExportStrategyConfiguration OnlyIf(Func<IExportRegistrationBlock, bool> filter);

        /// <summary>
        /// Process attribute on type as well as fluent interface
        /// </summary>
        /// <returns></returns>
        IFluentExportStrategyConfiguration ProcessAttributes();

        /// <summary>
        /// Assign a custom lifestyle to an export
        /// </summary>
        /// <param name="lifestyle"></param>
        /// <returns>configuraiton object</returns>
        IFluentExportStrategyConfiguration UsingLifestyle(ICompiledLifestyle lifestyle);

        /// <summary>
        /// Apply a condition on when to use strategy
        /// </summary>
        IWhenConditionConfiguration<IFluentExportStrategyConfiguration> When { get; }

        /// <summary>
        /// Configure constructor parameter
        /// </summary>
        /// <param name="parameterType">parameter type</param>
        /// <returns></returns>
        IFluentWithCtorConfiguration WithCtorParam(Type parameterType = null);

        /// <summary>
        /// Configure constructor parameter
        /// </summary>
        /// <typeparam name="TParam"></typeparam>
        /// <param name="paramFunc"></param>
        /// <returns></returns>
        IFluentWithCtorConfiguration<TParam> WithCtorParam<TParam>(Func<TParam> paramFunc = null);

        /// <summary>
        /// Export with specific metadata
        /// </summary>
        /// <param name="key">metadata key</param>
        /// <param name="value">metadata value</param>
        /// <returns>configuraiton object</returns>
        IFluentExportStrategyConfiguration WithMetadata(object key, object value);

        /// <summary>
        /// Set the priority for the export
        /// </summary>
        /// <param name="priority"></param>
        /// <returns></returns>
        IFluentExportStrategyConfiguration WithPriority(int priority);

        /// <summary>
        /// Defines a custom scope when creating instance
        /// </summary>
        /// <param name="customscope"></param>
        /// <returns></returns>
        IFluentExportStrategyConfiguration DefinesNamedScope(string customscope);
    }

    /// <summary>
    /// Represents a class that configures an export
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IFluentExportStrategyConfiguration<T>
    {
        /// <summary>
        /// Mark a particular Action() as the activation action
        /// </summary>
        /// <param name="activationMethod"></param>
        /// <returns></returns>
        IFluentExportStrategyConfiguration<T> ActivationMethod(Expression<Action<T>> activationMethod);

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
        /// Export as keyed type
        /// </summary>
        /// <param name="type">export type</param>
        /// <param name="key">key</param>
        /// <returns></returns>
        IFluentExportStrategyConfiguration<T> AsKeyed(Type type, object key);

        /// <summary>
        /// Export as a keyed type
        /// </summary>
        /// <typeparam name="TInterface">export type</typeparam>
        /// <param name="key">key to export under</param>
        /// <returns>configuration object</returns>
        IFluentExportStrategyConfiguration<T> AsKeyed<TInterface>(object key);

        /// <summary>
        /// Export as specific name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IFluentExportStrategyConfiguration<T> AsName(string name);

        /// <summary>
        /// Export the type by the interfaces it implements
        /// </summary>
        /// <returns>configuration object</returns>
        IFluentExportStrategyConfiguration<T> ByInterfaces(Func<Type, bool> filter = null);


        /// <summary>
        /// Creates a new scope and then resolves decorators inside of it.
        /// </summary>
        /// <param name="namedScope"></param>
        /// <returns></returns>
        IFluentExportStrategyConfiguration<T> DefinesNamedScope(string namedScope);

        /// <summary>
        /// You can provide a cleanup method to be called 
        /// </summary>
        /// <param name="disposalCleanupDelegate">action to call when disposing</param>
        /// <returns>configuration object</returns>
        IFluentExportStrategyConfiguration<T> DisposalCleanupDelegate(Action<T> disposalCleanupDelegate);

        /// <summary>
        /// Enrich with delegate
        /// </summary>
        /// <param name="enrichmentDelegate">enrichment delegate</param>
        /// <returns></returns>
        IFluentExportStrategyConfiguration<T> EnrichWithDelegate(
            Func<IExportLocatorScope, StaticInjectionContext, T, T> enrichmentDelegate);

        /// <summary>
        /// Export a public member of the type (property, field or method with return value)
        /// </summary>
        /// <typeparam name="TValue">type to export</typeparam>
        /// <param name="memberExpression">member expression</param>
        /// <returns></returns>
        IFluentExportMemberConfiguration<T> ExportMember<TValue>(Expression<Func<T, TValue>> memberExpression);

        /// <summary>
        /// Mark an export as externally owned means the container will not track and dispose the instance
        /// </summary>
        /// <returns>configuration object</returns>
        IFluentExportStrategyConfiguration<T> ExternallyOwned();
        
        /// <summary>
        /// This method allows you to specify which constructor to use ( () => new MyTypeName("Specific", "Constructor") )
        /// </summary>
        /// <param name="constructorExpression">constructor expression ( () => new MyTypeName("Specific", "Constructor") )</param>
        /// <returns></returns>
        IFluentExportStrategyConfiguration<T> ImportConstructor(Expression<Func<T>> constructorExpression);

        /// <summary>
        /// Use specific constructor for use
        /// </summary>
        /// <param name="constructorInfo">constructor to use</param>
        /// <returns></returns>
        IFluentExportStrategyConfiguration<T> ImportConstructor(ConstructorInfo constructorInfo);

        /// <summary>
        /// Use a specific constructor selection method
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        IFluentExportStrategyConfiguration<T> ImportConstructorSelection(IConstructorExpressionCreator method);

        /// <summary>
        /// Mark specific members to be injected
        /// </summary>
        /// <param name="selector">select specific members, if null all public members will be injected</param>
        /// <param name="injectMethod"></param>
        /// <returns>configuration object</returns>
        IFluentExportStrategyConfiguration<T> ImportMembers(Func<MemberInfo, bool> selector = null, bool injectMethod = false);

        /// <summary>
        /// Import a specific property
        /// </summary>
        /// <typeparam name="TProp"></typeparam>
        /// <param name="property">property expression</param>
        /// <returns>configuration object</returns>
        IFluentImportPropertyConfiguration<T, TProp> ImportProperty<TProp>(Expression<Func<T, TProp>> property);

        /// <summary>
        /// Import a specific method on the type
        /// </summary>
        /// <param name="method">method to import</param>
        /// <returns></returns>
        IFluentExportStrategyConfiguration<T> ImportMethod(Expression<Action<T>> method);

        /// <summary>
        /// Assign a lifestyle to this export
        /// </summary>
        ILifestylePicker<IFluentExportStrategyConfiguration<T>> Lifestyle { get; }

        /// <summary>
        /// Export only if function returns true
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        IFluentExportStrategyConfiguration<T> OnlyIf(Func<IExportRegistrationBlock, bool> filter);

        /// <summary>
        /// Process attribute on type as well as fluent interface
        /// </summary>
        /// <returns></returns>
        IFluentExportStrategyConfiguration<T> ProcessAttributes();

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
        /// Import a collection allowing you to specify a filter and a sort order
        /// </summary>
        /// <typeparam name="TParam"></typeparam>
        /// <typeparam name="TItem"></typeparam>
        /// <returns></returns>
        IFluentWithCtorCollectionConfiguration<T, TItem> WithCtorCollectionParam<TParam, TItem>()
            where TParam : IEnumerable<TItem>;

        /// <summary>
        /// Add a specific value for a particuar parameter in the constructor
        /// </summary>
        /// <typeparam name="TParam">type of parameter</typeparam>
        /// <typeparam name="TArg1"></typeparam>
        /// <param name="paramValue">Func(IInjectionScope, IInjectionContext, T) value for the parameter</param>
        /// <returns>configuration object</returns>
        IFluentWithCtorConfiguration<T, TParam> WithCtorParam<TArg1, TParam>(Func<TArg1, TParam> paramValue);
        
        /// <summary>
        /// Add a specific value for a particuar parameter in the constructor
        /// </summary>
        /// <typeparam name="TParam">type of parameter</typeparam>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <param name="paramValue">Func(IInjectionScope, IInjectionContext, T) value for the parameter</param>
        /// <returns>configuration object</returns>
        IFluentWithCtorConfiguration<T, TParam> WithCtorParam<TArg1, TArg2, TParam>(Func<TArg1, TArg2, TParam> paramValue);

        /// <summary>
        /// Add a specific value for a particuar parameter in the constructor
        /// </summary>
        /// <typeparam name="TParam">type of parameter</typeparam>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <typeparam name="TArg3"></typeparam>
        /// <param name="paramValue">Func(IInjectionScope, IInjectionContext, T) value for the parameter</param>
        /// <returns>configuration object</returns>
        IFluentWithCtorConfiguration<T, TParam> WithCtorParam<TArg1, TArg2, TArg3, TParam>(Func<TArg1, TArg2, TArg3, TParam> paramValue);

        /// <summary>
        /// Add a specific value for a particuar parameter in the constructor
        /// </summary>
        /// <typeparam name="TParam">type of parameter</typeparam>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <typeparam name="TArg3"></typeparam>
        /// <typeparam name="TArg4"></typeparam>
        /// <param name="paramValue">Func(IInjectionScope, IInjectionContext, T) value for the parameter</param>
        /// <returns>configuration object</returns>
        IFluentWithCtorConfiguration<T, TParam> WithCtorParam<TArg1, TArg2, TArg3, TArg4, TParam>(Func<TArg1, TArg2, TArg3, TArg4, TParam> paramValue);

        /// <summary>
        /// Add a specific value for a particuar parameter in the constructor
        /// </summary>
        /// <typeparam name="TParam">type of parameter</typeparam>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <typeparam name="TArg3"></typeparam>
        /// <typeparam name="TArg4"></typeparam>
        /// <typeparam name="TArg5"></typeparam>
        /// <param name="paramValue">Func(IInjectionScope, IInjectionContext, T) value for the parameter</param>
        /// <returns>configuration object</returns>
        IFluentWithCtorConfiguration<T, TParam> WithCtorParam<TArg1, TArg2, TArg3, TArg4, TArg5, TParam>(Func<TArg1, TArg2, TArg3, TArg4, TArg5, TParam> paramValue);

        /// <summary>
        /// Adds metadata to an export
        /// </summary>
        /// <param name="key">metadata key</param>
        /// <param name="value">metadata value</param>
        /// <returns>configuration object</returns>
        IFluentExportStrategyConfiguration<T> WithMetadata(object key, object value);

        /// <summary>
        /// Set the priority for the export
        /// </summary>
        /// <param name="priority"></param>
        /// <returns></returns>
        IFluentExportStrategyConfiguration<T> WithPriority(int priority);
    }
}
