using System;
using System.Collections.Generic;
using Grace.Data.Immutable;
namespace Grace.DependencyInjection.Impl
{
    public class ExportRegistrationBlock : IExportRegistrationBlockValueProvider
    {
        private readonly List<IExportStrategyProvider> _exportStrategyProviders = new List<IExportStrategyProvider>();
        private ImmutableLinkedList<IWrapperStrategyProvider> _wrapperProviders = ImmutableLinkedList<IWrapperStrategyProvider>.Empty;
        private ImmutableLinkedList<IDecoratorStrategyProvider> _decoratorStrategyProviders = ImmutableLinkedList<IDecoratorStrategyProvider>.Empty;
        private ImmutableLinkedList<IActivationStrategyInspector> _inspectors = ImmutableLinkedList<IActivationStrategyInspector>.Empty;
        private readonly IActivationStrategyCreator _strategyCreator;

        public ExportRegistrationBlock(IInjectionScope owningScope, IActivationStrategyCreator strategyCreator)
        {
            _strategyCreator = strategyCreator;
            OwningScope = owningScope;
        }

        public IInjectionScope OwningScope { get; }

        public IEnumerable<ICompiledExportStrategy> GetExportStrategies()
        {
            foreach (var strategyProvider in _exportStrategyProviders)
            {
                foreach (var strategy in strategyProvider.ProvideExportStrategies())
                {
                    yield return strategy;
                }
            }
        }

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

        public IEnumerable<IActivationStrategyInspector> GetInspectors()
        {
            return _inspectors;
        }

        public IFluentExportStrategyConfiguration<T> Export<T>()
        {
            var strategy = _strategyCreator.GetCompiledExportStrategy(typeof(T));

            _exportStrategyProviders.Add(new SimpleExportStrategyProvider(strategy));

            return new FluentExportStrategyConfiguration<T>(strategy);
        }

        public IFluentExportStrategyConfiguration Export(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            var strategy = _strategyCreator.GetCompiledExportStrategy(type);

            _exportStrategyProviders.Add(new SimpleExportStrategyProvider(strategy));

            return new FluentExportStrategyConfiguration(strategy);
        }

        public IExportTypeSetConfiguration Export(IEnumerable<Type> types)
        {
            if (types == null) throw new ArgumentNullException(nameof(types));

            var configuration = _strategyCreator.GetTypeSetConfiguration(types);

            _exportStrategyProviders.Add((IExportStrategyProvider)configuration);

            return configuration;
        }

        public IFluentExportInstanceConfiguration<T> ExportInstance<T>(T instance)
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));

            var strategy = _strategyCreator.GetConstantStrategyFromConfiguration(instance);

            _exportStrategyProviders.Add(new SimpleExportStrategyProvider(strategy));

            return new FluentExportInstanceConfiguration<T>(strategy);
        }

        public IFluentExportInstanceConfiguration<T> ExportInstance<T>(Func<T> instanceFunc)
        {
            if (instanceFunc == null) throw new ArgumentNullException(nameof(instanceFunc));

            var strategy = _strategyCreator.GetFuncStrategy(instanceFunc);

            _exportStrategyProviders.Add(new SimpleExportStrategyProvider(strategy));

            return new FluentExportInstanceConfiguration<T>(strategy);
        }

        public IFluentExportInstanceConfiguration<T> ExportInstance<T>(Func<IExportLocatorScope, T> instanceFunc)
        {
            if (instanceFunc == null) throw new ArgumentNullException(nameof(instanceFunc));

            var strategy = _strategyCreator.GetFuncWithScopeStrategy(instanceFunc);

            _exportStrategyProviders.Add(new SimpleExportStrategyProvider(strategy));

            return new FluentExportInstanceConfiguration<T>(strategy);
        }

        public IFluentExportInstanceConfiguration<T> ExportInstance<T>(Func<IExportLocatorScope, StaticInjectionContext, T> instanceFunc)
        {
            if (instanceFunc == null) throw new ArgumentNullException(nameof(instanceFunc));

            var strategy = _strategyCreator.GetFuncWithStaticContextStrategy(instanceFunc);

            _exportStrategyProviders.Add(new SimpleExportStrategyProvider(strategy));

            return new FluentExportInstanceConfiguration<T>(strategy);
        }

        public IFluentExportInstanceConfiguration<T> ExportInstance<T>(
            Func<IExportLocatorScope, StaticInjectionContext, IInjectionContext, T> instanceFunc)
        {
            if (instanceFunc == null) throw new ArgumentNullException(nameof(instanceFunc));

            var strategy = _strategyCreator.GetFuncWithInjectionContextStrategy(instanceFunc);

            _exportStrategyProviders.Add(new SimpleExportStrategyProvider(strategy));

            return new FluentExportInstanceConfiguration<T>(strategy);
        }

        public IFluentExportInstanceConfiguration<TResult> ExportFactory<TIn, TResult>(Func<TIn, TResult> factory)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));

            var strategy = _strategyCreator.GetFactoryStrategy(factory);

            _exportStrategyProviders.Add(new SimpleExportStrategyProvider(strategy));

            return new FluentExportInstanceConfiguration<TResult>(strategy);
        }

        public IFluentExportInstanceConfiguration<TResult> ExportFactory<T1, T2, TResult>(Func<T1, T2, TResult> factory)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));

            var strategy = _strategyCreator.GetFactoryStrategy(factory);

            _exportStrategyProviders.Add(new SimpleExportStrategyProvider(strategy));

            return new FluentExportInstanceConfiguration<TResult>(strategy);
        }

        public IFluentExportInstanceConfiguration<TResult> ExportFactory<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> factory)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));

            var strategy = _strategyCreator.GetFactoryStrategy(factory);

            _exportStrategyProviders.Add(new SimpleExportStrategyProvider(strategy));

            return new FluentExportInstanceConfiguration<TResult>(strategy);
        }

        public IFluentExportInstanceConfiguration<TResult> ExportFactory<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> factory)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));

            var strategy = _strategyCreator.GetFactoryStrategy(factory);

            _exportStrategyProviders.Add(new SimpleExportStrategyProvider(strategy));

            return new FluentExportInstanceConfiguration<TResult>(strategy);
        }

        public IFluentExportInstanceConfiguration<TResult> ExportFactory<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> factory)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));

            var strategy = _strategyCreator.GetFactoryStrategy(factory);

            _exportStrategyProviders.Add(new SimpleExportStrategyProvider(strategy));

            return new FluentExportInstanceConfiguration<TResult>(strategy);
        }

        public IFluentWrapperStrategyConfiguration ExportWrapper(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            var strategy = _strategyCreator.GetCompiledWrapperStrategy(type);

            _wrapperProviders = _wrapperProviders.Add(new SimpleExportWrapperProvider(strategy));

            return new FluentWrapperStrategyConfiguration(strategy);
        }

        public IFluentDecoratorStrategyConfiguration ExportDecorator(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            var strategy = _strategyCreator.GetCompiledDecoratorStrategy(type);

            _decoratorStrategyProviders = _decoratorStrategyProviders.Add(new SimpleDecoratorStrategyProvider(strategy));

            return new FluentDecoratorStrategyConfiguration(strategy);
        }

        public void ExportDecorator<T>(Func<T, T> apply)
        {
            if (apply == null) throw new ArgumentNullException(nameof(apply));

            var strategy = _strategyCreator.GetFuncDecoratorStrategy(apply);

            _decoratorStrategyProviders = _decoratorStrategyProviders.Add(new SimpleDecoratorStrategyProvider(strategy));
        }

        public void ExportInitialize<T>(Action<T> apply)
        {
            if (apply == null) throw new ArgumentNullException(nameof(apply));

            ExportDecorator<T>(x =>
            {
                apply(x);

                return x;
            });
        }

        public void AddInspector(IActivationStrategyInspector inspector)
        {
            if (inspector == null) throw new ArgumentNullException(nameof(inspector));

            _inspectors = _inspectors.Add(inspector);
        }

        public void AddActivationStrategy(IActivationStrategy activationStrategy)
        {
            if (activationStrategy == null) throw new ArgumentNullException(nameof(activationStrategy));

            if (activationStrategy is ICompiledExportStrategy)
            {
                _exportStrategyProviders.Add(new SimpleExportStrategyProvider((ICompiledExportStrategy)activationStrategy));
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

        public void AddExportStrategyProvider(IExportStrategyProvider strategyProvider)
        {
            if (strategyProvider == null) throw new ArgumentNullException(nameof(strategyProvider));

            _exportStrategyProviders.Add(strategyProvider);
        }
    }
}
