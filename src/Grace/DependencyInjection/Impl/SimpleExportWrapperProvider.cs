using System.Collections.Generic;

namespace Grace.DependencyInjection.Impl
{
    public class SimpleExportWrapperProvider : IWrapperStrategyProvider
    {
        private readonly ICompiledWrapperStrategy[] _wrappers;

        public SimpleExportWrapperProvider(params ICompiledWrapperStrategy[] wrappers)
        {
            _wrappers = wrappers;
        }

        public IEnumerable<ICompiledWrapperStrategy> ProvideWrappers()
        {
            return _wrappers;
        }
    }
}
