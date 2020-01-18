using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl.Wrappers;
using Grace.Tests.Classes.Simple;
using Optional;
using Xunit;

namespace Grace.Tests.ThridParty.Optional
{
    /// <summary>
    /// OPtional strategy provider
    /// </summary>
    public class OptionalStrategyProvider : IMissingExportStrategyProvider
    {
        /// <summary>
        /// Can a given request be located using this provider
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public bool CanLocate(IInjectionScope scope, IActivationExpressionRequest request)
        {
            return request.ActivationType.IsConstructedGenericType &&
                   request.ActivationType.GetGenericTypeDefinition() == typeof(Option<>);
        }

        /// <summary>
        /// Provide exports for a missing type
        /// </summary>
        /// <param name="scope">scope to provide value</param>
        /// <param name="request">request</param>
        /// <returns>set of activation strategies</returns>
        public IEnumerable<IActivationStrategy> ProvideExports(IInjectionScope scope, IActivationExpressionRequest request)
        {
            if (request.ActivationType.IsConstructedGenericType &&
                request.ActivationType.GetGenericTypeDefinition() == typeof(Option<>))
            {
                yield return new OptionalStrategy(scope);
            }
        }
    }

    public class OptionalTests
    {
        [Fact]
        public void OptionalIntHasValueFalse()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.AddMissingExportStrategyProvider(new OptionalStrategyProvider()));

            var instance = container.Locate<Option<int>>();

            Assert.False(instance.HasValue);
        }

        [Fact]
        public void OptionalIntHasValueTrue()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.ExportInstance(5);
                c.AddMissingExportStrategyProvider(new OptionalStrategyProvider());
            });

            var instance = container.Locate<Option<int>>();

            Assert.True(instance.HasValue);
            Assert.Equal(5, instance.ValueOr(() => throw new Exception("Not supposed to hit this")));
        }

        [Fact]
        public void OptionalBasicServiceHasValueFalse()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.AddMissingExportStrategyProvider(new OptionalStrategyProvider()));

            var instance = container.Locate<Option<IBasicService>>();

            Assert.False(instance.HasValue);
        }
        
        [Fact]
        public void OptionalBasicServiceHasValueTrue()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.ExportAs<BasicService, IBasicService>();
                c.AddMissingExportStrategyProvider(new OptionalStrategyProvider());
            });

            var instance = container.Locate<Option<IBasicService>>();

            Assert.True(instance.HasValue);
            Assert.IsType<BasicService>(instance.ValueOr(() => throw new Exception("Not supposed to hit this")));
        }

        [Fact]
        public void OptionalBasicServiceWithKeyAndIsSatisfied()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().AsKeyed<IBasicService>("basic");
                var optionalStrategy = new OptionalWrapperStrategy(c.OwningScope);
                c.AddActivationStrategy(optionalStrategy);
                c.AddMissingDependencyExpressionProvider(optionalStrategy);
            });

            var instance = container.Locate<Optional<IBasicService>>(withKey: "basic");

            Assert.True(instance.IsSatisfied());

            var value = instance.Value;
            Assert.NotNull(value);
        }

        [Fact]
        public void OptionalBasicServiceWithKeyAndNotIsSatisfied()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                var optionalStrategy = new OptionalWrapperStrategy(c.OwningScope);
                c.AddActivationStrategy(optionalStrategy);
                c.AddMissingDependencyExpressionProvider(optionalStrategy);
            });

            var instance = container.Locate<Optional<IBasicService>>(withKey:"basic");

            Assert.False(instance.IsSatisfied());

            var value = instance.Value;
            Assert.Null(value);
        }

        [Fact]
        public void OptionalBasicServiceWithKeyAndScopedService()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().AsKeyed<IBasicService>("basic").Lifestyle.SingletonPerScope();
                var optionalStrategy = new OptionalWrapperStrategy(c.OwningScope);
                c.AddActivationStrategy(optionalStrategy);
                c.AddMissingDependencyExpressionProvider(optionalStrategy);
            });

            var scope = container.CreateChildScope();

            var instance1 = scope.Locate<Optional<IBasicService>>(withKey: "basic");
            var instance2 = scope.Locate<Optional<IBasicService>>(withKey: "basic");

            Assert.True(instance1.IsSatisfied());
            Assert.True(instance2.IsSatisfied());

            var value1 = instance1.Value;
            Assert.NotNull(value1);
            var value2 = instance2.Value;
            Assert.NotNull(value2);

            Assert.Same(value1, value2);
        }
    }



    /// <summary>
    /// Wrapper Strategy for <see cref="Optional{T}"/>.
    /// </summary>
    public class OptionalWrapperStrategy : BaseKeyWrapperStrategy,
        IMissingExportStrategyProvider, IMissingDependencyExpressionProvider
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="injectionScope">InjectionScope</param>
        public OptionalWrapperStrategy(IInjectionScope injectionScope)
            : base(typeof(Optional<>), injectionScope)
        {
        }

        public ActivationStrategyDelegate GetActivationStrategyDelegate(
            IInjectionScope scope, 
            IActivationStrategyCompiler compiler, 
            Type activationType, 
            object key)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get an activation expression for this strategy
        /// </summary>
        /// <param name="scope">IInjectionScope</param>
        /// <param name="request">IActivationExpressionRequest</param>
        /// <returns>IActivationExpressionResult</returns>
        public override IActivationExpressionResult GetActivationExpression(
            IInjectionScope scope,
            IActivationExpressionRequest request)
        {
            var activationDelegate = CompileDelegate(scope, request);

            var factory = typeof(GraceOptionalFactory<>)
                .MakeGenericType(request.ActivationType.GenericTypeArguments[0])
                .GetConstructors()
                .First()
                .Invoke(new object[] { activationDelegate, request.LocateKey });

            var callExpression = Expression.Call(
                Expression.Constant(factory),
                factory.GetType().GetMethod(nameof(GraceOptionalFactory<object>.CreateOptional)),
                request.ScopeParameter,
                request.DisposalScopeExpression,
                request.InjectionContextParameter);

            request.RequireInjectionContext();

            return request.Services.Compiler.CreateNewResult(request, callExpression);
        }

        /// <summary>
        /// Get type that wrapper wraps
        /// </summary>
        /// <param name="type">wrapper type</param>
        /// <returns>type that has been wrapped</returns>
        public override Type GetWrappedType(Type type)
        {
            if (!type.IsConstructedGenericType)
            {
                return null;
            }

            return type.GenericTypeArguments[0];
        }

        private ActivationStrategyDelegate CompileDelegate(
            IInjectionScope scope,
            IActivationExpressionRequest request)
        {
            var requestType = request.ActivationType.GenericTypeArguments[0];
            var implementationType = typeof(GraceOptional<>).MakeGenericType(requestType);

            var newRequest = request.NewRequest(requestType, this, implementationType, RequestType.Other, null, true);

            if (request.LocateKey != null)
                newRequest.SetLocateKey(request.LocateKey);

            newRequest.DisposalScopeExpression = request.Constants.RootDisposalScope;
            newRequest.SetIsRequired(false);

            var activationExpression = request.Services.ExpressionBuilder
                .GetActivationExpression(scope, newRequest);

            var _delegate = request.Services.Compiler.CompileDelegate(scope, activationExpression);

            return _delegate;
        }

        #region IMissingExportStrategyProvider e IMissingDependencyExpressionProvider

        public bool CanLocate(IInjectionScope scope, IActivationExpressionRequest request)
        {
            return request.ActivationType.IsConstructedGenericType
                && request.ActivationType.GetGenericTypeDefinition() == typeof(Optional<>);
        }

        public IEnumerable<IActivationStrategy> ProvideExports(
            IInjectionScope scope, IActivationExpressionRequest request)
        {
            if (CanLocate(scope, request))
                yield return this;
        }

        public IActivationExpressionResult ProvideExpression(IInjectionScope scope, IActivationExpressionRequest request)
        {

            return CanLocate(scope, request) ? GetActivationExpression(scope, request) : null;
        }

        

        #endregion
    }

    /// <summary>
    /// Factory for <see cref="Optional{T}"/>.
    /// </summary>
    public class GraceOptionalFactory<TResult>
    {
        private ActivationStrategyDelegate _delegate;
        private string qualifier;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="activationDelegate">Delegate for create the wrapped service.</param>
        /// <param name="locateKey">Key of wrapped service.</param>
        public GraceOptionalFactory(ActivationStrategyDelegate activationDelegate, object locateKey)
        {
            _delegate = activationDelegate;
            qualifier = locateKey as string;
        }

        /// <summary>
        /// Create GraceOptional instance.
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="disposalScope"></param>
        /// <param name="injectionContext"></param>
        /// <returns></returns>
        public GraceOptional<TResult> CreateOptional(
            IExportLocatorScope scope,
            IDisposalScope disposalScope,
            IInjectionContext injectionContext)
        {
            return new GraceOptional<TResult>(scope, qualifier, () =>
            {
                return (TResult)_delegate(scope, disposalScope, injectionContext);
            });
        }
    }

    /// <summary>
    /// Implemetation of Optional for Grace.
    /// </summary>
    /// <typeparam name="TService">Wrapped service type.</typeparam>
    public class GraceOptional<TService> : Optional<TService>
    {
        private readonly IExportLocatorScope _locator;
        private readonly Func<TService> _delegate;
        private readonly string _qualifier;

        /// <summary>
        /// Determines whether the value has already been created.
        /// </summary>
        public bool IsValueCreated { get; private set; }

        /// <summary>
        /// Service reference.
        /// </summary>
        private TService _value;

        /// <summary>
        /// Get the service.
        /// </summary>
        public TService Value
        {
            get
            {
                if (!IsValueCreated)
                {
                    if (IsSatisfied())
                        _value = _delegate();

                    IsValueCreated = true;
                }
                return _value;
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public GraceOptional(IExportLocatorScope locator, string qualifier, Func<TService> @delegate)
        {
            _locator = locator;
            _qualifier = qualifier;
            _delegate = @delegate;
        }

        /// <summary>
        /// Determines whether the service exists, if it was registered in the container.
        /// </summary>
        /// <returns>True if it exists, false otherwise.</returns>
        public bool IsSatisfied() => _locator.CanLocate(typeof(TService), key: _qualifier);
    }

    /// <summary>
    /// Componente para injeção de serviços opcionais.
    /// </summary>
    /// <typeparam name="T">Tipo de dado do serviço.</typeparam>
    public interface Optional<out T>
    {
        /// <summary>
        /// Retorna o serviço ou nulo.
        /// </summary>
        T Value { get; }

        /// <summary>
        /// Verifica se o serviço foi carregado.
        /// </summary>
        bool IsValueCreated { get; }

        /// <summary>
        /// Verifica se o serviço existe.
        /// </summary>
        /// <returns>Verdadeiro se existe o registro do serviço, falso caso contrário.</returns>
        bool IsSatisfied();
    }
}
