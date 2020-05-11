using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Grace.DependencyInjection.Impl.Expressions;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// forwards strategy configuration
    /// </summary>
    public abstract class ProxyFluentExportStrategyConfiguration : IFluentExportStrategyConfiguration
    {
        private readonly IFluentExportStrategyConfiguration _strategy;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="strategy"></param>
        protected ProxyFluentExportStrategyConfiguration(IFluentExportStrategyConfiguration strategy)
        {
            _strategy = strategy;
        }

        /// <summary>
        /// Export as a specific type
        /// </summary>
        /// <param name="type">type to export as</param>
        /// <returns>configuraiton object</returns>
        public IFluentExportStrategyConfiguration As(Type type)
        {
            return _strategy.As(type);
        }

        /// <summary>
        /// Export as keyed interface
        /// </summary>
        /// <param name="type">type to export as</param>
        /// <param name="key">key to export under</param>
        /// <returns></returns>
        public IFluentExportStrategyConfiguration AsKeyed(Type type, object key)
        {
            return _strategy.AsKeyed(type, key);
        }

        /// <summary>
        /// Export as Name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IFluentExportStrategyConfiguration AsName(string name)
        {
            return _strategy.AsName(name);
        }

        /// <summary>
        /// Export by interfaces
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IFluentExportStrategyConfiguration ByInterfaces(Func<Type, bool> filter = null)
        {
            return _strategy.ByInterfaces(filter);
        }

        /// <summary>
        /// Mark the export as externally owned so the container does not track for disposal
        /// </summary>
        /// <returns>configuraiton object</returns>
        public IFluentExportStrategyConfiguration ExternallyOwned()
        {
            return _strategy.ExternallyOwned();
        }

        /// <summary>
        /// Use specific constructor for use
        /// </summary>
        /// <param name="constructorInfo">constructor to use</param>
        /// <returns></returns>
        public IFluentExportStrategyConfiguration ImportConstructor(ConstructorInfo constructorInfo)
        {
            return _strategy.ImportConstructor(constructorInfo);
        }

        /// <summary>
        /// Specify the constructor selection algorithm
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public IFluentExportStrategyConfiguration ImportConstructorSelection(IConstructorExpressionCreator method)
        {
            return _strategy.ImportConstructorSelection(method);
        }

        /// <summary>
        /// Import a specific member
        /// </summary>
        /// <param name="selector">selector method, can be null</param>
        /// <param name="injectMethods"></param>
        /// <returns>configuraiton object</returns>
        public IFluentExportStrategyConfiguration ImportMembers(Func<MemberInfo, bool> selector = null, bool injectMethods = false)
        {
            return _strategy.ImportMembers(selector,injectMethods);
        }

        /// <summary>
        /// Import property by name
        /// </summary>
        /// <param name="propertyName">property name</param>
        /// <returns>configuration object</returns>
        public IFluentImportPropertyConfiguration ImportProperty(string propertyName)
        {
            return _strategy.ImportProperty(propertyName);
        }

        /// <summary>
        /// Apply a lifestlye to export strategy
        /// </summary>
        public ILifestylePicker<IFluentExportStrategyConfiguration> Lifestyle => _strategy.Lifestyle;

        /// <summary>
        /// Export only if function returns true
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IFluentExportStrategyConfiguration OnlyIf(Func<IExportRegistrationBlock, bool> filter)
        {
            return _strategy.OnlyIf(filter);
        }

        /// <inheritdoc />
        public IFluentExportStrategyConfiguration ProcessAttributes()
        {
            return _strategy.ProcessAttributes();
        }

        /// <summary>
        /// Assign a custom lifestyle to an export
        /// </summary>
        /// <param name="lifestyle"></param>
        /// <returns>configuraiton object</returns>
        public IFluentExportStrategyConfiguration UsingLifestyle(ICompiledLifestyle lifestyle)
        {
            return _strategy.Lifestyle.Custom(lifestyle);
        }

        /// <summary>
        /// Apply a condition on when to use strategy
        /// </summary>
        public IWhenConditionConfiguration<IFluentExportStrategyConfiguration> When => _strategy.When;

        /// <summary>
        /// Configure constructor parameter
        /// </summary>
        /// <param name="parameterType">parameter type</param>
        /// <returns></returns>
        public IFluentWithCtorConfiguration WithCtorParam(Type parameterType = null)
        {
            return _strategy.WithCtorParam(parameterType);
        }

        /// <summary>
        /// Configure constructor parameter
        /// </summary>
        /// <typeparam name="TParam"></typeparam>
        /// <param name="paramFunc"></param>
        /// <returns></returns>
        public IFluentWithCtorConfiguration<TParam> WithCtorParam<TParam>(Func<TParam> paramFunc = null)
        {
            return _strategy.WithCtorParam(paramFunc);
        }

        /// <summary>
        /// Export with specific metadata
        /// </summary>
        /// <param name="key">metadata key</param>
        /// <param name="value">metadata value</param>
        /// <returns>configuraiton object</returns>
        public IFluentExportStrategyConfiguration WithMetadata(object key, object value)
        {
            return _strategy.WithMetadata(key, value);
        }

        /// <summary>
        /// Set the priority for the export
        /// </summary>
        /// <param name="priority"></param>
        /// <returns></returns>
        public IFluentExportStrategyConfiguration WithPriority(int priority)
        {
            return _strategy.WithPriority(priority);
        }

        /// <summary>
        /// Defines a custom scope when creating instance
        /// </summary>
        /// <param name="customscope"></param>
        /// <returns></returns>
        public IFluentExportStrategyConfiguration DefinesNamedScope(string customscope)
        {
            return _strategy.DefinesNamedScope(customscope);
        }
    }

    /// <summary>
    /// Base class for configurin an export strategy
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ProxyFluentExportStrategyConfiguration<T> : IFluentExportStrategyConfiguration<T>
    {
        private readonly IFluentExportStrategyConfiguration<T> _strategy;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="strategy"></param>
        protected ProxyFluentExportStrategyConfiguration(IFluentExportStrategyConfiguration<T> strategy)
        {
            _strategy = strategy;
        }

        #region IFluentExportStrategyConfiguration

        /// <summary>
        /// Mark a particular Action() as the activation action
        /// </summary>
        /// <param name="activationMethod"></param>
        /// <returns></returns>
        public IFluentExportStrategyConfiguration<T> ActivationMethod(Expression<Action<T>> activationMethod)
        {
            return _strategy.ActivationMethod(activationMethod);
        }

        /// <summary>
        /// Apply an action to the export just after construction
        /// </summary>
        /// <param name="applyAction">action to apply to export upon construction</param>
        /// <returns>configuration object</returns>
        public IFluentExportStrategyConfiguration<T> Apply(Action<T> applyAction)
        {
            return _strategy.Apply(applyAction);
        }

        /// <summary>
        /// Export as a specific type
        /// </summary>
        /// <param name="type">type to export as</param>
        /// <returns></returns>
        public IFluentExportStrategyConfiguration<T> As(Type type)
        {
            return _strategy.As(type);
        }

        /// <summary>
        /// Export as a particular type
        /// </summary>
        /// <typeparam name="TInterface">type to export as</typeparam>
        /// <returns>configuration object</returns>
        public IFluentExportStrategyConfiguration<T> As<TInterface>()
        {
            return _strategy.As<TInterface>();
        }

        /// <summary>
        /// Export as keyed type
        /// </summary>
        /// <param name="type">export type</param>
        /// <param name="key">key</param>
        /// <returns></returns>
        public IFluentExportStrategyConfiguration<T> AsKeyed(Type type, object key)
        {
            return _strategy.AsKeyed(type, key);
        }

        /// <summary>
        /// Export as a keyed type
        /// </summary>
        /// <typeparam name="TInterface">export type</typeparam>
        /// <param name="key">key to export under</param>
        /// <returns>configuration object</returns>
        public IFluentExportStrategyConfiguration<T> AsKeyed<TInterface>(object key)
        {
            return _strategy.AsKeyed<TInterface>(key);
        }

        /// <summary>
        /// Export as specific name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IFluentExportStrategyConfiguration<T> AsName(string name)
        {
            return _strategy.AsName(name);
        }

        /// <summary>
        /// Export the type by the interfaces it implements
        /// </summary>
        /// <returns></returns>
        public IFluentExportStrategyConfiguration<T> ByInterfaces(Func<Type, bool> filter = null)
        {
            return _strategy.ByInterfaces(filter);
        }

        /// <summary>
        /// Creates a new scope and then resolves decorators inside of it.
        /// </summary>
        /// <param name="namedScope"></param>
        /// <returns></returns>
        public IFluentExportStrategyConfiguration<T> DefinesNamedScope(string namedScope)
        {
            return _strategy.DefinesNamedScope(namedScope);
        }

        /// <summary>
        /// You can provide a cleanup method to be called 
        /// </summary>
        /// <param name="disposalCleanupDelegate"></param>
        /// <returns></returns>
        public IFluentExportStrategyConfiguration<T> DisposalCleanupDelegate(Action<T> disposalCleanupDelegate)
        {
            return _strategy.DisposalCleanupDelegate(disposalCleanupDelegate);
        }

        /// <summary>
        /// Enrich with delegate
        /// </summary>
        /// <param name="enrichmentDelegate">enrichment delegate</param>
        /// <returns></returns>
        public IFluentExportStrategyConfiguration<T> EnrichWithDelegate(Func<IExportLocatorScope, StaticInjectionContext, T, T> enrichmentDelegate)
        {
            return _strategy.EnrichWithDelegate(enrichmentDelegate);
        }

        /// <summary>
        /// Export a public member of the type (property, field or method with return value)
        /// </summary>
        /// <typeparam name="TValue">type to export</typeparam>
        /// <param name="memberExpression">member expression</param>
        /// <returns></returns>
        public IFluentExportMemberConfiguration<T> ExportMember<TValue>(Expression<Func<T, TValue>> memberExpression)
        {
            return _strategy.ExportMember(memberExpression);
        }

        /// <summary>
        /// Mark an export as externally owned means the container will not track and dispose the instance
        /// </summary>
        /// <returns></returns>
        public IFluentExportStrategyConfiguration<T> ExternallyOwned()
        {
            return _strategy.ExternallyOwned();
        }

        /// <summary>
        /// This method allows you to specify which constructor to use ( () => new MyTypeName("Specific", "Constructor") )
        /// </summary>
        /// <param name="constructorExpression">constructor expression ( () => new MyTypeName("Specific", "Constructor") )</param>
        /// <returns></returns>
        public IFluentExportStrategyConfiguration<T> ImportConstructor(Expression<Func<T>> constructorExpression)
        {
            return _strategy.ImportConstructor(constructorExpression);
        }

        /// <summary>
        /// Use specific constructor for use
        /// </summary>
        /// <param name="constructorInfo">constructor to use</param>
        /// <returns></returns>
        public IFluentExportStrategyConfiguration<T> ImportConstructor(ConstructorInfo constructorInfo)
        {
            return _strategy.ImportConstructor(constructorInfo);
        }

        /// <summary>
        /// Use a specific constructor selection method
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public IFluentExportStrategyConfiguration<T> ImportConstructorSelection(IConstructorExpressionCreator method)
        {
            return _strategy.ImportConstructorSelection(method);
        }

        /// <summary>
        /// Mark specific members to be injected
        /// </summary>
        /// <param name="selector">select specific members, if null all public members will be injected</param>
        /// <param name="injectMethod"></param>
        /// <returns>configuration object</returns>
        public IFluentExportStrategyConfiguration<T> ImportMembers(Func<MemberInfo, bool> selector = null, bool injectMethod = false)
        {
            return _strategy.ImportMembers(selector, injectMethod);
        }

        /// <summary>
        /// Import a specific property
        /// </summary>
        /// <typeparam name="TProp"></typeparam>
        /// <param name="property">property expression</param>
        /// <returns></returns>
        public IFluentImportPropertyConfiguration<T, TProp> ImportProperty<TProp>(Expression<Func<T, TProp>> property)
        {
            return _strategy.ImportProperty(property);
        }

        /// <summary>
        /// Import a specific method on the type
        /// </summary>
        /// <param name="method">method to import</param>
        /// <returns></returns>
        public IFluentExportStrategyConfiguration<T> ImportMethod(Expression<Action<T>> method)
        {
            return _strategy.ImportMethod(method);
        }

        /// <summary>
        /// Assign a lifestyle to this export
        /// </summary>
        public ILifestylePicker<IFluentExportStrategyConfiguration<T>> Lifestyle => _strategy.Lifestyle;
        
        /// <summary>
        /// Export only if function returns true
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IFluentExportStrategyConfiguration<T> OnlyIf(Func<IExportRegistrationBlock, bool> filter)
        {
            return _strategy.OnlyIf(filter);
        }

        /// <inheritdoc />
        public IFluentExportStrategyConfiguration<T> ProcessAttributes()
        {
            return _strategy.ProcessAttributes();
        }

        /// <summary>
        /// Export using a specific lifestyle
        /// </summary>
        /// <param name="lifestyle">lifestlye to use</param>
        /// <returns>configuration object</returns>
        public IFluentExportStrategyConfiguration<T> UsingLifestyle(ICompiledLifestyle lifestyle)
        {
            return _strategy.Lifestyle.Custom(lifestyle);
        }

        /// <summary>
        /// Add a condition to when this export can be used
        /// </summary>
        public IWhenConditionConfiguration<IFluentExportStrategyConfiguration<T>> When => _strategy.When;

        /// <summary>
        /// Add a specific value for a particuar parameter in the constructor
        /// </summary>
        /// <typeparam name="TParam">type of parameter</typeparam>
        /// <param name="paramValue">Func(T) value for the parameter</param>
        /// <returns>configuration object</returns>
        public IFluentWithCtorConfiguration<T, TParam> WithCtorParam<TParam>(Func<TParam> paramValue = null)
        {
            return _strategy.WithCtorParam(paramValue);
        }

        /// <summary>
        /// Import a collection allowing you to specify a filter and a sort order
        /// </summary>
        /// <typeparam name="TParam"></typeparam>
        /// <typeparam name="TItem"></typeparam>
        /// <returns></returns>
        public IFluentWithCtorCollectionConfiguration<T, TItem> WithCtorCollectionParam<TParam, TItem>() where TParam : IEnumerable<TItem>
        {
            return _strategy.WithCtorCollectionParam<TParam, TItem>();
        }

        /// <summary>
        /// Add a specific value for a particuar parameter in the constructor
        /// </summary>
        /// <typeparam name="TParam">type of parameter</typeparam>
        /// <typeparam name="TArg1"></typeparam>
        /// <param name="paramValue">Func(IInjectionScope, IInjectionContext, T) value for the parameter</param>
        /// <returns>configuration object</returns>
        public IFluentWithCtorConfiguration<T, TParam> WithCtorParam<TArg1, TParam>(Func<TArg1, TParam> paramValue)
        {
            return _strategy.WithCtorParam(paramValue);
        }
        
        /// <summary>
        /// Add a specific value for a particuar parameter in the constructor
        /// </summary>
        /// <typeparam name="TParam">type of parameter</typeparam>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <param name="paramValue">Func(IInjectionScope, IInjectionContext, T) value for the parameter</param>
        /// <returns>configuration object</returns>
        public IFluentWithCtorConfiguration<T, TParam> WithCtorParam<TArg1, TArg2, TParam>(Func<TArg1, TArg2, TParam> paramValue)
        {
            return _strategy.WithCtorParam(paramValue);
        }

        /// <summary>
        /// Add a specific value for a particuar parameter in the constructor
        /// </summary>
        /// <typeparam name="TParam">type of parameter</typeparam>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <typeparam name="TArg3"></typeparam>
        /// <param name="paramValue">Func(IInjectionScope, IInjectionContext, T) value for the parameter</param>
        /// <returns>configuration object</returns>
        public IFluentWithCtorConfiguration<T, TParam> WithCtorParam<TArg1, TArg2, TArg3, TParam>(Func<TArg1, TArg2, TArg3, TParam> paramValue)
        {
            return _strategy.WithCtorParam(paramValue);
        }

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
        public IFluentWithCtorConfiguration<T, TParam> WithCtorParam<TArg1, TArg2, TArg3, TArg4, TParam>(Func<TArg1, TArg2, TArg3, TArg4, TParam> paramValue)
        {
            return _strategy.WithCtorParam(paramValue);
        }

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
        public IFluentWithCtorConfiguration<T, TParam> WithCtorParam<TArg1, TArg2, TArg3, TArg4, TArg5, TParam>(Func<TArg1, TArg2, TArg3, TArg4, TArg5, TParam> paramValue)
        {
            return _strategy.WithCtorParam(paramValue);
        }

        /// <summary>
        /// Adds metadata to an export
        /// </summary>
        /// <param name="key">metadata key</param>
        /// <param name="value">metadata value</param>
        /// <returns>configuration object</returns>
        public IFluentExportStrategyConfiguration<T> WithMetadata(object key, object value)
        {
            return _strategy.WithMetadata(key, value);
        }

        /// <summary>
        /// Set the priority for the export
        /// </summary>
        /// <param name="priority"></param>
        /// <returns></returns>
        public IFluentExportStrategyConfiguration<T> WithPriority(int priority)
        {
            return _strategy.WithPriority(priority);
        }


        #endregion
    }
}
