using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Grace.Data.Immutable;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// Represents a block of registration
    /// </summary>
    public class ExportRegistrationBlock : IExportRegistrationBlockValueProvider
    {
        private readonly List<ICompiledExportStrategy> _exportStrategyProviders = new List<ICompiledExportStrategy>();
        private ImmutableLinkedList<IWrapperStrategyProvider> _wrapperProviders = ImmutableLinkedList<IWrapperStrategyProvider>.Empty;
        private ImmutableLinkedList<IDecoratorStrategyProvider> _decoratorStrategyProviders = ImmutableLinkedList<IDecoratorStrategyProvider>.Empty;
        private ImmutableLinkedList<IActivationStrategyInspector> _inspectors = ImmutableLinkedList<IActivationStrategyInspector>.Empty;
        private ImmutableLinkedList<IInjectionValueProvider> _valueProviders = ImmutableLinkedList<IInjectionValueProvider>.Empty;
        private ImmutableLinkedList<IMissingExportStrategyProvider> _missingExportStrategyProviders = ImmutableLinkedList<IMissingExportStrategyProvider>.Empty;
        private ImmutableLinkedList<IMissingDependencyExpressionProvider> _missingDependencyExpressionProviders = ImmutableLinkedList<IMissingDependencyExpressionProvider>.Empty;
        private ImmutableLinkedList<IMemberInjectionSelector> _globalSelectors = ImmutableLinkedList<IMemberInjectionSelector>.Empty;
        private readonly IActivationStrategyCreator _strategyCreator;
        private IExportStrategyProvider _currentProvider;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="owningScope"></param>
        /// <param name="strategyCreator"></param>
        public ExportRegistrationBlock(IInjectionScope owningScope, IActivationStrategyCreator strategyCreator)
        {
            _strategyCreator = strategyCreator;
            OwningScope = owningScope;
        }

        /// <summary>
        /// Scope this registration block is for
        /// </summary>
        public IInjectionScope OwningScope { get; }

        /// <summary>
        /// Export strategies from the registration block
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ICompiledExportStrategy> GetExportStrategies()
        {
            ProcessCurrentProvider();

            return _exportStrategyProviders;
        }

        /// <summary>
        /// Decorators from the registration block
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ICompiledDecoratorStrategy> GetDecoratorStrategies()
        {
            if (_decoratorStrategyProviders == ImmutableLinkedList<IDecoratorStrategyProvider>.Empty)
            {
                yield break;
            }

            foreach (var strategyProvider in _decoratorStrategyProviders.Reverse())
            {
                foreach (var strategy in strategyProvider.ProvideStrategies())
                {
                    yield return strategy;
                }
            }
        }

        /// <summary>
        /// Wrappers from the registration block
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ICompiledWrapperStrategy> GetWrapperStrategies()
        {
            if (_wrapperProviders == ImmutableLinkedList<IWrapperStrategyProvider>.Empty)
            {
                yield break;
            }

            foreach (var wrapperProvider in _wrapperProviders)
            {
                foreach (var strategy in wrapperProvider.ProvideWrappers())
                {
                    yield return strategy;
                }
            }
        }

        /// <summary>
        /// Get inspectors registered in block
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IActivationStrategyInspector> GetInspectors()
        {
            return _inspectors;
        }

        /// <summary>
        /// Get list of missing export strategy providers
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IMissingExportStrategyProvider> GetMissingExportStrategyProviders()
        {
            return _missingExportStrategyProviders;
        }

        /// <summary>
        /// Get list of missing dependency expression provider
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IMissingDependencyExpressionProvider> GetMissingDependencyExpressionProviders()
        {
            return _missingDependencyExpressionProviders;
        }

        /// <summary>
        /// Get list of value providers
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IInjectionValueProvider> GetValueProviders()
        {
            return _valueProviders;
        }

        /// <summary>
        /// Get member injection selectors
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IMemberInjectionSelector> GetMemberInjectionSelectors()
        {
            return _globalSelectors;
        }
        
        /// <summary>
        /// Add IMemberInjectionSelctor that selects 
        /// </summary>
        /// <param name="selector"></param>
        public void AddMemberInjectionSelector(IMemberInjectionSelector selector)
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            
            _globalSelectors = _globalSelectors.Add(selector);
        }

        /// <summary>
        /// Add configuration module
        /// </summary>
        /// <param name="module"></param>
        public void AddModule(IConfigurationModule module)
        {
            if (module == null) throw new ArgumentNullException(nameof(module));

            module.Configure(this);
        }

        /// <summary>
        /// Export a specific type
        /// </summary>
        /// <typeparam name="T">type to export</typeparam>
        /// <returns>export configuration</returns>
        public IFluentExportStrategyConfiguration<T> Export<T>()
        {
            var strategy = _strategyCreator.GetCompiledExportStrategy(typeof(T));

            AddExportStrategy(strategy);

            return new FluentExportStrategyConfiguration<T>(strategy, this);
        }

        /// <summary>
        /// Export a specific type (open generics allowed)
        /// </summary>
        /// <param name="type">type to export</param>
        /// <returns>export configuration</returns>
        public IFluentExportStrategyConfiguration Export(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            var strategy = _strategyCreator.GetCompiledExportStrategy(type);

            AddExportStrategy(strategy);

            return new FluentExportStrategyConfiguration(strategy, this);
        }

        /// <summary>
        /// Export a set of types
        /// </summary>
        /// <param name="types">types to export</param>
        /// <returns></returns>
        public IExportTypeSetConfiguration Export(IEnumerable<Type> types)
        {
            if (types == null) throw new ArgumentNullException(nameof(types));

            ProcessCurrentProvider();

            var configuration = _strategyCreator.GetTypeSetConfiguration(types);

            _currentProvider = (IExportStrategyProvider)configuration;

            return configuration;
        }

        /// <summary>
        /// Export a specific value
        /// </summary>
        /// <typeparam name="T">type to export</typeparam>
        /// <param name="instance">instance to export</param>
        /// <returns></returns>
        public IFluentExportInstanceConfiguration<T> ExportInstance<T>(T instance)
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));

            var strategy = _strategyCreator.GetConstantStrategy(instance);

            AddActivationStrategy(strategy);

            return new FluentExportInstanceConfiguration<T>(strategy, this);
        }

        /// <summary>
        /// Export a specific type using a function
        /// </summary>
        /// <typeparam name="T">type to export</typeparam>
        /// <param name="instanceFunc">function to create instance</param>
        /// <returns></returns>
        public IFluentExportInstanceConfiguration<T> ExportInstance<T>(Func<T> instanceFunc)
        {
            if (instanceFunc == null) throw new ArgumentNullException(nameof(instanceFunc));

            var strategy = _strategyCreator.GetFuncStrategy(instanceFunc);

            AddActivationStrategy(strategy);

            return new FluentExportInstanceConfiguration<T>(strategy, this);
        }

        /// <summary>
        /// Export a specific type using an IExportLocatorScope
        /// </summary>
        /// <typeparam name="T">type to export</typeparam>
        /// <param name="instanceFunc">instance func</param>
        /// <returns></returns>
        public IFluentExportInstanceConfiguration<T> ExportInstance<T>(Func<IExportLocatorScope, T> instanceFunc)
        {
            if (instanceFunc == null) throw new ArgumentNullException(nameof(instanceFunc));

            var strategy = _strategyCreator.GetFuncWithScopeStrategy(instanceFunc);

            AddActivationStrategy(strategy);

            return new FluentExportInstanceConfiguration<T>(strategy, this);
        }

        /// <summary>
        /// Export a specific type using IExportLocatorScope and StaticInjectionContext
        /// </summary>
        /// <typeparam name="T">type to export</typeparam>
        /// <param name="instanceFunc">isntance func</param>
        /// <returns></returns>
        public IFluentExportInstanceConfiguration<T> ExportInstance<T>(Func<IExportLocatorScope, StaticInjectionContext, T> instanceFunc)
        {
            if (instanceFunc == null) throw new ArgumentNullException(nameof(instanceFunc));

            var strategy = _strategyCreator.GetFuncWithStaticContextStrategy(instanceFunc);

            AddActivationStrategy(strategy);

            return new FluentExportInstanceConfiguration<T>(strategy, this);
        }

        /// <summary>
        /// Export a specific type using IExportLocatorScope, StaticInjectionContext and IInjectionContext
        /// </summary>
        /// <typeparam name="T">type to export</typeparam>
        /// <param name="instanceFunc">isntance func</param>
        /// <returns></returns>
        public IFluentExportInstanceConfiguration<T> ExportInstance<T>(
            Func<IExportLocatorScope, StaticInjectionContext, IInjectionContext, T> instanceFunc)
        {
            if (instanceFunc == null) throw new ArgumentNullException(nameof(instanceFunc));

            var strategy = _strategyCreator.GetFuncWithInjectionContextStrategy(instanceFunc);

            AddActivationStrategy(strategy);

            return new FluentExportInstanceConfiguration<T>(strategy, this);
        }

        /// <summary>
        /// Export an expression tree
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public IFluentExportInstanceConfiguration<TResult> ExportExpression<TResult>(Expression<Func<TResult>> expression)
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));


            var strategy = _strategyCreator.GetExpressionExportStrategy(expression);

            AddActivationStrategy(strategy);

            return new FluentExportInstanceConfiguration<TResult>(strategy, this);
        }

        /// <summary>
        /// Export a specific type
        /// </summary>
        /// <typeparam name="TResult">exported type</typeparam>
        /// <param name="factory">export factory</param>
        /// <returns></returns>
        public IFluentExportInstanceConfiguration<TResult> ExportFactory<TResult>(Func<TResult> factory)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));

            var strategy = _strategyCreator.GetFactoryStrategy(factory);

            AddActivationStrategy(strategy);

            return new FluentExportInstanceConfiguration<TResult>(strategy, this);
        }

        /// <summary>
        /// Export a specific type that requires some dependency
        /// </summary>
        /// <typeparam name="TIn">dependency type</typeparam>
        /// <typeparam name="TResult">export type</typeparam>
        /// <param name="factory">export function</param>
        /// <returns></returns>
        public IFluentExportInstanceConfiguration<TResult> ExportFactory<TIn, TResult>(Func<TIn, TResult> factory)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));

            var strategy = _strategyCreator.GetFactoryStrategy(factory);

            AddActivationStrategy(strategy);

            return new FluentExportInstanceConfiguration<TResult>(strategy, this);
        }

        /// <summary>
        /// Export a specific type that requires some dependencies
        /// </summary>
        /// <typeparam name="T1">dependency one</typeparam>
        /// <typeparam name="T2">dependency two</typeparam>
        /// <typeparam name="TResult">export type</typeparam>
        /// <param name="factory">export factory</param>
        /// <returns></returns>
        public IFluentExportInstanceConfiguration<TResult> ExportFactory<T1, T2, TResult>(Func<T1, T2, TResult> factory)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));

            var strategy = _strategyCreator.GetFactoryStrategy(factory);

            AddActivationStrategy(strategy);

            return new FluentExportInstanceConfiguration<TResult>(strategy, this);
        }

        /// <summary>
        /// Export a specific type that requires some dependencies
        /// </summary>
        /// <typeparam name="T1">dependency one</typeparam>
        /// <typeparam name="T2">dependency two</typeparam>
        /// <typeparam name="T3">dependency three</typeparam>
        /// <typeparam name="TResult">export type</typeparam>
        /// <param name="factory">export factory</param>
        /// <returns></returns>
        public IFluentExportInstanceConfiguration<TResult> ExportFactory<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> factory)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));

            var strategy = _strategyCreator.GetFactoryStrategy(factory);

            AddActivationStrategy(strategy);

            return new FluentExportInstanceConfiguration<TResult>(strategy, this);
        }

        /// <summary>
        /// Export a specific type that requires some dependencies
        /// </summary>
        /// <typeparam name="T1">dependency one</typeparam>
        /// <typeparam name="T2">dependency two</typeparam>
        /// <typeparam name="T3">dependency three</typeparam>
        /// <typeparam name="T4">dependency four</typeparam>
        /// <typeparam name="TResult">export type</typeparam>
        /// <param name="factory">export factory</param>
        /// <returns></returns>
        public IFluentExportInstanceConfiguration<TResult> ExportFactory<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> factory)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));

            var strategy = _strategyCreator.GetFactoryStrategy(factory);

            AddActivationStrategy(strategy);

            return new FluentExportInstanceConfiguration<TResult>(strategy, this);
        }

        /// <summary>
        /// Export a specific type that requires some dependencies
        /// </summary>
        /// <typeparam name="T1">dependency one</typeparam>
        /// <typeparam name="T2">dependency two</typeparam>
        /// <typeparam name="T3">dependency three</typeparam>
        /// <typeparam name="T4">dependency four</typeparam>
        /// <typeparam name="T5">dependency five</typeparam>
        /// <typeparam name="TResult">export type</typeparam>
        /// <param name="factory">export factory</param>
        /// <returns></returns>
        public IFluentExportInstanceConfiguration<TResult> ExportFactory<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> factory)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));

            var strategy = _strategyCreator.GetFactoryStrategy(factory);

            AddActivationStrategy(strategy);

            return new FluentExportInstanceConfiguration<TResult>(strategy, this);
        }

        /// <summary>
        /// Export a type to be used as a wrapper rather than export (types like Func(), Owned, Meta are wrapper types)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IFluentWrapperStrategyConfiguration ExportWrapper(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            var strategy = _strategyCreator.GetCompiledWrapperStrategy(type);

            _wrapperProviders = _wrapperProviders.Add(new SimpleExportWrapperProvider(strategy));

            return new FluentWrapperStrategyConfiguration(strategy);
        }

        /// <summary>
        /// Test if a type is exported
        /// </summary>
        /// <param name="type"></param>
        /// <param name="key"></param>
        /// <param name="excludeStrategy"></param>
        /// <returns></returns>
        public bool IsExported(Type type, object key = null, ICompiledExportStrategy excludeStrategy = null)
        {
            ProcessCurrentProvider();

            if (key != null)
            {
                if (_exportStrategyProviders.Any(s => s.ExportAsKeyed.Any(kvp =>
                {
                    return type == kvp.Key && key.Equals(kvp.Value) && !ReferenceEquals(excludeStrategy, s);
                })))
                {
                    return true;
                }
            }
            else
            {
                if (_exportStrategyProviders.Any(s => !ReferenceEquals(s,excludeStrategy) && s.ExportAs.Contains(type)))
                {
                    return true;
                }
            }

            var locateService = OwningScope.ScopeConfiguration.Implementation.Locate<ICanLocateTypeService>();

            var currentScope = OwningScope;

            while (currentScope != null)
            {
                if (locateService.CanLocate(currentScope, type, null, key, false))
                {
                    return true;
                }

                currentScope = currentScope.Parent as IInjectionScope;
            }

            return false;
        }

        /// <summary>
        /// Clears exports from current registration block, this does not unregister exports for previous configuration calls
        /// </summary>
        /// <param name="exportFilter"></param>
        /// <returns></returns>
        public bool ClearExports(Func<ICompiledExportStrategy, bool> exportFilter = null)
        {
            bool returnValue = false;

            ProcessCurrentProvider();

            if (exportFilter == null)
            {
                _exportStrategyProviders.Clear();

                return true;
            }

            for (int i = 0; i < _exportStrategyProviders.Count;)
            {
                if (!exportFilter(_exportStrategyProviders[i]))
                {
                    i++;
                }
                else
                {
                    returnValue = true;
                    _exportStrategyProviders.RemoveAt(i);
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Export a type that will be used as a decorator for exports
        /// </summary>
        /// <param name="type">decorator type</param>
        /// <returns></returns>
        public IFluentDecoratorStrategyConfiguration ExportDecorator(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            var strategy = _strategyCreator.GetCompiledDecoratorStrategy(type);

            _decoratorStrategyProviders = _decoratorStrategyProviders.Add(new SimpleDecoratorStrategyProvider(strategy));

            return new FluentDecoratorStrategyConfiguration(strategy);
        }

        /// <summary>
        /// Export a piece of logic that will be used to decorate exports upon creation
        /// </summary>
        /// <typeparam name="T">type to decorate</typeparam>
        /// <param name="apply">decorator logic</param>
        /// <param name="applyAfterLifestyle"></param>
        public IFluentDecoratorFactoryStrategyConfiguration ExportDecorator<T>(Func<T, T> apply, bool applyAfterLifestyle = true)
        {
            if (apply == null) throw new ArgumentNullException(nameof(apply));

            var strategy = _strategyCreator.GetFuncDecoratorStrategy(apply);

            strategy.ApplyAfterLifestyle = applyAfterLifestyle;

            _decoratorStrategyProviders = _decoratorStrategyProviders.Add(new SimpleDecoratorStrategyProvider(strategy));

            return new FluentDecoratorStrategyConfiguration(strategy);
        }
        
        /// <summary>
        /// Add injection inspector that will be called to inspect all exports, wrappers and decorators (apply cross cutting configuration with an inspector)
        /// </summary>
        /// <param name="inspector">inspector</param>
        public void AddInspector(IActivationStrategyInspector inspector)
        {
            if (inspector == null) throw new ArgumentNullException(nameof(inspector));

            _inspectors = _inspectors.Add(inspector);
        }


        /// <summary>
        /// Add IInjectionValueProvider allowing the developer to override the normal behavior for creating an injection value
        /// </summary>
        /// <param name="provider"></param>
        public void AddInjectionValueProvider(IInjectionValueProvider provider)
        {
            if (provider == null) throw new ArgumentNullException(nameof(provider));

            _valueProviders = _valueProviders.Add(provider);
        }

        /// <summary>
        /// Add missing export strategy provider
        /// </summary>
        /// <param name="provider"></param>
        public void AddMissingExportStrategyProvider(IMissingExportStrategyProvider provider)
        {
            if (provider == null) throw new ArgumentNullException(nameof(provider));

            _missingExportStrategyProviders = _missingExportStrategyProviders.Add(provider);
        }

        /// <summary>
        /// Add missing dependency expression provider
        /// </summary>
        /// <param name="provider"></param>
        public void AddMissingDependencyExpressionProvider(IMissingDependencyExpressionProvider provider)
        {
            _missingDependencyExpressionProviders = _missingDependencyExpressionProviders.Add(provider);
        }

        /// <summary>
        /// Add your own custom activation strategy
        /// </summary>
        /// <param name="activationStrategy">activation strategy</param>
        public void AddActivationStrategy(IActivationStrategy activationStrategy)
        {
            if (activationStrategy == null) throw new ArgumentNullException(nameof(activationStrategy));

            // ReSharper disable once CanBeReplacedWithTryCastAndCheckForNull
            if (activationStrategy is ICompiledExportStrategy)
            {
                AddExportStrategy((ICompiledExportStrategy)activationStrategy);
            }
            else if (activationStrategy is ICompiledDecoratorStrategy)
            {
                _decoratorStrategyProviders = _decoratorStrategyProviders.Add(
                     new SimpleDecoratorStrategyProvider((ICompiledDecoratorStrategy)activationStrategy));
            }
            else if (activationStrategy is ICompiledWrapperStrategy)
            {
                _wrapperProviders = _wrapperProviders.Add(new SimpleExportWrapperProvider((ICompiledWrapperStrategy)activationStrategy));
            }
            else
            {
                throw new NotSupportedException($"activation strategy of type {activationStrategy.GetType().Name} is not supported");
            }
        }

        /// <summary>
        /// Add your own strategy provider, usually used by 3rd party libraries to provide their own custom export types
        /// </summary>
        /// <param name="strategyProvider">strategy provider</param>
        public void AddExportStrategyProvider(IExportStrategyProvider strategyProvider)
        {
            if (strategyProvider == null) throw new ArgumentNullException(nameof(strategyProvider));

            _exportStrategyProviders.AddRange(strategyProvider.ProvideExportStrategies());
        }

        private void AddExportStrategy(ICompiledExportStrategy strategy)
        {
            ProcessCurrentProvider();
            
            _exportStrategyProviders.Add(strategy);
        }

        private void ProcessCurrentProvider()
        {
            if (_currentProvider != null)
            {
                _exportStrategyProviders.AddRange(_currentProvider.ProvideExportStrategies());

                _currentProvider = null;
            }
        }

    }
}
