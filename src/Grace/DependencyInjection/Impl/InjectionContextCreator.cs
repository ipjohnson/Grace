using System;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// Creates new injection contexts
    /// </summary>
    public class InjectionContextCreator : IInjectionContextCreator
    {
        /// <summary>
        /// Create new injection context
        /// </summary>
        /// <param name="type">type being requested</param>
        /// <param name="extraData">current extra data</param>
        /// <returns>new context</returns>
        public IInjectionContext CreateContext(Type type, object extraData)
        {
            return extraData as IInjectionContext ?? new InjectionContext(extraData);
        }
    }
}
