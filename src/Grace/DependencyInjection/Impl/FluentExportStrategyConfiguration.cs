using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Grace.DependencyInjection.Conditions;
using Grace.DependencyInjection.Impl.CompiledStrategies;
using Grace.DependencyInjection.Impl.Expressions;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// Provides activation strategy
    /// </summary>
    public interface IActivationStrategyProvider
    {
        /// <summary>
        /// Get stragey from configuration
        /// </summary>
        /// <returns></returns>
        IActivationStrategy GetStrategy();
    }

    /// <summary>
    /// Configuration object for export strategy
    /// </summary>
    public class FluentExportStrategyConfiguration : IFluentExportStrategyConfiguration, IActivationStrategyProvider
    {
        private readonly IConfigurableActivationStrategy _exportConfiguration;
        private readonly IExportRegistrationBlock _registrationBlock;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="exportConfiguration"></param>
        public FluentExportStrategyConfiguration(IConfigurableActivationStrategy exportConfiguration, IExportRegistrationBlock registrationBlock)
        {
            _exportConfiguration = exportConfiguration;
            _registrationBlock = registrationBlock;
        }

        /// <summary>
        /// Export as a specific type
        /// </summary>
        /// <param name="type">type to export as</param>
        /// <returns>configuration object</returns>
        public IFluentExportStrategyConfiguration As(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            if (type.GetTypeInfo().IsGenericTypeDefinition &&
                !_exportConfiguration.ActivationType.GetTypeInfo().IsGenericTypeDefinition)
            {
                throw new ArgumentException("Exported type is not open generic but As type is open");
            }

            _exportConfiguration.AddExportAs(type);

            return this;
        }

        /// <summary>
        /// Export as keyed interface
        /// </summary>
        /// <param name="type">type to export as</param>
        /// <param name="key">key to export under</param>
        /// <returns></returns>
        public IFluentExportStrategyConfiguration AsKeyed(Type type, object key)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (key == null) throw new ArgumentNullException(nameof(key));

            if (type.GetTypeInfo().IsGenericTypeDefinition &&
                !_exportConfiguration.ActivationType.GetTypeInfo().IsGenericTypeDefinition)
            {
                throw new ArgumentException("Exported type is not open generic but As type is open");
            }

            _exportConfiguration.AddExportAsKeyed(type, key);

            return this;
        }

        /// <summary>
        /// Export as Name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IFluentExportStrategyConfiguration AsName(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            _exportConfiguration.AddExportAsName(name);

            return this;
        }

        /// <summary>
        /// Export by interfaces
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IFluentExportStrategyConfiguration ByInterfaces(Func<Type, bool> filter = null)
        {
            if (filter == null)
            {
                filter = t => !InjectionScopeConfiguration.DefaultInterfaceFilter(t, _exportConfiguration.ActivationType);
            }

            foreach (var interfaceTypes in _exportConfiguration.ActivationType.GetTypeInfo().ImplementedInterfaces)
            {
                if (!filter(interfaceTypes))
                {
                    continue;
                }

                if (_exportConfiguration.ActivationType.GetTypeInfo().IsGenericTypeDefinition)
                {
                    if (interfaceTypes.IsConstructedGenericType)
                    {
                        _exportConfiguration.AddExportAs(interfaceTypes.GetGenericTypeDefinition());
                    }
                }
                else
                {
                    _exportConfiguration.AddExportAs(interfaceTypes);
                }
            }

            return this;
        }

        /// <summary>
        /// Use specific constructor for use
        /// </summary>
        /// <param name="constructorInfo">constructor to use</param>
        /// <returns></returns>
        public IFluentExportStrategyConfiguration ImportConstructor(ConstructorInfo constructorInfo)
        {
            if (constructorInfo == null) throw new ArgumentNullException(nameof(constructorInfo));

            _exportConfiguration.SelectedConstructor = constructorInfo;

            return this;
        }

        /// <summary>
        /// Specify the constructor selection algorithm
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public IFluentExportStrategyConfiguration ImportConstructorSelection(IConstructorExpressionCreator method)
        {
            _exportConfiguration.ConstructorSelectionMethod = method;

            return this;
        }

        /// <summary>
        /// Import a specific member
        /// </summary>
        /// <param name="selector">selector method, can be null</param>
        /// <param name="injectMethods"></param>
        /// <returns>configuraiton object</returns>
        public IFluentExportStrategyConfiguration ImportMembers(Func<MemberInfo, bool> selector = null, bool injectMethods = false)
        {
            _exportConfiguration.MemberInjectionSelector(new PublicMemeberInjectionSelector(selector, injectMethods, false));

            return this;
        }

        /// <summary>
        /// Import property by name
        /// </summary>
        /// <param name="propertyName">property name</param>
        /// <returns>configuration object</returns>
        public IFluentImportPropertyConfiguration ImportProperty(string propertyName)
        {
            var property = _exportConfiguration.ActivationType.GetRuntimeProperty(propertyName);

            if (property == null)
            {
                throw new Exception($"Could not find property named {propertyName} on type {_exportConfiguration.ActivationType.Name}");
            }

            var memberInjection = new MemberInjectionInfo { MemberInfo = property };

            _exportConfiguration.MemberInjectionSelector(new KnownMemberInjectionSelector(memberInjection));

            return new FluentImportPropertyConfiguration(this, memberInjection);
        }

        /// <summary>
        /// Apply a lifestlye to export strategy
        /// </summary>
        public ILifestylePicker<IFluentExportStrategyConfiguration> Lifestyle => new LifestylePicker<IFluentExportStrategyConfiguration>(this, lifestlye => UsingLifestyle(lifestlye));

        /// <summary>
        /// Export only if function returns true
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IFluentExportStrategyConfiguration OnlyIf(Func<IExportRegistrationBlock, bool> filter)
        {
            if (!filter(_registrationBlock))
            {
                _registrationBlock.ClearExports(export => export == _exportConfiguration);
            }

            return this;
        }

        /// <inheritdoc />
        public IFluentExportStrategyConfiguration ProcessAttributes()
        {
            _exportConfiguration.ProcessAttributeForStrategy();

            return this;
        }

        /// <summary>
        /// Assign a custom lifestyle to an export
        /// </summary>
        /// <param name="lifestyle"></param>
        /// <returns>configuraiton object</returns>
        public IFluentExportStrategyConfiguration UsingLifestyle(ICompiledLifestyle lifestyle)
        {
            _exportConfiguration.Lifestyle = lifestyle;

            return this;
        }

        /// <summary>
        /// Apply a condition on when to use strategy
        /// </summary>
        public IWhenConditionConfiguration<IFluentExportStrategyConfiguration> When =>
            new WhenConditionConfiguration<IFluentExportStrategyConfiguration>(condition => _exportConfiguration.AddCondition(condition), this);


        /// <summary>
        /// Configure constructor parameter
        /// </summary>
        /// <param name="parameterType">parameter type</param>
        /// <returns></returns>
        public IFluentWithCtorConfiguration WithCtorParam(Type parameterType = null)
        {
            var constructorInfo = new ConstructorParameterInfo(null) { ParameterType = parameterType };

            _exportConfiguration.ConstructorParameter(constructorInfo);

            return new FluentWithCtorConfiguration(this, constructorInfo);
        }

        /// <summary>
        /// Configure constructor parameter
        /// </summary>
        /// <typeparam name="TParam"></typeparam>
        /// <param name="paramFunc"></param>
        /// <returns></returns>
        public IFluentWithCtorConfiguration<TParam> WithCtorParam<TParam>(Func<TParam> paramFunc = null)
        {
            var constructorInfo = new ConstructorParameterInfo(paramFunc) { ParameterType = typeof(TParam) };

            _exportConfiguration.ConstructorParameter(constructorInfo);

            return new FluentWithCtorConfiguration<TParam>(this, constructorInfo);
        }

        /// <summary>
        /// Export with specific metadata
        /// </summary>
        /// <param name="key">metadata key</param>
        /// <param name="value">metadata value</param>
        /// <returns>configuraiton object</returns>
        public IFluentExportStrategyConfiguration WithMetadata(object key, object value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            _exportConfiguration.SetMetadata(key, value);

            return this;
        }

        /// <summary>
        /// Set the priority for the export
        /// </summary>
        /// <param name="priority"></param>
        /// <returns></returns>
        public IFluentExportStrategyConfiguration WithPriority(int priority)
        {
            _exportConfiguration.Priority = priority;

            return this;
        }

        /// <summary>
        /// Defines a custom scope when creating instance
        /// </summary>
        /// <param name="customscope"></param>
        /// <returns></returns>
        public IFluentExportStrategyConfiguration DefinesNamedScope(string customscope)
        {
            _exportConfiguration.CustomScopeName = customscope;

            return this;
        }

        /// <summary>
        /// Mark the export as externally owned so the container does not track for disposal
        /// </summary>
        /// <returns>configuraiton object</returns>
        public IFluentExportStrategyConfiguration ExternallyOwned()
        {
            _exportConfiguration.ExternallyOwned = true;

            return this;
        }

        /// <summary>
        /// Get stragey from configuration
        /// </summary>
        /// <returns></returns>
        public IActivationStrategy GetStrategy()
        {
            return _exportConfiguration;
        }

    }

    /// <summary>
    /// Configuration object for export stategy
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FluentExportStrategyConfiguration<T> : IFluentExportStrategyConfiguration<T>, IActivationStrategyProvider
    {
        private readonly ICompiledExportStrategy _exportConfiguration;
        private readonly IExportRegistrationBlock _registrationBlock;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="exportConfiguration"></param>
        public FluentExportStrategyConfiguration(ICompiledExportStrategy exportConfiguration, IExportRegistrationBlock registrationBlock)
        {
            _exportConfiguration = exportConfiguration;
            _registrationBlock = registrationBlock;
        }

        /// <summary>
        /// Mark a particular Action() as the activation action
        /// </summary>
        /// <param name="activationMethod"></param>
        /// <returns></returns>
        public IFluentExportStrategyConfiguration<T> ActivationMethod(Expression<Action<T>> activationMethod)
        {
            if (activationMethod == null) throw new ArgumentNullException(nameof(activationMethod));

            var methodExpression = activationMethod.Body as MethodCallExpression;

            if (methodExpression == null) throw new ArgumentException("Must be method call expression", nameof(activationMethod));

            _exportConfiguration.ActivationMethod = new MethodInjectionInfo { Method = methodExpression.Method };

            return this;
        }

        /// <summary>
        /// Apply an action to the export just after construction
        /// </summary>
        /// <param name="applyAction">action to apply to export upon construction</param>
        /// <returns>configuration object</returns>
        public IFluentExportStrategyConfiguration<T> Apply(Action<T> applyAction)
        {
            var enrichmentDelegate = new Func<IExportLocatorScope, T, T>((scope, t) =>
              {
                  applyAction(t);

                  return t;
              });

            _exportConfiguration.EnrichmentDelegate(enrichmentDelegate);

            return this;
        }

        /// <summary>
        /// Export as a specific type
        /// </summary>
        /// <param name="type">type to export as</param>
        /// <returns></returns>
        public IFluentExportStrategyConfiguration<T> As(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            _exportConfiguration.AddExportAs(type);

            return this;
        }

        /// <summary>
        /// Export as a particular type
        /// </summary>
        /// <typeparam name="TInterface">type to export as</typeparam>
        /// <returns>configuration object</returns>
        public IFluentExportStrategyConfiguration<T> As<TInterface>()
        {
            _exportConfiguration.AddExportAs(typeof(TInterface));

            return this;
        }

        /// <summary>
        /// Export as keyed type
        /// </summary>
        /// <param name="type">export type</param>
        /// <param name="key">key</param>
        /// <returns></returns>
        public IFluentExportStrategyConfiguration<T> AsKeyed(Type type, object key)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (key == null) throw new ArgumentNullException(nameof(key));

            _exportConfiguration.AddExportAsKeyed(type, key);

            return this;
        }

        /// <summary>
        /// Export as a keyed type
        /// </summary>
        /// <typeparam name="TInterface">export type</typeparam>
        /// <param name="key">key to export under</param>
        /// <returns>configuration object</returns>
        public IFluentExportStrategyConfiguration<T> AsKeyed<TInterface>(object key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            _exportConfiguration.AddExportAsKeyed(typeof(TInterface), key);

            return this;
        }

        /// <summary>
        /// Export as specific name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IFluentExportStrategyConfiguration<T> AsName(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentException("Value cannot be null or empty.", nameof(name));

            _exportConfiguration.AddExportAsName(name);

            return this;
        }

        /// <summary>
        /// Export the type by the interfaces it implements
        /// </summary>
        /// <returns>configuration object</returns>
        public IFluentExportStrategyConfiguration<T> ByInterfaces(Func<Type, bool> filter = null)
        {
            if (filter == null)
            {
                filter = t => !InjectionScopeConfiguration.DefaultInterfaceFilter(t, typeof(T));
            }

            foreach (var interfaceTypes in _exportConfiguration.ActivationType.GetTypeInfo().ImplementedInterfaces)
            {
                if (!filter(interfaceTypes))
                {
                    continue;
                }

                _exportConfiguration.AddExportAs(interfaceTypes);
            }

            return this;
        }

        /// <summary>
        /// Creates a new scope and then resolves decorators inside of it.
        /// </summary>
        /// <param name="namedScope"></param>
        /// <returns></returns>
        public IFluentExportStrategyConfiguration<T> DefinesNamedScope(string namedScope)
        {
            _exportConfiguration.CustomScopeName = namedScope;

            return this;
        }

        /// <summary>
        /// You can provide a cleanup method to be called 
        /// </summary>
        /// <param name="disposalCleanupDelegate">action to call when disposing</param>
        /// <returns>configuration object</returns>
        public IFluentExportStrategyConfiguration<T> DisposalCleanupDelegate(Action<T> disposalCleanupDelegate)
        {
            _exportConfiguration.DisposalDelegate = disposalCleanupDelegate;

            return this;
        }

        /// <summary>
        /// Enrich with delegate
        /// </summary>
        /// <param name="enrichmentDelegate">enrichment delegate</param>
        /// <returns></returns>
        public IFluentExportStrategyConfiguration<T> EnrichWithDelegate(Func<IExportLocatorScope, StaticInjectionContext, T, T> enrichmentDelegate)
        {
            if (enrichmentDelegate == null) throw new ArgumentNullException(nameof(enrichmentDelegate));

            _exportConfiguration.EnrichmentDelegate(enrichmentDelegate);

            return this;
        }

        /// <summary>
        /// Export a public member of the type (property, field or method with return value)
        /// </summary>
        /// <typeparam name="TValue">type to export</typeparam>
        /// <param name="memberExpression">member expression</param>
        /// <returns></returns>
        public IFluentExportMemberConfiguration<T> ExportMember<TValue>(Expression<Func<T, TValue>> memberExpression)
        {
            ICompiledExportStrategy strategy = null;

            var member = memberExpression.Body as MemberExpression;

            if (member != null)
            {
                if (member.Member is PropertyInfo)
                {
                    var propertyInfo = (PropertyInfo)member.Member;

                    strategy = new ExportedPropertyOrFieldStrategy(propertyInfo.PropertyType,
                        _exportConfiguration.InjectionScope, _exportConfiguration, propertyInfo.Name);
                }
                else if (member.Member is FieldInfo)
                {
                    var fieldInfo = (FieldInfo)member.Member;

                    strategy = new ExportedPropertyOrFieldStrategy(fieldInfo.FieldType,
                        _exportConfiguration.InjectionScope, _exportConfiguration, fieldInfo.Name);
                }
            }
            else
            {
                var methodCall = memberExpression.Body as MethodCallExpression;

                if (methodCall != null)
                {
                    var methodInfo = methodCall.Method;

                    strategy = new ExportMethodStrategy(methodInfo.ReturnType, _exportConfiguration.InjectionScope,
                        _exportConfiguration, methodInfo);
                }
            }

            if (strategy == null)
            {
                throw new NotSupportedException("Expression is not supported as a means to export, please use a property, field or method");
            }

            _exportConfiguration.AddSecondaryStrategy(strategy);

            return new FluentExportMemberConfiguration<T>(this, strategy);
        }

        /// <summary>
        /// Mark an export as externally owned means the container will not track and dispose the instance
        /// </summary>
        /// <returns>configuration object</returns>
        public IFluentExportStrategyConfiguration<T> ExternallyOwned()
        {
            _exportConfiguration.ExternallyOwned = true;

            return this;
        }

        /// <summary>
        /// This method allows you to specify which constructor to use ( () => new MyTypeName("Specific", "Constructor") )
        /// </summary>
        /// <param name="constructorExpression">constructor expression ( () => new MyTypeName("Specific", "Constructor") )</param>
        /// <returns></returns>
        public IFluentExportStrategyConfiguration<T> ImportConstructor(Expression<Func<T>> constructorExpression)
        {
            var newExpression = constructorExpression.Body as NewExpression;

            if (newExpression != null)
            {
                _exportConfiguration.SelectedConstructor = newExpression.Constructor;
            }

            return this;
        }

        /// <summary>
        /// Use specific constructor for use
        /// </summary>
        /// <param name="constructorInfo">constructor to use</param>
        /// <returns></returns>
        public IFluentExportStrategyConfiguration<T> ImportConstructor(ConstructorInfo constructorInfo)
        {
            if (constructorInfo == null) throw new ArgumentNullException(nameof(constructorInfo));

            _exportConfiguration.SelectedConstructor = constructorInfo;

            return this;
        }

        /// <summary>
        /// Use a specific constructor selection method
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public IFluentExportStrategyConfiguration<T> ImportConstructorSelection(IConstructorExpressionCreator method)
        {
            _exportConfiguration.ConstructorSelectionMethod = method;

            return this;
        }

        /// <summary>
        /// Mark specific members to be injected
        /// </summary>
        /// <param name="selector">select specific members, if null all public members will be injected</param>
        /// <param name="injectMethod"></param>
        /// <returns>configuration object</returns>
        public IFluentExportStrategyConfiguration<T> ImportMembers(Func<MemberInfo, bool> selector = null, bool injectMethod = false)
        {
            _exportConfiguration.MemberInjectionSelector(new PublicMemeberInjectionSelector(selector ?? (m => true), injectMethod, false));

            return this;
        }

        /// <summary>
        /// Import a specific property
        /// </summary>
        /// <typeparam name="TProp"></typeparam>
        /// <param name="property">property expression</param>
        /// <returns>configuration object</returns>
        public IFluentImportPropertyConfiguration<T, TProp> ImportProperty<TProp>(Expression<Func<T, TProp>> property)
        {
            var member = property.Body as MemberExpression;

            if (member == null)
            {
                throw new ArgumentException("Property must be a property on type" + typeof(T).FullName, nameof(property));
            }

            var propertyInfo =
                member.Member.DeclaringType.GetTypeInfo().GetDeclaredProperty(member.Member.Name);

            var memberInfo = new MemberInjectionInfo
            {
                MemberInfo = propertyInfo
            };

            if (_exportConfiguration.InjectionScope.ScopeConfiguration.Behaviors.KeyedTypeSelector(member.Type))
            {
                memberInfo.LocateKey = member.Member.Name;
            }

            _exportConfiguration.MemberInjectionSelector(new KnownMemberInjectionSelector(memberInfo));

            return new FluentImportPropertyConfiguration<T, TProp>(this, memberInfo);
        }

        /// <summary>
        /// Import a specific method on the type
        /// </summary>
        /// <param name="method">method to import</param>
        /// <returns></returns>
        public IFluentExportStrategyConfiguration<T> ImportMethod(Expression<Action<T>> method)
        {
            var methodCall = method.Body as MethodCallExpression;

            if (methodCall == null)
            {
                throw new ArgumentException("expression must be method", nameof(method));
            }

            var methodInjectionInfo = new MethodInjectionInfo { Method = methodCall.Method };

            _exportConfiguration.MethodInjectionInfo(methodInjectionInfo);

            return this;
        }

        /// <summary>
        /// Assign a lifestyle to this export
        /// </summary>
        public ILifestylePicker<IFluentExportStrategyConfiguration<T>> Lifestyle =>
            new LifestylePicker<IFluentExportStrategyConfiguration<T>>(this, lifeStyle => UsingLifestyle(lifeStyle));

        /// <summary>
        /// Export only if function returns true
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IFluentExportStrategyConfiguration<T> OnlyIf(Func<IExportRegistrationBlock, bool> filter)
        {
            if (!filter(_registrationBlock))
            {
                _registrationBlock.ClearExports(export => export == _exportConfiguration);
            }

            return this;
        }

        /// <inheritdoc />
        public IFluentExportStrategyConfiguration<T> ProcessAttributes()
        {
            _exportConfiguration.ProcessAttributeForStrategy();

            return this;
        }

        /// <summary>
        /// Export using a specific lifestyle
        /// </summary>
        /// <param name="lifestyle">lifestlye to use</param>
        /// <returns>configuration object</returns>
        public IFluentExportStrategyConfiguration<T> UsingLifestyle(ICompiledLifestyle lifestyle)
        {
            _exportConfiguration.Lifestyle = lifestyle;

            return this;
        }

        /// <summary>
        /// Add a condition to when this export can be used
        /// </summary>
        public IWhenConditionConfiguration<IFluentExportStrategyConfiguration<T>> When =>
            new WhenConditionConfiguration<IFluentExportStrategyConfiguration<T>>(condition => _exportConfiguration.AddCondition(condition), this);

        /// <summary>
        /// Add a specific value for a particuar parameter in the constructor
        /// </summary>
        /// <typeparam name="TParam">type of parameter</typeparam>
        /// <param name="paramValue">Func(T) value for the parameter</param>
        /// <returns>configuration object</returns>
        public IFluentWithCtorConfiguration<T, TParam> WithCtorParam<TParam>(Func<TParam> paramValue = null)
        {
            var parameterInfo = new ConstructorParameterInfo(paramValue) { ParameterType = typeof(TParam) };

            _exportConfiguration.ConstructorParameter(parameterInfo);

            return new FluentWithCtorConfiguration<T, TParam>(this, parameterInfo);
        }

        /// <summary>
        /// Import a collection allowing you to specify a filter and a sort order
        /// </summary>
        /// <typeparam name="TParam"></typeparam>
        /// <typeparam name="TItem"></typeparam>
        /// <returns></returns>
        public IFluentWithCtorCollectionConfiguration<T, TItem> WithCtorCollectionParam<TParam, TItem>() where TParam : IEnumerable<TItem>
        {
            var parameterInfo = new ConstructorParameterInfo(null) { ParameterType = typeof(IEnumerable<TItem>) };

            _exportConfiguration.ConstructorParameter(parameterInfo);

            return new FluentWithCtorCollectionConfiguration<T, TItem>(this, parameterInfo);
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
            if (paramValue == null) throw new ArgumentNullException(nameof(paramValue));

            var parameterInfo = new ConstructorParameterInfo(paramValue) { ParameterType = typeof(TParam) };

            _exportConfiguration.ConstructorParameter(parameterInfo);

            return new FluentWithCtorConfiguration<T, TParam>(this, parameterInfo);
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
            if (paramValue == null) throw new ArgumentNullException(nameof(paramValue));

            var parameterInfo = new ConstructorParameterInfo(paramValue) { ParameterType = typeof(TParam) };

            _exportConfiguration.ConstructorParameter(parameterInfo);

            return new FluentWithCtorConfiguration<T, TParam>(this, parameterInfo);
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
            if (paramValue == null) throw new ArgumentNullException(nameof(paramValue));

            var parameterInfo = new ConstructorParameterInfo(paramValue) { ParameterType = typeof(TParam) };

            _exportConfiguration.ConstructorParameter(parameterInfo);

            return new FluentWithCtorConfiguration<T, TParam>(this, parameterInfo);
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
            if (paramValue == null) throw new ArgumentNullException(nameof(paramValue));

            var parameterInfo = new ConstructorParameterInfo(paramValue) { ParameterType = typeof(TParam) };

            _exportConfiguration.ConstructorParameter(parameterInfo);

            return new FluentWithCtorConfiguration<T, TParam>(this, parameterInfo);
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
            if (paramValue == null) throw new ArgumentNullException(nameof(paramValue));

            var parameterInfo = new ConstructorParameterInfo(paramValue) { ParameterType = typeof(TParam) };

            _exportConfiguration.ConstructorParameter(parameterInfo);

            return new FluentWithCtorConfiguration<T, TParam>(this, parameterInfo);
        }

        /// <summary>
        /// Adds metadata to an export
        /// </summary>
        /// <param name="key">metadata key</param>
        /// <param name="value">metadata value</param>
        /// <returns>configuration object</returns>
        public IFluentExportStrategyConfiguration<T> WithMetadata(object key, object value)
        {
            _exportConfiguration.SetMetadata(key, value);

            return this;
        }

        /// <summary>
        /// Set the priority for the export
        /// </summary>
        /// <param name="priority"></param>
        /// <returns></returns>
        public IFluentExportStrategyConfiguration<T> WithPriority(int priority)
        {
            _exportConfiguration.Priority = priority;

            return this;
        }

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
