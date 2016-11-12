using System;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// interface to create new injection context
    /// </summary>
    public interface IInjectionContextCreator
    {
        /// <summary>
        /// Create new injection context
        /// </summary>
        /// <param name="type">type being requested</param>
        /// <param name="extraData">current extra data</param>
        /// <returns>new context</returns>
        IInjectionContext CreateContext(Type type, object extraData);
    }
}
