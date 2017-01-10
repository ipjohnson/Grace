using Grace.Data;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// Data object that is shared between the root context and children contexts for a request
    /// </summary>
    public interface IInjectionContextSharedData : IExtraDataContainer
    {
        /// <summary>
        /// Get a lock object by a specific name
        /// </summary>
        /// <param name="lockName"></param>
        /// <returns></returns>
        object GetLockObject(string lockName);
    }
}
