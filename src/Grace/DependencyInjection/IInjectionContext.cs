using System;
using Grace.Data;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// Context that can be created for each request
    /// </summary>
    public interface IInjectionContext : IExtraDataContainer
    {
        /// <summary>
        /// Data container that is shared between all context for an object graph
        /// </summary>
        IInjectionContextSharedData SharedData { get; }

        /// <summary>
        /// Original object that was passed in as extra data when locate was called
        /// </summary>
        object ExtraData { get; }

        /// <summary>
        /// Get a value by type from the extra data
        /// </summary>
        /// <param name="type">type to locate</param>
        /// <returns>instance or null if one can't be found</returns>
        object GetValueByType(Type type);

        /// <summary>
        /// Clone the extra data provider
        /// </summary>
        /// <returns></returns>
        IInjectionContext Clone();
    }
}
