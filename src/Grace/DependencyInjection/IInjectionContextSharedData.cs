using Grace.Data;

namespace Grace.DependencyInjection
{
    public interface IInjectionContextSharedData : IExtraDataContainer
    {
        object GetLockObject(string lockName);
    }
}
