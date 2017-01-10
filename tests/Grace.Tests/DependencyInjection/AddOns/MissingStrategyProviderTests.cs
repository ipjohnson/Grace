using System.Collections.Generic;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl.CompiledStrategies;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.AddOns
{
    public class MissingStrategyProviderTests
    {
        #region Decorator

        public class DecoratorProvider : IMissingExportStrategyProvider
        {
            public IEnumerable<IActivationStrategy> ProvideExports(IInjectionScope scope, IActivationExpressionRequest request)
            {
                if (request.ActivationType.IsConstructedGenericType &&
                    request.ActivationType.GetGenericTypeDefinition() == typeof(IDependentService<>))
                {
                    var decorator = new GenericCompiledDecoratorStrategy(typeof(DecoratorDependentService<>),scope, request.Services.LifestyleExpressionBuilder);

                    decorator.AddExportAs(typeof(IDependentService<>));

                    yield return decorator;

                    var export = new GenericCompiledExportStrategy(typeof(DependentService<>),scope, request.Services.LifestyleExpressionBuilder);

                    export.AddExportAs(typeof(IDependentService<>));

                    yield return export;
                }
            }
        }

        [Fact]
        public void Decorator_Provided_At_Missing()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.AddMissingExportStrategyProvider(new DecoratorProvider()));
            container.Configure(c => c.Export<BasicService>().As<IBasicService>());

            var instance = container.Locate<IDependentService<IBasicService>>();

            Assert.NotNull(instance);
            Assert.IsType<DecoratorDependentService<IBasicService>>(instance);
        }

        #endregion

        #region Missing wrapper
        public class WrapperClass<T>
        {
            public WrapperClass(T wrapped)
            {
                Wrapped = wrapped;
            }

            public T Wrapped { get; }
        }

        public class WrapperProvider : IMissingExportStrategyProvider
        {
            public IEnumerable<IActivationStrategy> ProvideExports(IInjectionScope scope, IActivationExpressionRequest request)
            {
                if (request.ActivationType.IsConstructedGenericType &&
                    request.ActivationType.GetGenericTypeDefinition() == typeof(WrapperClass<>))
                {
                    yield break;
                }
            }
        }

        #endregion
    }
}
