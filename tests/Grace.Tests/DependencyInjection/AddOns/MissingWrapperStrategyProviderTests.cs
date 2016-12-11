using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl.CompiledStrategies;

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
