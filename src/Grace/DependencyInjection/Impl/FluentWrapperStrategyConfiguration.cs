using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grace.DependencyInjection.Impl
{
    public class FluentWrapperStrategyConfiguration : IFluentWrapperStrategyConfiguration
    {
        private readonly IConfigurableCompiledWrapperStrategy _compiledWrapperStrategy;

        public FluentWrapperStrategyConfiguration(IConfigurableCompiledWrapperStrategy compiledWrapperStrategy)
        {
            _compiledWrapperStrategy = compiledWrapperStrategy;
        }

        public IFluentWrapperStrategyConfiguration As(Type type)
        {
            _compiledWrapperStrategy.AddExportAs(type);

            return this;
        }

        public IFluentWrapperStrategyConfiguration WrappedType(Type type)
        {
            _compiledWrapperStrategy.SetWrappedType(type);

            return this;
        }

        public IFluentWrapperStrategyConfiguration WrappedGenericArg(int genericArgPosition)
        {
            _compiledWrapperStrategy.SetWrappedGenericArgPosition(genericArgPosition);

            return this;
        }
    }
}
