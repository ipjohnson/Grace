using System.Collections.Generic;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// Simple class that returns a list of wrapper strategies
    /// </summary>
    public class SimpleExportWrapperProvider : IWrapperStrategyProvider
    {
        private readonly ICompiledWrapperStrategy[] _wrappers;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="wrappers"></param>
        public SimpleExportWrapperProvider(params ICompiledWrapperStrategy[] wrappers)
        {
            _wrappers = wrappers;
        }

        /// <summary>
        /// Provide list of wrappers
        /// </summary>
        /// <returns>list of wrappers</returns>
        public IEnumerable<ICompiledWrapperStrategy> ProvideWrappers()
        {
            return _wrappers;
        }
    }
}
