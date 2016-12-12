using System.Collections.Generic;
using Grace.DependencyInjection;

namespace Grace.Tests.DependencyInjection.AddOns
{
    public class MissingWrapperStrategyProviderTests
    {
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
    }
}
